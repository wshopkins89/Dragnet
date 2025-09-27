import sys
import time
from datetime import datetime, timedelta, timezone
import json
from sqlalchemy import create_engine, text, inspect, Table, MetaData
import pytz
import requests
from DragnetUtilities import create_table, check_if_table_exists, insert_crypto_data_retro, aggregate_interval_candles_asof, calculate_indicators_for_interval
import socket
import logging
import pandas as pd
import math

COOLDOWN_PERIOD = 60
MAX_RETRIES = 3
current_position = 0
blacklist = {}

def log_module_event(control_engine, module_type, module_id, module_ip, log_level, message):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.module_logs (module_type, module_id, module_ip, timestamp, log_level, message)
            VALUES (:module_type, :module_id, :module_ip, :timestamp, :log_level, :message)
        """)
        conn.execute(query, {
            'module_type': module_type,    # e.g. 'scanner', 'scanner', etc.
            'module_id': module_id,        # usually scanner_id or similar
            'module_ip': module_ip,            # if you track nodes, else pass None or 0
            'timestamp': now,
            'log_level': log_level,        # 'info', 'warning', 'error'
            'message': message
        })
        conn.commit()

if len(sys.argv) < 15:
    print("Data pass incomplete. Values missing or in error")
    print("Arguments received:", sys.argv)
    time.sleep(10)
    sys.exit(1)
'''
scanner_id = 'testretroscanner'
dragnet_db_host = 'localhost'
dragnet_db_username = 'dragnet'
dragnet_db_password = 'dragnet5'
control_db_host = 'localhost'
control_db_user = 'dragnet'
control_db_pass = 'dragnet5'
assets_db_host = 'localhost'
assets_db_username = 'dragnet'
assets_db_pass = 'dragnet5'
start_asset = 'bitcoin'
end_asset = 'ether'
start_date = datetime.strptime('08042025', "%m%d%Y")
end_date = datetime.strptime('01012001', "%m%d%Y")
delay = .1

'''
scanner_id = sys.argv[1]
dragnet_db_host = sys.argv[2]
dragnet_db_username = sys.argv[3]
dragnet_db_password = sys.argv[4]
control_db_host = sys.argv[5]
control_db_user = sys.argv[6]
control_db_pass = sys.argv[7]
assets_db_host = sys.argv[8]
assets_db_username = sys.argv[9]
assets_db_pass = sys.argv[10]
start_asset = sys.argv[11]
end_asset = sys.argv[12]
start_date = datetime.strptime(sys.argv[13], "%m%d%Y")
end_date = datetime.strptime(sys.argv[14], "%m%d%Y")
delay = float(sys.argv[15])

engine_assets = f'mysql+mysqldb://{assets_db_username}:{assets_db_pass}@{assets_db_host}/assets'
assets_engine = create_engine(engine_assets, pool_size=5, max_overflow=2, pool_recycle=1800, pool_pre_ping=True, future=True)
engine_control = f'mysql+mysqldb://{control_db_user}:{control_db_pass}@{control_db_host}'
control_engine = create_engine(engine_control, pool_size=5, max_overflow=2, pool_recycle=1800, pool_pre_ping=True, future=True)
engine_dragnet = f'mysql+mysqldb://{dragnet_db_username}:{dragnet_db_password}@{dragnet_db_host}/dragnet'
dragnet_engine = create_engine(engine_dragnet, pool_size=5, max_overflow=2, pool_recycle=1800, pool_pre_ping=True, future=True)
inspector = inspect(dragnet_engine)

node_ip = 'error'
try:
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.connect(('8.8.8.8', 80))  # Google's DNS; not actually contacted
    node_ip = s.getsockname()[0]
    s.close()
except Exception as e:
    log_module_event(control_engine, 'scanner', scanner_id, node_ip, 'error', str(e))
    raise

now = datetime.now()

try:
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.scanner_modules
                (scanner_id, node_ip, asset_range_start, asset_range_end, status, created_at, last_heartbeat)
            VALUES
                (:scanner_id, :node_ip, :asset_range_start, :asset_range_end, :status, :created_at, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                scanner_id = VALUES(scanner_id),
                node_ip = VALUES(node_ip),
                asset_range_start = VALUES(asset_range_start),
                asset_range_end = VALUES(asset_range_end),
                status = "Running",
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'scanner_id': str(scanner_id),
            'node_ip': node_ip,
            'asset_range_start': start_asset,
            'asset_range_end': end_asset,
            'status': ("Running"),
            'created_at': now,
            'last_heartbeat': now
        })
        conn.commit()
except Exception as e:
    log_module_event(control_engine, 'retroscanner', scanner_id, node_ip, 'error', str(e))
    raise

def set_heartbeat(control_engine, scanner_id):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.scanner_modules (scanner_id, last_heartbeat)
            VALUES (:scanner_id, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'scanner_id': scanner_id,
            'last_heartbeat': now
        })
        conn.commit()

with assets_engine.connect() as conn:
    all_assets = sorted([row[0].lower() for row in conn.execute(text("SELECT name FROM crypto WHERE type='crypto'"))])

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
                'worker': scanner_id,
                'expiration': expiration,
                'now': now
            })
            result = conn.execute(select_query, {'asset': asset}).scalar()
    return result == scanner_id

def round_down_to_minute(dt):
    return dt.replace(second=0, microsecond=0)

def release_lock(asset):
    query = text("""
        DELETE FROM dragnetcontrol.dragnet_locks
        WHERE asset_name = :asset AND locked_by = :worker
    """)
    with control_engine.connect() as conn:
        with conn.begin():
            conn.execute(query, {'asset': asset, 'worker': scanner_id})

def set_module_status(control_engine, scanner_id, status):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            UPDATE dragnetcontrol.scanner_modules
            SET status = :status, last_heartbeat = :last_heartbeat
            WHERE scanner_id = :scanner_id
        """)
        conn.execute(query, {
            'scanner_id': scanner_id,
            'status': status,
            'last_heartbeat': now
        })
        conn.commit()

