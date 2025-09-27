import os
import sys
import pymysql
from sqlalchemy import create_engine, text, Table, MetaData, Column, Integer, SmallInteger, Float, String, DateTime, JSON, inspect
from sqlalchemy.dialects.mysql import insert
from datetime import datetime, timedelta
import pytz
import requests
import time
import json
from sqlalchemy.exc import IntegrityError
import socket
import gc
import io
from DragnetUtilities import create_table, check_if_table_exists, insert_crypto_data
import logging

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stdout.reconfigure(line_buffering=True)

MAX_RETRIES = 3
COOLDOWN_PERIOD = 60
blacklist = {}

Obscanner_id = sys.argv[1]
dragnet_db_host = sys.argv[2]
dragnet_db_username = sys.argv[3]
dragnet_db_password = sys.argv[4]
control_db_host = sys.argv[5]
control_db_username = sys.argv[6]
control_db_pass = sys.argv[7]
assets_db_host = sys.argv[8]
assets_db_username = sys.argv[9]
assets_db_pass = sys.argv[10]
start_asset = sys.argv[11]
end_asset = sys.argv[12]
delay = sys.argv[13]
coinbase_api_key = sys.argv[14]
coinbase_api_secret = sys.argv[15]
coinbase_passphrase = sys.argv[16]
binance_us_api_key = sys.argv[17]
granularity = int(sys.argv[18])
hours = float(sys.argv[19])

"""Obscanner_id = 'testscanner'
dragnet_db_host = '192.168.1.210'
dragnet_db_username = 'dragnet'
dragnet_db_password = 'dragnet5'
control_db_host = '192.168.1.210'
control_db_username = 'dragnet'
control_db_pass = 'dragnet5'
assets_db_host = '192.168.1.210'
assets_db_username = 'dragnet'
assets_db_pass = 'dragnet5'
start_asset = '00 token'
end_asset = 'zora'
delay = .444
coinbase_api_key = 'f543e74079ddd719989ed24a29019f02'
coinbase_api_secret = 'LR5Ql2/A6CTyUqtbecJ8IFcsuGbo8WftVtY+VKkHdrcShzLou5cGwB4t6Q7xd3Y6AkYVau0m6+tQdrtqEjXBtQ=='
coinbase_passphrase = '3bnlix8qyh'
binance_us_api_key = '3apj9lJKK5lnUUrJ0gZWUsCgNzf4MUl70e4gVJEh32ReR3ziiVXhZhU46xdx2rdk'
granularity = 60
hours = .25"""

engine_assets = f'mysql+mysqldb://{assets_db_username}:{assets_db_pass}@{assets_db_host}/assets'
assets_engine = create_engine(engine_assets)
engine_control = f'mysql+mysqldb://{control_db_username}:{control_db_pass}@{control_db_host}'
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
    log_module_event(control_engine, 'scanner', Obscanner_id, node_ip, 'error', str(e))
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
            'module_id': module_id,# e.g. 'scanner', 'scanner', etc.      # usually scanner_id or similar            # if you track nodes, else pass None or 0
            'log_level': log_level,        # 'info', 'warning', 'error'
            'message': message
        })
        conn.commit()
try:
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.orderbook_modules
                (Obscanner_id, node_ip, asset_range_start, asset_range_end, status, created_at, last_heartbeat)
            VALUES
                (:Obscanner_id, :node_ip, :asset_range_start, :asset_range_end, :status, :created_at, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                Obscanner_id = VALUES(Obscanner_id),
                node_ip = VALUES(node_ip),
                asset_range_start = VALUES(asset_range_start),
                asset_range_end = VALUES(asset_range_end),
                status = "Running",
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'Obscanner_id': str(Obscanner_id),
            'node_ip': node_ip,
            'asset_range_start': start_asset,
            'asset_range_end': end_asset,
            'status': ("Running"),
            'created_at': now,
            'last_heartbeat': now
        })
        conn.commit()
except Exception as e:
    log_module_event(control_engine, 'scanner', Obscanner_id, node_ip, 'error', str(e))
    raise

