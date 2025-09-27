import sys
import socket
import os
import pandas as pd
from sqlalchemy import create_engine, inspect, MetaData, text
import time
import numpy as np
from datetime import datetime, timedelta
import sqlalchemy
from sqlalchemy.dialects.mysql import insert
import concurrent.futures
import warnings
import json
import logging
from sqlalchemy.sql import case
import gc
import math
import io
import time
from sqlalchemy import MetaData
from DragnetUtilities import create_table

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
logging.basicConfig(level=logging.INFO)
warnings.simplefilter(action='ignore', category=FutureWarning)

if len(sys.argv) < 11:
    print("Data pass incomplete. Values missing or in error")
    time.sleep(10)
    sys.exit(1)

"""node_id = 1
curator_id = 'testCurator'
dragnet_db_host = '192.168.1.210'
dragnet_db_username = 'dragnet'
dragnet_db_password = 'dragnet5'
control_db_host = '192.168.1.210'
control_db_user = 'dragnet'
control_db_pass = 'dragnet5'
start_asset = 'B'
end_asset = 'B'
delay = """

curator_id = sys.argv[1]
dragnet_db_host = sys.argv[2]
dragnet_db_username = sys.argv[3]
dragnet_db_password = sys.argv[4]
control_db_host = sys.argv[5]
control_db_user = sys.argv[6]
control_db_pass = sys.argv[7]
start_asset = sys.argv[8]
end_asset = sys.argv[9]
batch_size = int(sys.argv[10])
workers = int(sys.argv[11])

engine_control = f'mysql+mysqldb://{control_db_user}:{control_db_pass}@{control_db_host}'
control_engine = create_engine(engine_control)
engine_dragnet = f'mysql+mysqldb://{dragnet_db_username}:{dragnet_db_password}@{dragnet_db_host}/dragnet'
dragnet_engine = create_engine(engine_dragnet)
inspector = inspect(dragnet_engine)

node_ip = 'error'
try:
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.connect(('8.8.8.8', 80))  # Google's DNS; not actually contacted
    node_ip = s.getsockname()[0]
    s.close()
except Exception as e:
    log_module_event(control_engine, 'curator', curator_id, node_ip, 'error', str(e))
    raise

now = datetime.now()