print(f"Backfilling from {start_date} to {end_date}")

def dragnet_col(base, suffix, interval_label):
    """
    Returns standardized Dragnet column names:
    - For 1-minute (interval_label=""), just add suffix if present (e.g. RSI2, close_price3)
    - For 5min/15min/etc, add both (e.g. RSI3M15, close_price2M60)
    """
    if interval_label == "":
        return f"{base}{suffix}"
    return f"{base}{suffix}{interval_label}"

# --- Time Conversion ---
def convert_unix_to_central(unix_ts):
    utc_time = datetime.utcfromtimestamp(unix_ts).replace(tzinfo=pytz.utc)
    central_time = utc_time.astimezone(pytz.timezone("America/Chicago"))
    return central_time.timestamp()

def convert_pacific_to_central(unix_ts):
    pacific_time = datetime.fromtimestamp(unix_ts, pytz.timezone("America/Los_Angeles"))
    central_time = pacific_time.astimezone(pytz.timezone("America/Chicago"))
    return central_time.timestamp()

# --- API Fetchers ---
def fetch_coinbase_data(ticker, start, end):
    headers = {"User-Agent": "Mozilla/5.0"}
    url = f"https://api.exchange.coinbase.com/products/{ticker}/candles"
    params = {
        "granularity": 60,
        "start": start.isoformat(),
        "end": end.isoformat()
    }
    response = requests.get(url, headers=headers, params=params)
    data = response.json()
    result = []
    if isinstance(data, list):
        for item in data:
            ts = int(item[0])  # Again, RAW UNIX TIMESTAMP
            result.append({
                "timestamp": ts,
                "low_price": item[1],
                "high_price": item[2],
                "open_price": item[3],
                "close_price": item[4],
                "volume": item[5]
            })
    return result