def set_heartbeat(control_engine, Obscanner_id):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.orderbook_modules (Obscanner_id, last_heartbeat)
            VALUES (:Obscanner_id, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'Obscanner_id': Obscanner_id,
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

def set_module_status(control_engine, Obscanner_id, status):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            UPDATE dragnetcontrol.orderbook_modules
            SET status = :status, last_heartbeat = :last_heartbeat
            WHERE Obscanner_id = :Obscanner_id
        """)
        conn.execute(query, {
            'Obscanner_id': Obscanner_id,
            'status': status,
            'last_heartbeat': now
        })
        conn.commit()

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
                'worker': Obscanner_id,
                'expiration': expiration,
                'now': now
            })
            result = conn.execute(select_query, {'asset': asset}).scalar()
    return result == Obscanner_id

def release_lock(asset):
    query = text("""
        DELETE FROM dragnetcontrol.dragnet_locks
        WHERE asset_name = :asset AND locked_by = :worker
    """)
    with control_engine.connect() as conn:
        with conn.begin():
            conn.execute(query, {'asset': asset, 'worker': Obscanner_id})

def set_module_status(control_engine, Obscanner_id, status):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            UPDATE dragnetcontrol.orderbook_modules
            SET status = :status, last_heartbeat = :last_heartbeat
            WHERE Obscanner_id = :Obscanner_id
        """)
        conn.execute(query, {
            'Obscanner_id': Obscanner_id,
            'status': status,
            'last_heartbeat': now
        })
        conn.commit()

def load_table(engine, table_name):
    metadata = MetaData()
    metadata.reflect(engine, only=[table_name])
    return Table(table_name, metadata, autoload_with=engine)

def get_central_time(utc_dt):
    central = pytz.timezone('America/Chicago')
    return utc_dt.astimezone(central)

def fetch_coinbase_order_book(ticker, common_timestamp, level=2):
    try:
        res = requests.get(f'https://api.exchange.coinbase.com/products/{ticker}/book?level={level}')
        res.raise_for_status()
        order_book = res.json()
        if level == 2:
            order_book['bids'] = order_book['bids'][:50]
            order_book['asks'] = order_book['asks'][:50]
        bids = order_book['bids']
        asks = order_book['asks']
        bid_price = float(bids[0][0]) if bids else None
        ask_price = float(asks[0][0]) if asks else None
        if bid_price is not None and ask_price is not None:
            market_price = (ask_price + bid_price) / 2
        else:
            market_price = None
        return ask_price, bid_price, market_price, order_book
    except Exception as e:
        log_module_event(control_engine, 'ObObscanner', Obscanner_id, node_id, 'error', str(e))
        raise

def fetch_kraken_order_book(pair, timeout=10, level=10):
    url = f"https://api.kraken.com/0/public/Depth?pair={pair}&count={level}"
    try:
        response = requests.get(url, timeout=timeout)
        response.raise_for_status()
        data = response.json()
        if 'error' in data and data['error']:
            print(f"Error from Kraken: {data['error']}")
            return None, None, None, None
        result = data['result']
        key = list(result.keys())[0]
        bids = result[key]['bids']
        asks = result[key]['asks']
        bid_price = float(bids[0][0]) if bids else None
        ask_price = float(asks[0][0]) if asks else None
        if bid_price is not None and ask_price is not None:
            market_price = (bid_price + ask_price) / 2
        else:
            market_price = None
        order_book_data = {'bids': bids, 'asks': asks}
        return ask_price, bid_price, market_price, order_book_data
    except Exception as e:
        log_module_event(control_engine, 'Obscanner', Obscanner_id, node_id, 'error', str(e))
        raise

def fetch_binance_order_book(symbol, timeout=10, level=10):
    url = f"https://api.binance.us/api/v3/depth?symbol={symbol}&limit={level}"
    try:
        response = requests.get(url, timeout=timeout)
        response.raise_for_status()
        data = response.json()
        bids = data['bids']
        asks = data['asks']
        bid_price = float(bids[0][0]) if bids else None
        ask_price = float(asks[0][0]) if asks else None
        if bid_price is not None and ask_price is not None:
            market_price = (bid_price + ask_price) / 2
        else:
            market_price = None
        order_book_data = {'bids': bids, 'asks': asks}
        return ask_price, bid_price, market_price, order_book_data
    except Exception as e:
        print(f"Binance order book fetch failed for {symbol}: {e}")
        return None, None, None, None

def fetch_data_with_retry(fetch_function, exchange_name, *args, **kwargs):
    if exchange_name in blacklist:
        elapsed_time = time.time() - blacklist[exchange_name]
        if elapsed_time < COOLDOWN_PERIOD:
            print(f"{exchange_name} is currently blacklisted. Skipping...")
            return None
        del blacklist[exchange_name]
    for attempt in range(MAX_RETRIES):
        try:
            return fetch_function(*args, **kwargs)
        except (requests.exceptions.ConnectTimeout, requests.exceptions.ReadTimeout) as e:
            print(f"{e.__class__.__name__} error occurred on attempt {attempt + 1}. Retrying...")
            time.sleep(1)
    print(f"Failed to fetch data from {exchange_name} after {MAX_RETRIES} attempts. Blacklisting for {COOLDOWN_PERIOD} seconds...")
    blacklist[exchange_name] = time.time()
    return None

