import sys
import os
import pymysql
from sqlalchemy import create_engine, text
import pandas as pd
import datetime
from sqlalchemy import create_engine, Column, Integer, Float, String, DateTime, BigInteger, SmallInteger, inspect, JSON
from sqlalchemy.orm import declarative_base, Session, sessionmaker
from sqlalchemy.sql import text
from datetime import datetime, timedelta
import requests
import time
from sqlalchemy import Table, MetaData
from sqlalchemy.dialects.mysql import insert
import json
import urllib3.exceptions
import pytz
import socket
from DragnetUtilities import create_table, check_if_table_exists, insert_crypto_data
import io
import logging

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

COOLDOWN_PERIOD = 60
MAX_RETRIES = 3
current_position = 0
blacklist = {}

if len(sys.argv) < 19:
    print("Data pass incomplete. Values missing or in error")
    time.sleep(10)
    sys.exit(1)

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
delay = sys.argv[13]
coinbase_api_key = sys.argv[14]
coinbase_api_secret = sys.argv[15]
coinbase_passphrase = sys.argv[16]
binance_us_api_key = sys.argv[17]
granularity = int(sys.argv[18])
hours = float(sys.argv[19])

"""scanner_id = 'testscanner'
dragnet_db_host = '192.168.1.210'
dragnet_db_username = 'dragnet'
dragnet_db_password = 'dragnet5'
control_db_host = '192.168.1.210'
control_db_user = 'dragnet'
control_db_pass = 'dragnet5'
assets_db_host = '192.168.1.210'
assets_db_username = 'dragnet'
assets_db_pass = 'dragnet5'
start_asset = 'bitcoin'
end_asset = 'ether'
delay = float(.444)
coinbase_api_key = 'f543e74079ddd719989ed24a29019f02'
coinbase_api_secret = 'LR5Ql2/A6CTyUqtbecJ8IFcsuGbo8WftVtY+VKkHdrcShzLou5cGwB4t6Q7xd3Y6AkYVau0m6+tQdrtqEjXBtQ=='
coinbase_passphrase = '3bnlix8qyh'
binance_us_api_key = '3apj9lJKK5lnUUrJ0gZWUsCgNzf4MUl70e4gVJEh32ReR3ziiVXhZhU46xdx2rdk'
granularity = int(60)
hours = float(.25)"""

engine_assets = f'mysql+mysqldb://{assets_db_username}:{assets_db_pass}@{assets_db_host}/assets'
assets_engine = create_engine(engine_assets)
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
    log_module_event(control_engine, 'scanner', scanner_id, node_ip, 'error', str(e))
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
    log_module_event(control_engine, 'scanner', scanner_id, node_ip, 'error', str(e))
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

def release_lock(asset):
    query = text("""
        DELETE FROM dragnetcontrol.dragnet_locks
        WHERE asset_name = :asset AND locked_by = :worker
    """)
    with control_engine.connect() as conn:
        with conn.begin():
            conn.execute(query, {'asset': asset, 'worker': scanner_id})

def convert_unix_to_central(unix_timestamp):
    utc_time = datetime.utcfromtimestamp(unix_timestamp)

    # Convert the datetime object to a timezone-aware datetime object in UTC.
    utc_tz = pytz.timezone('UTC')
    utc_time = utc_tz.localize(utc_time)

    # Convert to US Central Time.
    us_central = pytz.timezone('US/Central')
    central_time = utc_time.astimezone(us_central)

    # Return as Unix timestamp
    central_unix_timestamp = central_time.timestamp()

    return central_unix_timestamp

def convert_pacific_to_central(unix_timestamp):
    # Create a timezone-aware datetime object in Pacific Time.
    pacific_tz = pytz.timezone('America/Los_Angeles')
    pacific_time = datetime.fromtimestamp(unix_timestamp, pacific_tz)

    # Convert to US Central Time.
    central_tz = pytz.timezone('America/Chicago')
    central_time = pacific_time.astimezone(central_tz)

    # Return as Unix timestamp.
    central_unix_timestamp = central_time.timestamp()

    return central_unix_timestamp

def round_down_to_minute(dt):
    return dt.replace(second=0, microsecond=0)

# Fetch the total number of tickers in the 'stocks' table
query = text("SELECT COUNT(*) FROM crypto")
with assets_engine.connect() as conn:
     total_tickers = conn.execute(query).scalar()