def fetch_kraken_data(pair, since, until):
    """
    Fetches all trades for a Kraken pair between since and until (UNIX seconds, inclusive).
    Returns a flat list of trades (each: [price, volume, time, side, ordertype, misc]).
    """
    all_trades = []
    next_since = since
    while True:
        url = f"https://api.kraken.com/0/public/Trades?pair={pair}&since={next_since}"
        response = requests.get(url)
        data = response.json()
        if "result" not in data:
            break
        pair_keys = [k for k in data["result"] if k != "last"]
        if not pair_keys:
            break
        trades = data["result"][pair_keys[0]]
        # Only keep trades in our desired window
        filtered = [trade for trade in trades if since <= float(trade[2]) < until]
        if filtered:
            all_trades.extend(filtered)
        # Stop if we've gone past our window or there are no trades
        if not trades or (trades and float(trades[-1][2]) >= until):
            break
        next_since = data["result"]["last"]
    return all_trades


def fetch_binance_data(ticker, start, end):
    interval = "1m"
    start_ms = int(start.timestamp()) * 1000
    url = f"https://api.binance.us/api/v3/klines?symbol={ticker}&interval={interval}&startTime={start_ms}"
    response = requests.get(url, headers={"User-Agent": "Mozilla/5.0"})
    data = response.json()
    result = []
    if isinstance(data, list):
        for item in data:
            ts = int(item[0] // 1000)  # UNIX UTC TIMESTAMP, as in functional code
            result.append({
                "timestamp": ts,
                "open_price3": float(item[1]),
                "high_price3": float(item[2]),
                "low_price3": float(item[3]),
                "close_price3": float(item[4]),
                "volume3": float(item[5])
            })
    return result


def fetch_data_with_retry(fetch_fn, name, *args):
    if name in blacklist:
        elapsed = time.time() - blacklist[name]
        if elapsed < COOLDOWN_PERIOD:
            print(f"{name} blacklisted. Skipping...")
            return []
        else:
            del blacklist[name]

    for attempt in range(MAX_RETRIES):
        try:
            return fetch_fn(*args)
        except Exception as e:
            print(f"[{name}] Attempt {attempt+1} failed: {e}")
            time.sleep(2)

    print(f"{name} failed after {MAX_RETRIES} attempts. Blacklisting.")
    blacklist[name] = time.time()
    return []

def fetch_asset_exchange_metadata(start_asset, end_asset):
    with assets_engine.connect() as conn:
        rows = conn.execute(text("""
            SELECT name, coinbase_status, coinbaseticker, coinbasetradepairs,
                   kraken_status, krakenticker, krakentradepairs,
                   binance_status, binanceticker, binancetradepairs
            FROM crypto WHERE type = "crypto"
        """)).fetchall()

        # Build a list of all asset names, sorted alphabetically
        all_assets = sorted(rows, key=lambda r: r[0].lower())

        names = [row[0].lower() for row in all_assets]
        if start_asset and end_asset:
            start = start_asset.lower()
            end = end_asset.lower()
            try:
                start_idx = names.index(start)
                end_idx = names.index(end)
            except ValueError as e:
                raise Exception(f"Asset not found in list: {e}")

            # Make sure indices are in the right order
            if start_idx > end_idx:
                start_idx, end_idx = end_idx, start_idx

            filtered = all_assets[start_idx:end_idx + 1]
            return filtered

        elif start_asset:
            start = start_asset.lower()
            filtered = [row for row in all_assets if row[0].lower() == start]
            return filtered

        elif end_asset:
            end = end_asset.lower()
            filtered = [row for row in all_assets if row[0].lower() == end]
            return filtered

        else:
            return all_assets

def trades_to_candles(trades, interval_seconds=60):
    if not trades:
        return []
    trades = [trade[:6] for trade in trades]
    df = pd.DataFrame(trades, columns=['price', 'volume', 'timestamp', 'side', 'order_type', 'misc'])
    df['price'] = pd.to_numeric(df['price'], errors='coerce')
    df['volume'] = pd.to_numeric(df['volume'], errors='coerce')
    df['datetime'] = pd.to_datetime(df['timestamp'], unit='s')
    df = df.set_index('datetime')
    ohlc = df['price'].resample(f'{interval_seconds}s').ohlc()
    vol = df['volume'].resample(f'{interval_seconds}s').sum()
    ohlc['volume'] = vol
    ohlc = ohlc.reset_index()
    result = []
    for _, row in ohlc.iterrows():
        if pd.isna(row['open']):
            continue  # skip empty candles
        ts = int(row['datetime'].timestamp())
        result.append({
            'timestamp': ts,
            'open_price2': row['open'],
            'high_price2': row['high'],
            'low_price2': row['low'],
            'close_price2': row['close'],
            'volume2': row['volume']
        })
    return result

def safe_row_for_db(row):
    # row is a dict (from to_dict) or a pandas Series
    out = {}
    for k, v in (row.items() if isinstance(row, dict) else row.to_dict().items()):
        out[k] = None if pd.isna(v) else v
    return out

assets = fetch_asset_exchange_metadata(start_asset, end_asset)
def unix_to_central_mysql_datetime(ts):
    """Convert UTC unix timestamp to US Central MySQL DATETIME string."""
    central = pytz.timezone('US/Central')
    dt_central = datetime.fromtimestamp(ts, pytz.utc).astimezone(central)
    return dt_central.strftime('%Y-%m-%d %H:%M:%S')

def fill_synthetic_candles(merged, start_ts, end_ts, price_fields_prefixes, skip_existing=True):
    all_minutes = list(range(start_ts, end_ts+1, 60))
    # For each prefix, track the last known close *before* the window
    prev_close = {prefix: None for prefix in price_fields_prefixes}
    sorted_keys = sorted(merged.keys())

    # Scan to get last close before start_ts for each prefix
    for prefix in price_fields_prefixes:
        prior_candles = [ts for ts in sorted_keys if ts < start_ts and f'close_price{prefix}' in merged[ts]]
        if prior_candles:
            last_ts = prior_candles[-1]
            prev_close[prefix] = merged[last_ts][f'close_price{prefix}']
        else:
            # Try first available in window (for edge cases)
            window_candles = [ts for ts in sorted_keys if f'close_price{prefix}' in merged[ts]]
            if window_candles:
                prev_close[prefix] = merged[window_candles[0]][f'close_price{prefix}']

    for ts in all_minutes:
        for prefix in price_fields_prefixes:
            if skip_existing and ts in merged and f'close_price{prefix}' in merged[ts]:
                # Keep updating prev_close for later synthetic fills
                prev_close[prefix] = merged[ts][f'close_price{prefix}']
                continue
            if prev_close[prefix] is None:
                continue  # Can't fill if no real previous value
            # Fill synthetic
            merged.setdefault(ts, {})['timestamp'] = ts
            merged[ts][f'open_price{prefix}'] = prev_close[prefix]
            merged[ts][f'high_price{prefix}'] = prev_close[prefix]
            merged[ts][f'low_price{prefix}'] = prev_close[prefix]
            merged[ts][f'close_price{prefix}'] = prev_close[prefix]
            merged[ts][f'volume{prefix}'] = 0.0

        # After filling (or not), update prev_close if we just landed a real candle in this slot
        for prefix in price_fields_prefixes:
            if ts in merged and f'close_price{prefix}' in merged[ts]:
                prev_close[prefix] = merged[ts][f'close_price{prefix}']

    return merged

json_cols_to_exclude = [
    'Fib100%'
    'Fib78.6%'
    'Fib61.8%'
    'Fib50%'
    'Fib38.2%'
    'Fib23.6%'
    'Fib0%'
    'orderbook',
    'top_of_book_depth',
    'market_depth',
    'order_book_slope',
    'ticker_data_json',
    'best_bid_ask_json'
    'orderbook2',
    'top_of_book_depth2',
    'market_depth2',
    'order_book_slope2',
    'ticker_data_json2',
    'best_bid_ask_json2'
    'orderbook3',
    'top_of_book_depth3',
    'market_depth3',
    'order_book_slope3',
    'ticker_data_json3',
    'best_bid_ask_json3'
    'price_action'
    'VIPTrades'
]

desired_columns = [
    "timestamp",
    "close_price", "open_price", "high_price", "low_price", "volume",
    "close_priceM5", "open_priceM5", "high_priceM5", "low_priceM5", "volumeM5",
    "close_priceM15", "open_priceM15", "high_priceM15", "low_priceM15", "volumeM15",
    "close_priceM60", "open_priceM60", "high_priceM60", "low_priceM60", "volumeM60",
    "close_priceM1440", "open_priceM1440", "high_priceM1440", "low_priceM1440", "volumeM1440",
    "close_price2", "open_price2", "high_price2", "low_price2", "volume2",
    "close_price2M5", "open_price2M5", "high_price2M5", "low_price2M5", "volume2M5",
    "close_price2M15", "open_price2M15", "high_price2M15", "low_price2M15", "volume2M15",
    "close_price2M60", "open_price2M60", "high_price2M60", "low_price2M60", "volume2M60",
    "close_price2M1440", "open_price2M1440", "high_price2M1440", "low_price2M1440", "volume2M1440",
    "close_price3", "open_price3", "high_price3", "low_price3", "volume3",
    "close_price3M5", "open_price3M5", "high_price3M5", "low_price3M5", "volume3M5",
    "close_price3M15", "open_price3M15", "high_price3M15", "low_price3M15", "volume3M15",
    "close_price3M60", "open_price3M60", "high_price3M60", "low_price3M60", "volume3M60",
    "close_price3M1440", "open_price3M1440", "high_price3M1440", "low_price3M1440", "volume3M1440"

]
select_cols = ', '.join(desired_columns)


MAX_LOOKBACK = 1200   # Should be at least the largest indicator window you use (e.g. EMA200)
BLOCK_SIZE = 300     # Number of minutes per processing chunk (tune for RAM/speed)
intervals = [1, 5, 15, 60, 1440]
suffixes = ['', '2', '3']

while True:
    try:
        all_assets = fetch_asset_exchange_metadata(start_asset, end_asset)

        for row in all_assets:
            (
                base_asset_name, cb_status, cb_ticker, cb_tradepairs_json,
                kr_status, kr_ticker, kr_tradepairs_json,
                bn_status, bn_ticker, bn_tradepairs_json
            ) = row

            table_name = base_asset_name.lower()
            if not acquire_lock(table_name):
                print(f"[{table_name}] Skipped (locked by another worker)")
                continue

            try:
                if not check_if_table_exists(dragnet_engine, table_name):
                    ticker_table = create_table(dragnet_engine, table_name)
                else:
                    metadata = MetaData()
                    ticker_table = Table(table_name, metadata, autoload_with=dragnet_engine)

                cb_tradepairs = json.loads(cb_tradepairs_json) if cb_tradepairs_json else []
                kr_tradepairs = json.loads(kr_tradepairs_json) if kr_tradepairs_json else []
                bn_tradepairs = json.loads(bn_tradepairs_json) if bn_tradepairs_json else []

                # -- Ensure all dates are UTC --
                current = start_date.replace(tzinfo=pytz.utc)
                end_date_utc = end_date.replace(tzinfo=pytz.utc)

                while current > end_date_utc:
                    # ----- define window edges -----
                    block_end = current
                    block_start = current - timedelta(minutes=BLOCK_SIZE)
                    next_ = max(end_date_utc, current - timedelta(minutes=BLOCK_SIZE))

                    # ---------- RAW FETCH ----------
                    merged = {}
                    coinbase_earliest = int(current.timestamp())
                    kraken_earliest = int(current.timestamp())

                    for quote in set(cb_tradepairs + kr_tradepairs + bn_tradepairs):
                        cb_pair = f"{cb_ticker}-{quote}" if cb_ticker else None
                        kr_pair = f"{kr_ticker}{quote}" if kr_ticker else None
                        bn_pair = f"{bn_ticker}{quote}" if bn_ticker else None

                        # Coinbase (1m)
                        if cb_status == "active" and cb_ticker and quote in cb_tradepairs:
                            try:
                                cb_data = fetch_coinbase_data(cb_pair, next_, current)
                                if cb_data:
                                    for r in cb_data:
                                        ts = int(r['timestamp'])
                                        merged.setdefault(ts, {'timestamp': ts}).update(
                                            {k: v for k, v in r.items() if k != 'timestamp'})
                                    coinbase_earliest = min(coinbase_earliest, min(r['timestamp'] for r in cb_data))
                            except Exception as e:
                                print(f"[{table_name}] CB fetch err: {e}")

                        # Kraken (trades -> 1m)
                        if kr_status == "active" and kr_ticker and quote in kr_tradepairs:
                            try:
                                ks, ke = int(coinbase_earliest), int(current.timestamp())
                                trades = fetch_kraken_data(kr_pair, ks, ke)
                                if trades:
                                    kr_candles = trades_to_candles(trades)
                                    for r in kr_candles:
                                        ts = int(r['timestamp'])
                                        merged.setdefault(ts, {'timestamp': ts}).update(
                                            {k: v for k, v in r.items() if k != 'timestamp'})
                                    kraken_earliest = min(kraken_earliest, min(r['timestamp'] for r in kr_candles))
                            except Exception as e:
                                print(f"[{table_name}] Kraken fetch err: {e}")

                        # Binance (1m) -> suffix 3
                        if bn_status == "active" and bn_ticker and quote in bn_tradepairs:
                            try:
                                bn_data = fetch_binance_data(bn_pair, next_, current)
                                for r in bn_data:
                                    ts = int(r['timestamp'])
                                    merged.setdefault(ts, {'timestamp': ts}).update(
                                        {k: v for k, v in r.items() if k != 'timestamp'})
                            except Exception as e:
                                print(f"[{table_name}] Binance fetch err: {e}")

                    # ---------- SYNTHETIC FILL ----------
                    all_ts = sorted(merged.keys())
                    if not all_ts:
                        print(f"[{table_name}] No real candles found; skipping.")
                        current = next_
                        continue

                    min_ts, max_ts = all_ts[0], all_ts[-1]
                    merged = fill_synthetic_candles(
                        merged, min_ts, max_ts,
                        price_fields_prefixes=['', '2', '3'],  # fill all families
                        skip_existing=True
                    )

                    # ---------- BUILD DF & AS-OF AGGREGATES ----------
                    df = pd.DataFrame(list(merged.values()))
                    df['timestamp'] = pd.to_datetime(df['timestamp'], unit='s').dt.tz_localize(None)
                    df = df.sort_values('timestamp').reset_index(drop=True)


                    def families_present(df_):
                        fams = ['', '2', '3']
                        need = lambda s: [f'open_price{s}', f'high_price{s}', f'low_price{s}', f'close_price{s}',
                                          f'volume{s}']
                        return [s for s in fams if all(c in df_.columns for c in need(s))]


                    present = families_present(df)

                    # per-minute, as-the-candle-forms aggregates (M5/M15/M60/M1440)
                    for interval_min in (5, 15, 60, 1440):
                        for sfx in present:
                            rolled = aggregate_interval_candles_asof(df, interval_min, sfx)
                            if rolled.empty:
                                continue
                            rolled = rolled.set_index('timestamp')
                            new_cols = [c for c in rolled.columns if c not in df.columns]  # avoid _x/_y
                            if new_cols:
                                df = df.merge(rolled[new_cols], how='left', left_on='timestamp', right_index=True)

                    # ---------- FIRST WRITE (1m + as-of aggregates, no indicators) ----------
                    # FIRST WRITE (1m + as-of aggregates, no indicators)
                    # FIRST WRITE (1m + as-of aggregates, no indicators)
                    df_out = df.copy()
                    df_out['timestamp'] = df_out['timestamp'].dt.tz_localize('UTC').dt.tz_convert('US/Central').dt.strftime('%Y-%m-%d %H:%M:%S')

                    table_cols = set(c.name for c in ticker_table.columns)
                    batch = [{k: (None if pd.isna(v) else v) for k, v in r.items() if k in table_cols}
                             for r in df_out.to_dict(orient='records')]

                    print(f"Inserting {len(batch)} rows into {table_name} (1m + as-of M5/M15/M60/M1440)...")
                    insert_crypto_data_retro(dragnet_engine, ticker_table, batch)  # should be ON DUP KEY UPDATE
                    set_heartbeat(control_engine, scanner_id)
                    print("Inserted aggregates.")

                    # ---------- CALC PASS ----------
                    safe_start = next_
                    safe_end = current
                    calc_from = safe_start - timedelta(minutes=MAX_LOOKBACK)

                    central = pytz.timezone("US/Central")
                    from_str = calc_from.astimezone(central).strftime('%Y-%m-%d %H:%M:%S')
                    to_str = safe_end.astimezone(central).strftime('%Y-%m-%d %H:%M:%S')

                    with dragnet_engine.connect() as conn:
                        q = text(f"""
                            SELECT {select_cols} FROM `{table_name}`
                            WHERE timestamp >= :from_ AND timestamp < :to_
                            ORDER BY timestamp ASC
                        """)
                        df = pd.read_sql(q, conn, params={'from_': from_str, 'to_': to_str})

                    if df.empty:
                        print(f"[{table_name}] Nothing came back for calc window.")
                    else:
                        # timestamps from DB are Central-naive; keep them naive for comparisons
                        df['timestamp'] = pd.to_datetime(df['timestamp']).dt.tz_localize(None)
                        df = df.sort_values('timestamp').reset_index(drop=True)

                        present_families = families_present(df)

                        # ensure as-of aggregates exist in calc window (cheap to recompute)
                        for interval_min in (5, 15, 60, 1440):
                            for sfx in present_families:
                                rolled = aggregate_interval_candles_asof(df, interval_min, sfx)
                                if rolled.empty:
                                    continue
                                rolled = rolled.set_index('timestamp')
                                new_cols = [c for c in rolled.columns if c not in df.columns]
                                if new_cols:
                                    df = df.merge(rolled[new_cols], how='left', left_on='timestamp', right_index=True)

                            label = f'M{interval_min}'
                            df = calculate_indicators_for_interval(df, label)

                        # base 1m indicators
                        df = calculate_indicators_for_interval(df, '')

                        # --- tz-correct safe window mask (convert bounds to Central, then drop tz) ---
                        safe_start_c = safe_start.astimezone(central).replace(tzinfo=None)
                        safe_end_c = safe_end.astimezone(central).replace(tzinfo=None)
                        mask = (df['timestamp'] >= safe_start_c) & (df['timestamp'] < safe_end_c)
                        df_safe = df.loc[mask].copy()

                        if not df_safe.empty:
                            # CALC WRITE (safe window upsert)
                            df_safe['timestamp'] = df_safe['timestamp'] \
                                .dt.tz_localize('UTC') \
                                .dt.tz_convert('US/Central') \
                                .dt.strftime('%Y-%m-%d %H:%M:%S')

                            table_cols = set(c.name for c in ticker_table.columns)
                            payload = [{k: (None if pd.isna(v) else v) for k, v in r.items() if k in table_cols}
                                       for r in df_safe.to_dict(orient='records')]

                            print(f"Upserting {len(payload)} rows with indicators into {table_name} ...")
                            insert_crypto_data_retro(dragnet_engine, ticker_table, payload)  # ON DUP KEY UPDATE
                            set_heartbeat(control_engine, scanner_id)
                            print("Upserted calcs.")

                    # ---------- STEP WINDOW BACK ----------
                    step_to = min(coinbase_earliest, kraken_earliest)
                    current = next_ if step_to == int(current.timestamp()) else \
                        datetime.fromtimestamp(step_to, tz=timezone.utc) - timedelta(minutes=1)
                    time.sleep(float(delay) if delay else 1)

            except Exception as e:
                print(f"Error processing {table_name}: {e}")

            finally:
                release_lock(table_name)

        time.sleep(float(delay) if delay else 5)
    except Exception as e:
        set_module_status(control_engine, scanner_id, "Stopped")
        log_module_event(control_engine, 'scanner', scanner_id, node_ip, 'error', str(e))
        raise

print("Backfill complete.")