def fetch_asset_exchange_metadata(start_asset, end_asset):
    query = text('''
        SELECT
          name, coinbase_status, coinbaseticker, coinbasetradepairs,
          kraken_status, krakenticker, krakentradepairs,
          binance_status, binanceticker, binancetradepairs
        FROM crypto
        WHERE type = "crypto"
    ''')
    with assets_engine.connect() as conn:
        result = conn.execute(query).fetchall()

    # Build a list of all asset names, sorted alphabetically
    all_assets = sorted(result, key=lambda r: r[0].lower())

    # Find the start and end asset index
    names = [row[0].lower() for row in all_assets]
    start_asset = start_asset.lower()
    end_asset = end_asset.lower()
    try:
        start_idx = names.index(start_asset)
        end_idx = names.index(end_asset)
    except ValueError as e:
        raise Exception(f"Asset not found in list: {e}")

    # Make sure indices are in the right order
    if start_idx > end_idx:
        start_idx, end_idx = end_idx, start_idx

    filtered = all_assets[start_idx:end_idx+1]
    return filtered

print(f"[Scanner Shard] Processing coins {start_asset}-{end_asset}")

while True:
    try:
        all_assets = fetch_asset_exchange_metadata(start_asset, end_asset)
        for row in all_assets:
            (name, cb_status, cb_ticker, cb_tradepairs_json,
             kr_status, kr_ticker, kr_tradepairs_json,
             bn_status, bn_ticker, bn_tradepairs_json) = row
            table_name = name.lower()
            if not acquire_lock(table_name):
                print(f"[{table_name}] Skipped (locked by another worker)")
                continue
            common_timestamp_utc = datetime.utcnow().replace(tzinfo=pytz.utc)
            common_timestamp_ct = get_central_time(common_timestamp_utc)
            data_entry = {'timestamp': common_timestamp_ct}
            # Parse JSON trade pairs for each exchange
            cb_tradepairs = json.loads(cb_tradepairs_json) if cb_tradepairs_json else []
            kr_tradepairs = json.loads(kr_tradepairs_json) if kr_tradepairs_json else []
            bn_tradepairs = json.loads(bn_tradepairs_json) if bn_tradepairs_json else []
            # Coinbase
            if cb_status == 'active' and cb_ticker and cb_tradepairs:
                for quote in cb_tradepairs:
                    cb_pair = f"{cb_ticker}-{quote}"
                    ask, bid, price, ob = fetch_data_with_retry(fetch_coinbase_order_book, "coinbase", cb_pair,
                                                                common_timestamp_ct, level=2) or (None, None, None,
                                                                                                  None)
                    if price is not None:
                        data_entry.update({
                            'market_price': price, 'ask_price': ask, 'bid_price': bid, 'order_book': ob
                        })
            # Kraken
            if kr_status == 'active' and kr_ticker and kr_tradepairs:
                for quote in kr_tradepairs:
                    kr_pair = f"{kr_ticker}{quote}"
                    ask2, bid2, price2, ob2 = fetch_data_with_retry(fetch_kraken_order_book, "kraken", kr_pair,
                                                                    timeout=10, level=2) or (None, None, None, None)
                    if price2 is not None:
                        data_entry.update({
                            'market_price2': price2, 'ask_price2': ask2, 'bid_price2': bid2, 'order_book2': ob2
                        })
            # Binance
            if bn_status == 'active' and bn_ticker and bn_tradepairs:
                for quote in bn_tradepairs:
                    bn_pair = f"{bn_ticker}{quote}"
                    ask3, bid3, price3, ob3 = fetch_data_with_retry(fetch_binance_order_book, "binance", bn_pair,
                                                                    timeout=10, level=2) or (None, None, None, None)
                    if price3 is not None:
                        data_entry.update({
                            'market_price3': price3, 'ask_price3': ask3, 'bid_price3': bid3, 'order_book3': ob3
                        })
            data_entry = {k: v for k, v in data_entry.items() if v is not None}
            try:
                # Check if the table exists, otherwise create it
                if not check_if_table_exists(dragnet_engine, table_name):
                    ticker_table = create_table(dragnet_engine, table_name)
                else:
                    ticker_table = load_table(dragnet_engine, table_name)
                insert_crypto_data(dragnet_engine, ticker_table, [data_entry])
                print(f"Orderbook Data for {table_name} inserted.")
            except Exception as e:
                print(f"Error processing {table_name}: {e}")
            finally:
                release_lock(table_name)
            try:
                del data_entry
                del ticker_table
                del cb_tradepairs
                del kr_tradepairs
                del bn_tradepairs
            except:
                pass
            gc.collect()
            set_heartbeat(control_engine, Obscanner_id)
        time.sleep(float(delay) if delay else 5)
    except Exception as e:
        set_module_status(control_engine, Obscanner_id, "Stopped")
        log_module_event(control_engine, 'orderbook', Obscanner_id, node_id, 'error', f"CRASH: {str(e)}")
        raise

if __name__ == "__main__":
    main()