def log_module_event(control_engine, module_type, module_id, module_ip, log_level, message):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.module_logs (timestamp, module_ip, module_type, module_id, log_level, message)
            VALUES (:timestamp, :module_ip, :module_type, :module_id, :log_level, :message)
        """)
        conn.execute(query, {
            'timestamp': now,
            'module_ip': module_ip,
            'module_type': module_type,
            'module_id': module_id,# e.g. 'curator', 'scanner', etc.      # usually curator_id or similar            # if you track nodes, else pass None or 0
            'log_level': log_level,        # 'info', 'warning', 'error'
            'message': message
        })
        conn.commit()
try:
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.curator_modules
                (curator_id, node_ip, asset_range_start, asset_range_end, status, created_at, last_heartbeat)
            VALUES
                (:curator_id, :node_ip, :asset_range_start, :asset_range_end, :status, :created_at, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                curator_id = VALUES(curator_id),
                node_ip = VALUES(node_ip),
                asset_range_start = VALUES(asset_range_start),
                asset_range_end = VALUES(asset_range_end),
                status = "Running",
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'curator_id': str(curator_id),
            'node_ip': node_ip,
            'asset_range_start': start_asset,
            'asset_range_end': end_asset,
            'status': ("Running"),
            'created_at': now,
            'last_heartbeat': now
        })
        conn.commit()
except Exception as e:
    log_module_event(control_engine, 'curator', curator_id, node_ip, 'error', str(e))
    raise

def set_heartbeat(control_engine, curator_id):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.curator_modules (curator_id, last_heartbeat)
            VALUES (:curator_id, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'curator_id': curator_id,
            'last_heartbeat': now
        })
        conn.commit()

all_assets = sorted(inspector.get_table_names())  # get all asset names, sorted alphabetically

# Find indices in the sorted list
try:
    start_idx = all_assets.index(start_asset)
except ValueError:
    raise Exception(f"Start asset '{start_asset}' not found!")
try:
    end_idx = all_assets.index(end_asset)
except ValueError:
    raise Exception(f"End asset '{end_asset}' not found!")
# Ensure correct order
if start_idx > end_idx:
    start_idx, end_idx = end_idx, start_idx
ASSETS = all_assets[start_idx:end_idx + 1]  # inclusive
logging.info(f"Assets to process: {ASSETS}")

def acquire_lock(asset, duration_minutes=10):
    now = datetime.now()
    expiration = now + timedelta(minutes=duration_minutes)
    query = text(f"""
        INSERT INTO dragnetcontrol.dragnet_locks (asset_name, locked_by, lock_expiration)
        VALUES (:asset, :worker, :expiration)
        ON DUPLICATE KEY UPDATE
            locked_by = IF(lock_expiration < :now, :worker, locked_by),
            lock_expiration = IF(lock_expiration < :now, :expiration, lock_expiration)
    """)
    select_query = text(f"""
        SELECT locked_by FROM dragnetcontrol.dragnet_locks
        WHERE asset_name = :asset
    """)
    with control_engine.connect() as conn:
        with conn.begin():
            conn.execute(query, {
                'asset': asset,
                'worker': curator_id,
                'expiration': expiration,
                'now': now
            })
            result = conn.execute(select_query, {'asset': asset}).scalar()
    return result == curator_id

def release_lock(asset):
    query = text("""
        DELETE FROM dragnetcontrol.dragnet_locks
        WHERE asset_name = :asset AND locked_by = :worker
    """)
    with control_engine.connect() as conn:
        with conn.begin():
            conn.execute(query, {'asset': asset, 'worker': curator_id})

def process_with_lock(asset):
    if acquire_lock(asset):
        try:
            process_asset_combined(asset)
        finally:
            release_lock(asset)

def safe_jsonify(val):
    if pd.isna(val) or val is None or (isinstance(val, str) and val.strip().lower() == "null"):
        return None
    if isinstance(val, (dict, list)):
        return json.dumps(val)  # Serialize dict/list to string
    if isinstance(val, str):
        val = val.strip()
        # Try to load string as JSON; if it fails, NULL it out
        try:
            loaded = json.loads(val)
            return json.dumps(loaded)  # Re-serialize to ensure MySQL likes it
        except Exception:
            return None
    return None  # For numbers, etc. (or be stricter)

def upsert_dragnet_row(dragnet_engine, df, asset):
    if df.empty or len(df) == 0:
        print(f"[{asset}] DataFrame is empty, skipping upsert.")
        return

    table_name = asset
    json_cols = [
        "order_book", "order_book2", "order_book3",
        "market_depth", "market_depth2", "market_depth3",
        "top_of_book_depth", "top_of_book_depth2", "top_of_book_depth3",
        "order_book_slope", "order_book_slope2", "order_book_slope3",
        "price_action", "VIPTrades"
    ]
    # Clean and serialize JSON columns
    for col in json_cols:
        if col in df.columns:
            df[col] = df[col].apply(safe_jsonify)
    df = df.replace({np.nan: None})

    cols = [f"`{col}`" for col in df.columns]
    cols_str = ', '.join(cols)
    placeholders = ', '.join(['%s'] * len(cols))
    upserts = ', '.join([f"{col}=VALUES({col})" for col in cols if col != '`timestamp`'])

    sql = (
        f"INSERT INTO `{table_name}` ({cols_str}) VALUES ({placeholders}) "
        f"ON DUPLICATE KEY UPDATE {upserts}"
    )
    data = [tuple(row) for row in df.values]

    if not data:
        print(f"[{asset}] No data to upsert, skipping SQL execution.")
        return

    raw_conn = dragnet_engine.raw_connection()
    try:
        with raw_conn.cursor() as cursor:
            try:
                cursor.executemany(sql, data)
                raw_conn.commit()
            except Exception as ex:
                print(f"[{asset}] SQL upsert failed: {ex}")
                # Optional: log to a file or database here for later review
    finally:
        raw_conn.close()

def last_non_null(series):
    """Helper aggregator: return the last non-null value in series, or None if none."""
    valid = series.dropna()
    return valid.iloc[-1] if not valid.empty else None

def first_non_null(series):
    """Helper aggregator: return the first non-null value in series, or None if none."""
    valid = series.dropna()
    return valid.iloc[0] if not valid.empty else None

def vectorized_1min_aggregation(master_df):
    if master_df.empty:
        logging.warning("vectorized_1min_aggregation: Got empty DataFrame.")
        return pd.DataFrame()

    df = master_df.set_index('timestamp', drop=True).copy()
    agg_dict = {}

    suffixes = ["", "2", "3"]
    price_cols = ["market_price", "ask_price", "bid_price"]
    candle_cols = ["open_price", "high_price", "low_price", "close_price", "volume"]

    for s in suffixes:
        for pc in price_cols:
            colname = pc + s if s else pc
            if colname in df.columns:
                agg_dict[colname] = last_non_null
        for cc in candle_cols:
            colname = cc + s if s else cc
            if colname not in df.columns:
                continue
            if cc.startswith("open_price"):
                agg_dict[colname] = first_non_null
            elif cc.startswith("high_price"):
                agg_dict[colname] = "max"
            elif cc.startswith("low_price"):
                agg_dict[colname] = "min"
            elif cc.startswith("close_price"):
                agg_dict[colname] = last_non_null
            elif cc.startswith("volume"):
                agg_dict[colname] = "sum"

    for ob_col in ["order_book", "order_book2", "order_book3"]:
        if ob_col in df.columns:
            agg_dict[ob_col] = last_non_null

    # **CRUCIAL PATCH**
    if not agg_dict:
        logging.warning("vectorized_1min_aggregation: agg_dict is empty! Columns present: %s", df.columns.tolist())
        # Return a minimal DataFrame with just the timestamp (if present)
        # If you want, you can return an empty DataFrame, or one with the original timestamps:
        # Option 1: Return just timestamp index as a DataFrame
        return pd.DataFrame({"timestamp": df.index})

    grouped = df.groupby(pd.Grouper(freq='T', sort=True))
    agg_df = grouped.agg(agg_dict)

    agg_df.reset_index(inplace=True)
    agg_df.rename(columns={"timestamp": "timestamp"}, inplace=True)  # for clarity
    return agg_df


##############################################################################
# The rest of your code remains mostly the same
##############################################################################

def calculate_ichimoku(df, high_col, low_col, close_col, suffix=''):
    period9_high = df[high_col].rolling(window=9).max()
    period9_low = df[low_col].rolling(window=9).min()
    df[f'Tenken_Sen{suffix}'] = (period9_high + period9_low) / 2

    period26_high = df[high_col].rolling(window=26).max()
    period26_low = df[low_col].rolling(window=26).min()
    df[f'Kijun_Sen{suffix}'] = (period26_high + period26_low) / 2

    df[f'Senkou_A{suffix}'] = ((df[f'Tenken_Sen{suffix}'] + df[f'Kijun_Sen{suffix}']) / 2).shift(26)

    period52_high = df[high_col].rolling(window=52).max()
    period52_low = df[low_col].rolling(window=52).min()
    df[f'Senkou_B{suffix}'] = ((period52_high + period52_low) / 2).shift(26)

    df[f'Chicku_Span{suffix}'] = df[close_col]
    return df

def calculate_rsi(data, period=14):
    delta = data.diff()
    gain = delta.where(delta > 0, 0)
    loss = -delta.where(delta < 0, 0)
    avg_gain = gain.rolling(window=period).mean()
    avg_loss = loss.rolling(window=period).mean()
    rs = avg_gain / avg_loss
    return 100 - (100 / (1 + rs))

def calculate_macd(data, short_period=12, long_period=26, signal_period=9):
    exp1 = data.ewm(span=short_period, adjust=False).mean()
    exp2 = data.ewm(span=long_period, adjust=False).mean()
    macd_line = exp1 - exp2
    signal_line = macd_line.ewm(span=signal_period, adjust=False).mean()
    macd_histogram = macd_line - signal_line
    return macd_line, signal_line, macd_histogram

def calculate_stochastic_oscillator(df, high_col, low_col, close_col, k_period=14, d_period=3):
    low_min = df[low_col].rolling(window=k_period).min()
    high_max = df[high_col].rolling(window=k_period).max()
    percent_k = 100 * (df[close_col] - low_min) / (high_max - low_min)
    percent_d = percent_k.rolling(window=d_period).mean()
    return percent_k, percent_d

def calculate_sma(data, period=15):
    return data.rolling(window=period).mean()

def calculate_ema(data, periods=[12, 26, 50, 200]):
    return {f'EMA{period}P': data.ewm(span=period, adjust=False).mean() for period in periods}

def calculate_atr_values(high_prices, low_prices, close_prices, periods=[7, 14, 20, 50, 100]):
    true_ranges = []
    for i in range(len(high_prices)):
        if i == 0:
            tr = high_prices.iloc[i] - low_prices.iloc[i]
        else:
            current_high = high_prices.iloc[i]
            current_low = low_prices.iloc[i]
            previous_close = close_prices.iloc[i - 1]
            tr = max(
                current_high - current_low,
                abs(current_high - previous_close),
                abs(current_low - previous_close)
            )
        true_ranges.append(tr)
    true_ranges = pd.Series(true_ranges, index=high_prices.index)
    atr_values = {}
    for period in periods:
        atr = true_ranges.rolling(window=period).mean()
        atr_values[f'ATR{period}P'] = atr
    return atr_values

def calculate_bollinger_bands(data, period=20, num_std_dev=2):
    sma = calculate_sma(data, period)
    rolling_std = data.rolling(window=period).std()
    upper_band = sma + (rolling_std * num_std_dev)
    lower_band = sma - (rolling_std * num_std_dev)
    return pd.DataFrame({'Middle': sma, 'Upper': upper_band, 'Lower': lower_band})

def calculate_buy_sell_ratio(order_book):
    if not order_book.get('bids') or not order_book.get('asks'):
        return None
    total_bid_volume = sum(float(bid[1]) for bid in order_book['bids'])
    total_ask_volume = sum(float(ask[1]) for ask in order_book['asks'])
    if total_ask_volume == 0:
        return None
    return total_bid_volume / total_ask_volume

def calculate_price_impact(order_book, amount=1.0):
    if not order_book.get('asks'):
        return None
    total_volume = 0.0
    total_cost = 0.0
    for ask in order_book['asks']:
        price = float(ask[0])
        volume = float(ask[1])
        if total_volume + volume >= amount:
            volume_needed = amount - total_volume
            total_cost += price * volume_needed
            break
        else:
            total_cost += price * volume
            total_volume += volume
    if total_volume == 0:
        return None
    average_price = total_cost / amount
    best_ask = float(order_book['asks'][0][0])
    return (average_price - best_ask) / best_ask

def calculate_order_book_slope(order_book, depth=10):
    if not order_book.get('bids') or not order_book.get('asks'):
        return None
    def linear_regression_slope(x, y):
        n = len(x)
        if n == 0:
            return None
        sum_x = sum(x)
        sum_y = sum(y)
        sum_xx = sum(xi * xi for xi in x)
        sum_xy = sum(xi * yi for xi, yi in zip(x, y))
        numerator = n * sum_xy - sum_x * sum_y
        denominator = n * sum_xx - sum_x * sum_x
        if denominator == 0:
            return None
        return numerator / denominator

    bid_prices = [float(bid[0]) for bid in order_book['bids'][:depth]]
    bid_volumes = [float(bid[1]) for bid in order_book['bids'][:depth]]
    bid_cumulative_volumes = []
    cumulative = 0
    for volume in bid_volumes:
        cumulative += volume
        bid_cumulative_volumes.append(cumulative)
    bid_slope = linear_regression_slope(bid_prices, bid_cumulative_volumes)

    ask_prices = [float(ask[0]) for ask in order_book['asks'][:depth]]
    ask_volumes = [float(ask[1]) for ask in order_book['asks'][:depth]]
    ask_cumulative_volumes = []
    cumulative = 0
    for volume in ask_volumes:
        cumulative += volume
        ask_cumulative_volumes.append(cumulative)
    ask_slope = linear_regression_slope(ask_prices, ask_cumulative_volumes)
    return {'bid_slope': bid_slope, 'ask_slope': ask_slope}

def calculate_bid_ask_spread(order_book):
    try:
        if not order_book['bids'] or not order_book['asks']:
            return None
        best_bid = float(order_book['bids'][0][0])
        best_ask = float(order_book['asks'][0][0])
        return best_ask - best_bid
    except (IndexError, KeyError, ValueError) as e:
        logging.warning(f"Error calculating bid-ask spread: {e}")
        return None

def calculate_market_depth(order_book, depth=10):
    if not order_book.get('bids') or not order_book.get('asks'):
        return None, None
    bids = order_book['bids'][:depth]
    asks = order_book['asks'][:depth]
    bid_volume = sum(float(bid[1]) for bid in bids)
    ask_volume = sum(float(ask[1]) for ask in asks)
    return bid_volume, ask_volume

def calculate_liquidity_imbalance(order_book, depth=10):
    try:
        if not order_book['bids'] or not order_book['asks']:
            return None
        bid_volume, ask_volume = calculate_market_depth(order_book, depth)
        if (bid_volume + ask_volume) == 0:
            return None
        return (bid_volume - ask_volume) / (bid_volume + ask_volume)
    except (IndexError, KeyError, ValueError) as e:
        logging.warning(f"Error calculating order book imbalance: {e}")
        return None

def calculate_cvd(order_books):
    try:
        cvd = 0
        for order_book in order_books:
            if not order_book or not order_book.get('bids') or not order_book.get('asks'):
                continue
            bid_volume, ask_volume = calculate_market_depth(order_book)
            if bid_volume is not None and ask_volume is not None:
                cvd += (bid_volume - ask_volume)
        return cvd if cvd != 0 else None
    except Exception as e:
        log_module_event(control_engine, 'curator', curator_id, node_ip, 'error', str(e))
        raise
        return None

def calculate_order_flow_ratio(order_book):
    bid_volume, ask_volume = calculate_market_depth(order_book)
    if bid_volume is None or ask_volume is None:
        return None
    total_volume = bid_volume + ask_volume
    if total_volume == 0:
        return None
    return bid_volume / total_volume

def calculate_vwap(order_book, depth=10):
    if not order_book.get('bids') or not order_book.get('asks'):
        return None
    weighted_bid_sum = sum(float(bid[0]) * float(bid[1]) for bid in order_book['bids'][:depth])
    bid_volume = sum(float(bid[1]) for bid in order_book['bids'][:depth])

    weighted_ask_sum = sum(float(ask[0]) * float(ask[1]) for ask in order_book['asks'][:depth])
    ask_volume = sum(float(ask[1]) for ask in order_book['asks'][:depth])
    if (bid_volume + ask_volume) == 0:
        return None
    return (weighted_bid_sum + weighted_ask_sum) / (bid_volume + ask_volume)

def calculate_order_flow_imbalance(order_book):
    total_buy_volume = sum(float(bid[1]) for bid in order_book.get('bids', []))
    total_sell_volume = sum(float(ask[1]) for ask in order_book.get('asks', []))
    total_volume = total_buy_volume + total_sell_volume
    if total_volume == 0:
        return None
    return (total_buy_volume - total_sell_volume) / total_volume

def calculate_order_flow_ratio(order_book):
    total_buy_volume = sum(float(bid[1]) for bid in order_book.get('bids', []))
    total_sell_volume = sum(float(ask[1]) for ask in order_book.get('asks', []))
    if total_sell_volume == 0:
        return None
    return total_buy_volume / total_sell_volume

def calculate_order_book_pressure(order_book, depth=10):
    if not order_book.get('bids') or not order_book.get('asks'):
        return None
    total_bid = sum(float(bid[1]) for bid in order_book['bids'][:depth])
    total_ask = sum(float(ask[1]) for ask in order_book['asks'][:depth])
    if (total_bid + total_ask) == 0:
        return None
    return total_bid / (total_bid + total_ask)

def identify_support_resistance_from_order_book(order_book, depth=20):
    if not order_book.get('bids') or not order_book.get('asks'):
        return None, None
    bids = order_book['bids'][:depth]
    asks = order_book['asks'][:depth]
    support_level = max(bids, key=lambda x: float(x[1]))[0] if bids else None
    resistance_level = max(asks, key=lambda x: float(x[1]))[0] if asks else None
    if support_level is not None:
        support_level = float(support_level)
    if resistance_level is not None:
        resistance_level = float(resistance_level)
    return support_level, resistance_level

def calculate_order_book_indicators(order_book, suffix):
    order_book_indicators = {}
    order_book_indicators[f'bidask_spread{suffix}'] = calculate_bid_ask_spread(order_book)
    order_book_indicators[f'buy_sell_ratio{suffix}'] = calculate_buy_sell_ratio(order_book)
    order_book_indicators[f'order_flow_imbalance{suffix}'] = calculate_order_flow_imbalance(order_book)
    order_book_indicators[f'cumulative_volume_delta{suffix}'] = calculate_cvd([order_book])
    order_book_indicators[f'price_impact{suffix}'] = calculate_price_impact(order_book)

    bid_depth, ask_depth = calculate_market_depth(order_book)
    order_book_indicators[f'top_of_book_depth{suffix}'] = {'bid_depth': bid_depth, 'ask_depth': ask_depth}

    order_book_indicators[f'liquidity_imbalance{suffix}'] = calculate_liquidity_imbalance(order_book)
    order_book_indicators[f'vwap{suffix}'] = calculate_vwap(order_book)
    order_book_indicators[f'order_book_pressure{suffix}'] = calculate_order_book_pressure(order_book)

    support, resistance = identify_support_resistance_from_order_book(order_book)
    order_book_indicators[f'support{suffix}'] = support
    order_book_indicators[f'resistance{suffix}'] = resistance

    order_book_slope = calculate_order_book_slope(order_book)
    order_book_indicators[f'order_book_slope{suffix}'] = order_book_slope

    order_book_indicators[f'market_depth{suffix}'] = order_book
    df = pd.DataFrame([order_book_indicators])
    return df

def calculate_indicators_for_interval(df, interval):
    suffixes = ['', '2', '3']
    for suffix in suffixes:
        close_col = f'close_price{suffix}{interval}'
        high_col  = f'high_price{suffix}{interval}'
        low_col   = f'low_price{suffix}{interval}'

        if close_col in df.columns and df[close_col].notnull().any():
            rsi_col = f'RSI{suffix}{interval}'
            df[rsi_col] = calculate_rsi(df[close_col])

            macd_col       = f'MACD{suffix}{interval}'
            macd_sig_col   = f'MACD_Signal{suffix}{interval}'
            macd_hist_col  = f'MACD_Hist{suffix}{interval}'
            macd_line, sig_line, hist = calculate_macd(df[close_col])
            df[macd_col]      = macd_line
            df[macd_sig_col]  = sig_line
            df[macd_hist_col] = hist

            sma_col = f'SMA{suffix}{interval}'
            df[sma_col] = calculate_sma(df[close_col])

            bb = calculate_bollinger_bands(df[close_col], period=20, num_std_dev=2)
            df[f'Bollinger_Middle{suffix}{interval}'] = bb['Middle']
            df[f'Bollinger_Upper{suffix}{interval}']  = bb['Upper']
            df[f'Bollinger_Lower{suffix}{interval}']  = bb['Lower']

            k_col = f'Stochastic_Oscillator_K{suffix}{interval}'
            d_col = f'Stochastic_Oscillator_D{suffix}{interval}'
            percent_k, percent_d = calculate_stochastic_oscillator(df, high_col, low_col, close_col)
            df[k_col] = percent_k
            df[d_col] = percent_d

            ema_dict = calculate_ema(df[close_col])
            for ema_key, ema_series in ema_dict.items():
                df[f'{ema_key}{suffix}{interval}'] = ema_series

            df = calculate_ichimoku(df, high_col, low_col, close_col, suffix=f'{suffix}{interval}')

            atr_values = calculate_atr_values(df[high_col], df[low_col], df[close_col])
            for atr_key, atr_series in atr_values.items():
                df[f'{atr_key}{suffix}{interval}'] = atr_series

    return df

##############################################################################
# Higher-interval aggregator: "open" falls back to last candle's "close"
##############################################################################
def aggregate_interval_candles(df, interval_min, suffix=""):
    # Defensive: bail out if df is None or empty
    if df is None:
        print(f"[{suffix}] aggregate_interval_candles: df is None, skipping.")
        return pd.DataFrame()
    if len(df) == 0:
        print(f"[{suffix}] aggregate_interval_candles: df is empty, skipping.")
        return pd.DataFrame()
    if df.empty:
        return pd.DataFrame()

    open_col = 'open_price' if suffix == "" else f'open_price{suffix}'
    high_col = 'high_price' if suffix == "" else f'high_price{suffix}'
    low_col = 'low_price' if suffix == "" else f'low_price{suffix}'
    close_col = 'close_price' if suffix == "" else f'close_price{suffix}'
    vol_col = 'volume' if suffix == "" else f'volume{suffix}'

    df = df.copy()
    df = df.sort_values('timestamp').reset_index(drop=True)
    freq_str = f'{interval_min}T'
    interval_name = f'M{interval_min}'

    df[f'{interval_name}_start'] = df['timestamp'].dt.floor(freq_str)

    results = []
    for idx, row in df.iterrows():
        window_start = row[f'{interval_name}_start']
        ts = row['timestamp']
        window = df[(df['timestamp'] >= window_start) & (df['timestamp'] <= ts)]
        if window.empty:
            continue
        open_val = window.iloc[0][open_col]
        close_val = window.iloc[-1][close_col]
        high_val = window[high_col].max()
        low_val = window[low_col].min()
        vol_val = window[vol_col].sum()
        results.append({
            'timestamp': ts,
            f'open_price{suffix}{interval_name}': open_val,
            f'high_price{suffix}{interval_name}': high_val,
            f'low_price{suffix}{interval_name}': low_val,
            f'close_price{suffix}{interval_name}': close_val,
            f'volume{suffix}{interval_name}': vol_val
        })
    return pd.DataFrame(results)

def fill_open_with_min_age(interval_df, interval_val, suffix="", min_age_minutes=2):
    col_name = f"open_price{suffix}M{interval_val}"
    df = interval_df.sort_values("timestamp").reset_index(drop=True)
    for i, row in df.iterrows():
        if pd.isnull(row[col_name]):
            current_ts = row["timestamp"]
            found = None
            for j in range(i - 1, -1, -1):
                prev_row = df.iloc[j]
                if (current_ts - prev_row["timestamp"]) >= timedelta(minutes=min_age_minutes):
                    if pd.notnull(prev_row[col_name]):
                        found = prev_row[col_name]
                        break
            df.at[i, col_name] = found
    return df

def parse_json_safe(obj):
    if isinstance(obj, str) and obj.strip().startswith("{"):
        try:
            return json.loads(obj)
        except Exception:
            return None
    return obj

def vectorize_order_book_indicators(df, suffixes=["", "2", "3"]):
    for suffix in suffixes:
        ob_col = "order_book" + suffix if suffix else "order_book"
        if ob_col in df.columns:
            # Parse JSONs to dicts in one go
            df[ob_col] = df[ob_col].apply(parse_json_safe)
            # Calculate indicators and expand into columns
            indicators = df[ob_col].apply(lambda ob: calculate_order_book_indicators(ob, suffix) if ob else pd.DataFrame([{}]))
            indicators_df = pd.concat(indicators.tolist(), axis=0).reset_index(drop=True)
            for col in indicators_df.columns:
                df[col] = indicators_df[col].values
    return df


def process_asset_combined(asset):
    try:
        logging.info(f"[{asset}] Starting process_asset_combined.")

        # (1) Pull master data for the last 2 hours.
        master_query = f"""
                   SELECT *
                   FROM `{asset}`
                   WHERE timestamp >= DATE_SUB(NOW(), INTERVAL 2 HOUR)
                   ORDER BY timestamp ASC
               """
        t1 = time.time()
        master_df = pd.read_sql(master_query, dragnet_engine)
        print(f"[{asset}] DB READ: {time.time() - t1:.2f}s, shape={master_df.shape}")
        if master_df.empty:
            logging.info(f"[{asset}] No master data found.")
            return
        if 'timestamp' not in master_df.columns:
            logging.error(f"[{asset}] 'timestamp' column missing! Columns: {master_df.columns.tolist()}")
            return

        master_df['timestamp'] = pd.to_datetime(master_df['timestamp'])
        master_df.sort_values("timestamp", inplace=True)
        master_df.reset_index(drop=True, inplace=True)
        master_df.dropna(axis=1, how="all", inplace=True)

        # (2) Vectorized sub-minute → 1-minute aggregation
        agg_df = vectorized_1min_aggregation(master_df)
        # after groupby, we ensure it's sorted
        agg_df.sort_values("timestamp", inplace=True, ignore_index=True)

        agg_df = vectorize_order_book_indicators(agg_df)
        rep_df = agg_df  # rep_df is now just agg_df, already augmented

         #(4) 1-minute rolling indicators
        merged_df = rep_df.copy()
        if not merged_df.empty:
            merged_df = calculate_indicators_for_interval(merged_df, interval="")
            # Optionally keep last N rows
            merged_df = merged_df.tail(3)
        upsert_dragnet_row(dragnet_engine, merged_df, asset)
        logging.info(f"[{asset}] 1‑minute data processed and upserted.")

        # (5) Process higher intervals
        all_cols = set(rep_df.columns)
        possible_suffixes = ["", "2", "3"]
        valid_suffixes = []
        for s in possible_suffixes:
            need = []
            for base in ["open_price", "high_price", "low_price", "close_price", "volume"]:
                col = base + s if s else base
                if col in all_cols:
                    need.append(col)
            # if we found all 5, we keep the suffix
            # (strict check: if we want exactly all 5, do a length check)
            if len(need) == 5:
                valid_suffixes.append(s)

        for interval_val in [5, 15, 60]:
            logging.info(f"[{asset}] Processing interval: {interval_val} minutes")

            agg_interval_dfs = []
            for s in valid_suffixes:
                df_interval = aggregate_interval_candles(rep_df, interval_val, suffix=s)
                if not df_interval.empty:
                    agg_interval_dfs.append(df_interval)

            if agg_interval_dfs:
                interval_df = agg_interval_dfs[0]
                for other_df in agg_interval_dfs[1:]:
                    interval_df = pd.merge(interval_df, other_df, on="timestamp", how="outer")
            else:
                interval_df = pd.DataFrame()

            for s in valid_suffixes:
                interval_df = fill_open_with_min_age(interval_df, interval_val, suffix=s, min_age_minutes=2)

            interval_df = calculate_indicators_for_interval(interval_df, interval=f'M{interval_val}')
            upsert_dragnet_row(dragnet_engine, interval_df, asset)
            set_heartbeat(control_engine, curator_id)
            logging.info(f"[{asset}] {interval_val}-minute data processed and upserted.")

        # (6) Delete ephemeral sub-minute data older than 2 minutes.
        del_query = text(f"""
            DELETE FROM dragnet.`{asset}`
            WHERE timestamp < (NOW() - INTERVAL 2 MINUTE)
              AND close_price IS NULL
              AND close_price2 IS NULL
              AND close_price3 IS NULL
        """)
        with dragnet_engine.begin() as conn:
            conn.execute(del_query)

        # (7) Mark older rows as Curated
        current_time = pd.Timestamp.now()
        timestamps_to_curate = rep_df.loc[
            rep_df['timestamp'] < (current_time - pd.Timedelta(minutes=2)), 'timestamp'
        ].tolist()
        if timestamps_to_curate:
            ts_list = ', '.join([f"'{str(ts)}'" for ts in timestamps_to_curate])
            update_query = text(f"""
                UPDATE dragnet.`{asset}`
                SET Curated = 1
                WHERE timestamp IN ({ts_list})
            """)
            with dragnet_engine.begin() as conn:
                conn.execute(update_query)

        logging.info(f"[{asset}] Combined processing complete. Curated older rows.")
    except Exception as e:
        log_module_event(control_engine, 'curator', curator_id, node_ip, 'error', str(e))
        raise

def chunked(iterable, n):
    """Yield successive n-sized chunks from iterable."""
    for i in range(0, len(iterable), n):
        yield iterable[i:i + n]

def main():
    while True:
        for asset_batch in chunked(ASSETS, batch_size):
            with concurrent.futures.ThreadPoolExecutor(max_workers=workers) as executor:
                futures = {executor.submit(process_with_lock, asset): asset for asset in asset_batch}
                for future in concurrent.futures.as_completed(futures):
                    asset_name = futures[future]
                    try:
                        future.result()
                    except Exception as e:
                        log_module_event(control_engine, 'curator', curator_id, node_ip, 'error', str(e))
                        raise
            # Brief sleep between batches helps OS/gc clean up
            time.sleep(0.1)
        # Optionally sleep before starting another full cycle
        time.sleep(2)

def set_module_status(control_engine, curator_id, status):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            UPDATE dragnetcontrol.curator_modules
            SET status = :status, last_heartbeat = :last_heartbeat
            WHERE curator_id = :curator_id
        """)
        conn.execute(query, {
            'curator_id': curator_id,
            'status': status,
            'last_heartbeat': now
        })
        conn.commit()

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        # Set status to "Stopped" on fatal error
        set_module_status(control_engine, curator_id, "Stopped")
        log_module_event(control_engine, 'curator', curator_id, node_ip, 'error', f"CRASH: {str(e)}")
        raise  # Let it crash visibly after setting status