def fetch_binance_us_data(ticker):
    start_time = int((datetime.now() - timedelta(hours=hours)).timestamp())
    interval = granularity // 60  # Calculate interval based on granularity

    headers = {
        "X-MBX-APIKEY": binance_us_api_key
    }

    res = requests.get(f'https://api.binance.us/api/v3/klines?symbol={ticker}&interval={interval}m&startTime={start_time * 1000}', headers=headers)

    data = res.json()
    crypto_data = []
    print(f"Binance API response: {data}")

    if isinstance(data, list):
        for item in data:
            timestamp_utc = int(datetime.fromtimestamp(item[0] // 1000).timestamp()) # Divide the timestamp by 1000
            timestamp_central = convert_unix_to_central(timestamp_utc)
            crypto_data.append({
                'timestamp': timestamp_utc,
                'low_price3': float(item[3]),
                'high_price3': float(item[2]),
                'open_price3': float(item[1]),
                'close_price3': float(item[4]),
                'volume3': float(item[5])
            })
            #print(f"Binance Data:{crypto_data}")
    else:
        print(f"Binance.Us: Unexpected API response: {data}")
    return crypto_data

def fetch_coinbase_data(ticker):
    # Define the timezone for Pacific Time and Central Time
    granularity = 60
    pacific_tz = pytz.timezone('America/Los_Angeles')
    central_tz = pytz.timezone('America/Chicago')

    # Calculate the current time in Pacific Time and the start time for your query
    now_pacific = datetime.now(pacific_tz)
    start_time_pacific = now_pacific - timedelta(hours=hours)

    # Convert to ISO format for the API call
    start_time_iso = start_time_pacific.isoformat()
    end_time_iso = now_pacific.isoformat()

    print(f"Start time (PT): {start_time_iso}")
    print(f"End time (PT): {end_time_iso}")

    headers = {
        "CB-ACCESS-KEY": coinbase_api_key,
        "CB-ACCESS-SIGN": coinbase_api_secret,
        "CB-ACCESS-TIMESTAMP": str(int(datetime.now(pytz.utc).timestamp())),
        "CB-ACCESS-PASSPHRASE": coinbase_passphrase
    }

    # Coinbase API request
    res = requests.get(f'https://api.exchange.coinbase.com/products/{ticker}/candles?granularity={granularity}&start={start_time_iso}&end={end_time_iso}',
        headers=headers)

    data = res.json()
    crypto_data = []

    if isinstance(data, list):
        for item in data:
            # Convert the timestamp from the Coinbase API response from UTC to Central Time
            timestamp_pacific = int(item[0])
            timestamp_central = int(convert_pacific_to_central(timestamp_pacific))

            crypto_data.append({
                'timestamp': timestamp_central,  # This is now a datetime object in Central Time
                'low_price': item[1],
                'high_price': item[2],
                'open_price': item[3],
                'close_price': item[4],
                'volume': item[5]
            })
            print(f"Coinbase Data:{crypto_data}")
    else:
        print(f"Coinbase: Unexpected API response: {data}")

    return crypto_data

def fetch_kraken_data(ticker):
    end_time = int(datetime.now().timestamp())
    start_time = int((datetime.now() - timedelta(hours=hours)).timestamp())
    interval = granularity // 60  # Calculate interval based on granularity

    start_time_iso = datetime.fromtimestamp(start_time).isoformat()
    end_time_iso = datetime.fromtimestamp(end_time).isoformat()
    print(f"Start time: {start_time_iso}")
    print(f"End time: {end_time_iso}")

    res = requests.get(f'https://api.kraken.com/0/public/OHLC?pair={ticker}&interval={interval}&since={start_time}')

    data = res.json()
    crypto_data = []
    print(f"Kraken API response: {data}")

    if 'result' in data:
        result_key = list(data['result'].keys())[0]  # Add this line to extract the correct key
        for item in data['result'][result_key]:
            timestamp_utc = int(item[0])
            timestamp_central = int(convert_unix_to_central(timestamp_utc))
            crypto_data.append({
                'timestamp': timestamp_utc,
                'low_price2': float(item[4]),
                'high_price2': float(item[3]),
                'open_price2': float(item[1]),
                'close_price2': float(item[2]),
                'volume2': float(item[6])
            })
            #print(f"Kraken Data:{crypto_data}")
    else:
        print(f"Kraken: Unexpected API response: {data}")

    return crypto_data

def fetch_data_with_retry(fetch_function, exchange_name, *args, **kwargs):
    # Check if the exchange is blacklisted and the cooldown has elapsed
    if exchange_name in blacklist:
        elapsed_time = time.time() - blacklist[exchange_name]
        if elapsed_time < COOLDOWN_PERIOD:
            print(f"{exchange_name} is currently blacklisted. Skipping...")
            return None
        # If cooldown has elapsed, remove the exchange from the blacklist
        del blacklist[exchange_name]

    for attempt in range(MAX_RETRIES):
        try:
            return fetch_function(*args, **kwargs)
        except (requests.exceptions.ConnectTimeout, requests.exceptions.ReadTimeout) as e:
            print(f"{e.__class__.__name__} error occurred on attempt {attempt + 1}. Retrying...")
            time.sleep(1)  # Wait for 10 seconds before trying again

    print(f"Failed to fetch data from {exchange_name} after {MAX_RETRIES} attempts. Blacklisting for {COOLDOWN_PERIOD} seconds...")
    blacklist[exchange_name] = time.time()  # Add the exchange to the blacklist with the current timestamp
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

while True:
    try:
        all_assets = fetch_asset_exchange_metadata(start_asset, end_asset)  # <--- your new way!
        for row in all_assets:
            (
                base_asset_name, cb_status, cb_ticker, cb_tradepairs_json,
                kr_status, kr_ticker, kr_tradepairs_json,
                bn_status, bn_ticker, bn_tradepairs_json
            ) = row

            table_name = base_asset_name.lower()  # your table is just the asset name

            # 🔐 Attempt to acquire lock before doing anything else
            if not acquire_lock(table_name):
                print(f"[{table_name}] Skipped (locked by another worker)")
                continue

            try:
                # Create or load the table
                if not check_if_table_exists(dragnet_engine, table_name):
                    ticker_table = create_table(dragnet_engine, table_name)
                else:
                    metadata = MetaData()
                    ticker_table = Table(table_name, metadata, autoload_with=dragnet_engine)

                batch_rows = []

                # Parse JSON trade pairs for each exchange
                cb_tradepairs = json.loads(cb_tradepairs_json) if cb_tradepairs_json else []
                kr_tradepairs = json.loads(kr_tradepairs_json) if kr_tradepairs_json else []
                bn_tradepairs = json.loads(bn_tradepairs_json) if bn_tradepairs_json else []

                for quote in set(cb_tradepairs + kr_tradepairs + bn_tradepairs):
                    # Build per-exchange pairs
                    cb_pair = f"{cb_ticker}-{quote}" if cb_ticker else None
                    kr_pair = f"{kr_ticker}{quote}" if kr_ticker else None
                    bn_pair = f"{bn_ticker}{quote}" if bn_ticker else None

                    merged = {}

                    # Coinbase candles
                    if cb_status == "active" and cb_ticker and quote in cb_tradepairs:
                        cb_data = fetch_data_with_retry(fetch_coinbase_data, "coinbase", cb_pair)
                        if cb_data:
                            for row in cb_data:
                                ts = row['timestamp']
                                if ts not in merged: merged[ts] = {}
                                for k, v in row.items(): merged[ts][k] = v

                    # Kraken candles
                    if kr_status == "active" and kr_ticker and quote in kr_tradepairs:
                        kr_data = fetch_data_with_retry(fetch_kraken_data, "kraken", kr_pair)
                        if kr_data:
                            for row in kr_data:
                                ts = row['timestamp']
                                if ts not in merged: merged[ts] = {}
                                for k, v in row.items(): merged[ts][k] = v

                    # Binance.us candles
                    if bn_status == "active" and bn_ticker and quote in bn_tradepairs:
                        bn_data = fetch_data_with_retry(fetch_binance_us_data, "binance", bn_pair)
                        if bn_data:
                            for row in bn_data:
                                ts = row['timestamp']
                                if ts not in merged: merged[ts] = {}
                                for k, v in row.items(): merged[ts][k] = v

                    for ts, r in merged.items():
                        if isinstance(ts, int):
                            r['timestamp'] = datetime.fromtimestamp(ts)
                        batch_rows.append(r)

                if batch_rows:
                    print(f"Inserting {len(batch_rows)} rows into {table_name} ...")
                    insert_crypto_data(dragnet_engine, ticker_table, batch_rows)
                    set_heartbeat(control_engine, scanner_id)
                    print(f"Inserted.")

            except Exception as e:
                print(f"Error processing {table_name}: {e}")

            finally:
                release_lock(table_name)

        dragnet_engine.dispose()
        time.sleep(float(delay) if delay else 5)

    except Exception as e:
        set_module_status(control_engine, scanner_id, "Stopped")
        log_module_event(control_engine, 'scanner', scanner_id, node_ip, 'error', str(e))
        raise

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

if __name__ == "__main__":
    main()