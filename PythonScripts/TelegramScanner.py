import sys
import os
import re
import json
import time
import socket
import asyncio
import logging
from datetime import datetime, timedelta, timezone
import pymysql  # needed for mysql+mysqldb
import pytz
from telethon import TelegramClient, types
from telethon.errors import FloodWaitError, ChannelPrivateError, UsernameInvalidError, UsernameNotOccupiedError
from sqlalchemy import create_engine, text, Table, MetaData, Column, Integer, BigInteger, String, DateTime, Boolean, JSON
from sqlalchemy.dialects.mysql import insert
from sqlalchemy.engine import Engine
import random

logging.basicConfig(level=logging.INFO, format='[TG] %(asctime)s %(levelname)s: %(message)s')

# ------------------
# Arg parsing / setup
# ------------------

if len(sys.argv) < 18:
    print("Usage: see header docstring. Not enough args.")
    sys.exit(1)

telegramscanner_id = sys.argv[1]
dragnet_db_host    = sys.argv[2]
dragnet_db_user    = sys.argv[3]
dragnet_db_pass    = sys.argv[4]
control_db_host    = sys.argv[5]
control_db_user    = sys.argv[6]
control_db_pass    = sys.argv[7]
assets_db_host     = sys.argv[8]
assets_db_user     = sys.argv[9]
assets_db_pass     = sys.argv[10]
tg_session_name    = sys.argv[11]
tg_api_id          = int(sys.argv[12])
tg_api_hash        = sys.argv[13]
start_asset        = sys.argv[14].lower()
end_asset          = sys.argv[15].lower()
telephone_no       = sys.argv[16]
delay_seconds      = float(sys.argv[17])
timespan           = int(sys.argv[18])
'''
telegramscanner_id = 'telegramtestscanner'
dragnet_db_host    = '192.168.1.210'
dragnet_db_user    = 'dragnet'
dragnet_db_pass    = 'dragnet5'
control_db_host    = '192.168.1.210'
control_db_user    = 'dragnet'
control_db_pass    = 'dragnet5'
assets_db_host     = '192.168.1.210'
assets_db_user     = 'dragnet'
assets_db_pass     = 'dragnet5'
tg_session_name    = 'testsession'
tg_api_id          = 23120476
tg_api_hash        = 'dcd3b7fa3f1d6d55c985091724ae05c4'
start_asset        = '00 token'
end_asset          = 'zora'
phone_number       = '+19132146287'
delay_seconds      = float(.5)
timespan           = 10
'''
engine_assets  = create_engine(f'mysql+mysqldb://{assets_db_user}:{assets_db_pass}@{assets_db_host}/assets')
engine_control = create_engine(f'mysql+mysqldb://{control_db_user}:{control_db_pass}@{control_db_host}/dragnetcontrol')
engine_tg      = create_engine(f'mysql+mysqldb://{dragnet_db_user}:{dragnet_db_pass}@{dragnet_db_host}/telegramdata')

WORKER_ID = f"{socket.gethostname()}_{os.getpid()}"
# Only ingest messages newer than this rolling window (minutes)
LOOKBACK_MINUTES = int(os.getenv("TG_LOOKBACK_MINUTES", timespan))  # e.g. set TG_LOOKBACK_MINUTES=3
# Optional safety cap per channel per cycle (avoid infinite catch-ups)
MAX_MSGS_PER_CYCLE = int(os.getenv("TG_MAX_MSGS_PER_CYCLE", "5000"))
# ---- new tunables near the top ----
PER_CHANNEL_LIMIT = 500      # safety cap
PAUSE_BETWEEN_CHANNELS = 3  # seconds
# Tunables (env overrides supported)
PER_ASSET_PAUSE_SEC = float(os.getenv("TG_PER_ASSET_PAUSE", 0.4))  # tiny delay between assets
CYCLE_DELAY_SEC     = float(os.getenv("TG_CYCLE_DELAY", 0.5))      # delay between loops

floodwait_blacklist = {}  # {asset_name: expiry_unix_ts}

def is_blacklisted(asset_name):
    return time.time() < floodwait_blacklist.get(asset_name, 0)

def add_to_blacklist(asset_name, duration):
    floodwait_blacklist[asset_name] = time.time() + duration
    print(f"[{asset_name}] Blacklisted for {duration//3600}h due to FloodWait.")
# ------------------
# DB helpers (control)
# ------------------

def log_module_event(engine_ctrl: Engine, module_type: str, module_id: str, module_ip: str, log_level: str, message: str):
    now = datetime.now()
    with engine_ctrl.begin() as conn:
        conn.execute(text("""
            INSERT INTO module_logs (timestamp, module_ip, module_type, module_id, log_level, message)
            VALUES (:ts, :ip, :mt, :mid, :lvl, :msg)
        """), {
            'ts': now, 'ip': module_ip, 'mt': module_type, 'mid': module_id, 'lvl': log_level, 'msg': message[:1024]
        })


def set_heartbeat(engine_ctrl: Engine, telegramscanner_id: str):
    now = datetime.now()
    with engine_ctrl.begin() as conn:
        conn.execute(text("""
            INSERT INTO telegramscanner_modules (telegramscanner_id, node_ip, status, asset_range_start, asset_range_end, created_at, last_heartbeat)
            VALUES (:sid, :ip, 'Running', :a, :b, :c, :h)
            ON DUPLICATE KEY UPDATE last_heartbeat = VALUES(last_heartbeat), status='Running'
        """), {
            'sid': telegramscanner_id, 'ip': WORKER_ID, 'a': start_asset, 'b': end_asset, 'c': datetime.now(), 'h': now
        })


def acquire_lock(engine_ctrl: Engine, key: str, minutes: int = 10) -> bool:
    now = datetime.now()
    exp = now + timedelta(minutes=minutes)
    with engine_ctrl.begin() as conn:
        conn.execute(text("""
            INSERT INTO dragnet_locks (asset_name, locked_by, lock_expiration)
            VALUES (:k, :w, :e)
            ON DUPLICATE KEY UPDATE
              locked_by = IF(lock_expiration < :now, :w, locked_by),
              lock_expiration = IF(lock_expiration < :now, :e, lock_expiration)
        """), {'k': key, 'w': WORKER_ID, 'e': exp, 'now': now})
        owner = conn.execute(text("SELECT locked_by FROM dragnet_locks WHERE asset_name=:k"), {'k': key}).scalar()
        return owner == WORKER_ID


def release_lock(engine_ctrl: Engine, key: str):
    with engine_ctrl.begin() as conn:
        conn.execute(text("DELETE FROM dragnet_locks WHERE asset_name=:k AND locked_by=:w"), {'k': key, 'w': WORKER_ID})


# --------------------
# telegramdata schema
# --------------------

def ensure_tg_schema(engine_tg: Engine):
    meta = MetaData()
    meta.reflect(bind=engine_tg)

    if 'telegram_cursors' not in meta.tables:
        Table('telegram_cursors', meta,
              Column('channel_key', String(255), primary_key=True),
              Column('last_message_id', BigInteger),
              Column('updated_at', DateTime))

    # Per‑asset tables are created lazily via ensure_asset_table()
    meta.create_all(engine_tg)


def snake(s: str) -> str:
    s = s.strip().lower()
    s = re.sub(r"[^a-z0-9]+", "_", s)
    return s.strip('_')


def ensure_asset_table(engine_tg: Engine, asset_name: str) -> Table:
    #print(f"[DEBUG] Entered ensure_asset_table({asset_name})")
    meta = MetaData()
    tbl_name = snake(asset_name)
    #print(f"[DEBUG] About to check if table exists: {tbl_name}")
    # Use a context manager to ensure the connection closes!
    with engine_tg.connect() as conn:
        exists = engine_tg.dialect.has_table(conn, tbl_name)
    #print(f"[DEBUG] Table exists: {exists}")
    if not exists:
        print(f"[DEBUG] Creating table: {tbl_name}")
        Table(tbl_name, meta,
              Column('id', BigInteger, primary_key=True, autoincrement=True),
              Column('asset_name', String(128), index=True),
              Column('asset_symbol', String(32), index=True),
              Column('is_official', Boolean),
              Column('channel_key', String(255), index=True),
              Column('channel_title', String(255)),
              Column('message_id', BigInteger, index=True),
              Column('message_date', DateTime, index=True),
              Column('author_id', BigInteger, nullable=True),
              Column('author_name', String(255), nullable=True),
              Column('text', String(8192)),
              Column('views', Integer, nullable=True),
              Column('forwards', Integer, nullable=True),
              Column('replies', Integer, nullable=True),
              Column('raw', JSON, nullable=True),
              Column('msg_type', String(64)),
              Column('ingested_at', DateTime, default=datetime.utcnow))
        #print(f"[DEBUG] About to call meta.create_all for: {tbl_name}")
        meta.create_all(engine_tg)
        #print(f"[DEBUG] Table created: {tbl_name}")
    else:
        #print(f"[DEBUG] Reflecting metadata for table: {tbl_name}")
        meta.reflect(bind=engine_tg, only=[tbl_name])
        #print(f"[DEBUG] Metadata reflected for table: {tbl_name}")
    tbl = Table(tbl_name, meta, autoload_with=engine_tg)
    #print(f"[DEBUG] Returning table object for: {tbl_name}")
    return tbl

async def run_with_timeout(coro, timeout=30):
    try:
        return await asyncio.wait_for(coro, timeout=timeout)
    except asyncio.TimeoutError:
        logging.error("Timed out!")
        return None

def get_last_msg_id(engine_tg: Engine, channel_key: str) -> int | None:
    with engine_tg.begin() as conn:
        row = conn.execute(text("SELECT last_message_id FROM telegram_cursors WHERE channel_key=:k"), {'k': channel_key}).fetchone()
        return int(row[0]) if row and row[0] is not None else None


def set_last_msg_id(engine_tg: Engine, channel_key: str, msg_id: int):
    with engine_tg.begin() as conn:
        conn.execute(text("""
            INSERT INTO telegram_cursors (channel_key, last_message_id, updated_at)
            VALUES (:k, :m, :u)
            ON DUPLICATE KEY UPDATE last_message_id=VALUES(last_message_id), updated_at=VALUES(updated_at)
        """), {'k': channel_key, 'm': msg_id, 'u': datetime.now(timezone.utc)})


# -----------------------
# Assets & handle helpers
# -----------------------
HANDLE_RX = re.compile(r"^(?:https?://t\.me/|@)?(?P<user>[A-Za-z0-9_]{3,})/?$")

def normalize_handle(raw: str) -> str | None:
    if not raw: return None
    m = HANDLE_RX.match(raw.strip())
    return m.group('user').lower() if m else None


def load_assets_with_channels(engine_assets: Engine, start_letter: str, end_letter: str):
    q = text("""
        SELECT name, symbol, telegram, official
        FROM crypto
        WHERE type='crypto' AND telegram IS NOT NULL AND telegram<>''
    """)
    rows = []
    with engine_assets.connect() as conn:
        for name, sym, tg, off in conn.execute(q):
            first = (name or sym or '')[:1].upper()
            if start_letter <= first <= end_letter:
                h = normalize_handle(tg)
                if h:
                    rows.append({'name': name, 'symbol': sym, 'handle': h, 'official': int(bool(off))})
    rows.sort(key=lambda r: r['name'].lower())
    # Trim by explicit start/end assets, inclusive
    names = [r['name'].lower() for r in rows]
    try:
        si = names.index(start_asset)
        ei = names.index(end_asset)
        if si > ei:
            si, ei = ei, si
        rows = rows[si:ei+1]
    except ValueError:
        # If explicit names not found, just keep the letter range list
        pass
    return rows


# ----------------
# Telethon client
# ----------------
async def get_client():
    client = TelegramClient(tg_session_name, tg_api_id, tg_api_hash)
    await client.connect()
    if await client.is_user_authorized():
        logging.info("Already authorized (session found).")
        return client

    # Not authorized: do full flow
    phone = input("Enter phone in E.164, e.g. +19132146287\nPhone: ").strip()
    if not phone.startswith('+'):
        logging.warning("Phone must be in E.164 format (starts with +).")
    sent = await client.send_code_request(phone)
    code = input("Enter the login code you received: ").strip()

    try:
        await client.sign_in(phone=phone, code=code)
        logging.info("Code accepted.")
    except errors.SessionPasswordNeededError:
        pw = input("Two‑step password enabled. Enter your Telegram password: ").strip()
        await client.sign_in(password=pw)
        logging.info("Password accepted.")
    except errors.SignUpRequiredError:
        first = input("New account. Enter first name to sign up: ").strip() or "Vanir"
        await client.sign_up(code=code, first_name=first)
        logging.info("Signed up new account.")
    except errors.PhoneCodeInvalidError:
        logging.error("Invalid code. Try again.")
        raise
    except errors.FloodWaitError as e:
        logging.error(f"Flood wait: must wait {e.seconds}s.")
        raise

    if not await client.is_user_authorized():
        raise RuntimeError("Login failed for unknown reason.")
    return client


async def resolve_channel(client: TelegramClient, handle: str):
    try:
        return await client.get_entity(handle)
    except (UsernameInvalidError, UsernameNotOccupiedError, ValueError):
        return None


def insert_rows(engine_tg: Engine, tbl: Table, rows: list[dict]) -> int:
    if not rows: return 0
    inserted = 0
    with engine_tg.begin() as conn:
        for r in rows:
            exists = conn.execute(text(f"SELECT 1 FROM {tbl.name} WHERE channel_key=:c AND message_id=:m LIMIT 1"),
                                  {'c': r['channel_key'], 'm': r['message_id']}).fetchone()
            if exists: continue
            conn.execute(tbl.insert().values(**r))
            inserted += 1
    return inserted


async def harvest_one(client: TelegramClient, asset: dict) -> int:
    asset_name  = asset['name']
    asset_sym   = asset['symbol']
    handle      = asset['handle']
    is_official = asset['official']

    print(f"[{asset_name}] Entered harvest_one()")
    if not acquire_lock(engine_control, asset_name):
        print(f"[{asset_name}] Lock held elsewhere, skipping.")
        logging.info(f"{asset_name}: locked elsewhere")
        return 0

    try:
        print(f"[{asset_name}] Resolving channel @{handle}")
        entity = await resolve_channel(client, handle)
        #print(f"[{asset_name}] resolve_channel result: {repr(entity)}")
        if not entity:
            print(f"[{asset_name}] Cannot resolve channel.")
            logging.warning(f"{asset_name}: cannot resolve @{handle}")
            return 0

        #print(f"[{asset_name}] Ensuring asset table.")
        tbl = ensure_asset_table(engine_tg, asset_name)
        channel_key = handle

        #print(f"[{asset_name}] Getting cutoff time and last_id.")
        cutoff_utc = datetime.now(timezone.utc) - timedelta(minutes=LOOKBACK_MINUTES)
        last_id    = get_last_msg_id(engine_tg, channel_key) or 0

        #print(f"[{asset_name}] last_id={last_id}, cutoff_utc={cutoff_utc}")
        rows: list[dict] = []
        newest_id = last_id
        seen      = 0

        async def process_message(msg, entity, for_debug=False):
            # Try to extract all fields, regardless of type.
            result = {
                'asset_name': asset_name,
                'asset_symbol': asset_sym,
                'is_official': is_official,
                'channel_key': channel_key,
                'channel_title': getattr(entity, 'title', None) or channel_key,
                'message_id': int(getattr(msg, 'id', -1)),
                'message_date': getattr(msg, 'date', None),
                'author_id': None,
                'author_name': None,
                'text': None,
                'views': getattr(msg, 'views', None),
                'forwards': getattr(msg, 'forwards', None),
                'replies': getattr(getattr(msg, 'replies', None), 'replies', None) if getattr(msg, 'replies', None) else None,
                'raw': {},
                'ingested_at': datetime.now(timezone.utc).replace(tzinfo=None),
                'msg_type': type(msg).__name__,
            }

            # Try for author fields (if present)
            if hasattr(msg, 'from_id') and getattr(msg, 'from_id', None) and hasattr(msg.from_id, 'user_id'):
                result['author_id'] = msg.from_id.user_id
            if hasattr(msg, 'sender'):
                sender = msg.sender
                if sender and isinstance(sender, (types.User, types.Channel, types.Chat)):
                    result['author_name'] = getattr(sender, 'username', None) or getattr(sender, 'title', None)

            # Try for text body (if present)
            if hasattr(msg, 'message'):
                result['text'] = (getattr(msg, 'message', None) or '').strip()[:8000]
            elif hasattr(msg, 'action'):  # Some ServiceMessage types
                result['text'] = str(getattr(msg, 'action', None))[:8000]
            else:
                result['text'] = None

            # Fill in raw for later analysis
            try:
                # Only include serializable fields (may fail for big TL objects)
                result['raw'] = {k: str(v)[:200] for k, v in vars(msg).items()}
            except Exception:
                result['raw'] = {'error': 'unserializable message'}

            # Print for debugging
            #if for_debug:
                #print(f"[{asset_name}] Parsed message: id={result['message_id']}, type={result['msg_type']}, text='{result['text']}'")
                #if result['raw']:
                    #print(f"[{asset_name}] Raw fields: {list(result['raw'].keys())}")

            return result

        def update_newest(msg, newest_id):
            if hasattr(msg, 'id') and msg.id > newest_id:
                return msg.id
            return newest_id

        def get_msg_dt(msg):
            msg_dt = getattr(msg, 'date', None)
            if msg_dt is None: return None
            if msg_dt.tzinfo: return msg_dt
            return msg_dt.replace(tzinfo=timezone.utc)

        def should_skip_due_to_age(msg_dt, cutoff_utc):
            return (msg_dt is not None) and (msg_dt < cutoff_utc)

        msg_count = 0

        # --- MESSAGE LOOP ---
        iter_args = {"min_id": last_id} if last_id > 0 else {}
        async for msg in client.iter_messages(entity, **iter_args):
            msg_count += 1
            msg_type = type(msg).__name__
            #print(f"[{asset_name}] iter_messages: #{msg_count}, id={getattr(msg, 'id', None)}, type={msg_type}")
            msg_dt = get_msg_dt(msg)
            newest_id = update_newest(msg, newest_id)
            if should_skip_due_to_age(msg_dt, cutoff_utc):
                #print(f"[{asset_name}] Message id={getattr(msg, 'id', None)} is older than cutoff, {'breaking out.' if last_id == 0 else 'skipping.'}")
                if last_id == 0: break
                else: continue

            # Try to parse and insert ALL message types, not just text/chat messages
            row = await process_message(msg, entity, for_debug=True)
            rows.append(row)
            seen += 1

            if seen >= MAX_MSGS_PER_CYCLE:
                #print(f"[{asset_name}] Hit MAX_MSGS_PER_CYCLE, breaking out.")
                break

        #print(f"[{asset_name}] Finished iter_messages loop. {seen} rows collected.")
        #print(f"[{asset_name}] Inserting {len(rows)} rows into table.")
        n_ins = insert_rows(engine_tg, tbl, rows)

        if newest_id and newest_id > last_id:
            #print(f"[{asset_name}] Updating cursor to {newest_id}")
            set_last_msg_id(engine_tg, channel_key, newest_id)

        print(f"[{asset_name}] Done. Inserted={n_ins}, seen={seen}, last_id->{newest_id}")
        logging.info(f"{asset_name}/@{handle}: +{n_ins} msgs (window={LOOKBACK_MINUTES}m, seen={seen}, last_id->{newest_id})")
        return n_ins

    except FloodWaitError as e:
        # If it's a big floodwait (>15 min), just skip for the length of the floodwait (plus 5 min for safety)
        floodwait_seconds = e.seconds + 300
        if e.seconds > 900:  # Arbitrary threshold: only skip if over 15 minutes
            print(
                f"[{asset_name}] Excessive FloodWaitError: blacklisting for {floodwait_seconds // 3600}h, skipping for now.")
            add_to_blacklist(asset_name, floodwait_seconds)
            return 0
        print(f"[{asset_name}] FloodWaitError: sleeping {e.seconds + 1}s then retrying")
        logging.warning(f"{asset_name}: floodwait {e.seconds}s (retrying this asset)")
        await asyncio.sleep(e.seconds + 1)
        return await harvest_one(client, asset, retry_depth=(retry_depth + 1 if 'retry_depth' in locals() else 1))


    except ChannelPrivateError:
        print(f"[{asset_name}] ChannelPrivateError: channel private or invite-only, skipping.")
        logging.warning(f"{asset_name}: channel private or invite-only")
        return 0
    except Exception as e:
        print(f"[{asset_name}] Unexpected exception: {repr(e)}")
        logging.exception(f"{asset_name}: unexpected error: {e}")
        return 0
    finally:
        #print(f"[{asset_name}] Releasing lock (finally).")
        release_lock(engine_control, asset_name)


async def main_loop():
    ensure_tg_schema(engine_tg)
    set_heartbeat(engine_control, telegramscanner_id)
    client = await get_client()

    first_letter = start_asset[:1].upper()
    last_letter  = end_asset[:1].upper()
    assets = load_assets_with_channels(engine_assets, first_letter, last_letter)
    if not assets:
        logging.info("No assets with Telegram handles found in range.")
        return

    while True:
        total = 0
        # --- RANDOMIZE ASSET ORDER! ---
        assets_shuffled = assets[:]
        random.shuffle(assets_shuffled)
        print(f"Asset scrape order this cycle: {[a['name'] for a in assets_shuffled]}")

        for a in assets_shuffled:
            asset_name = a['name']
            if is_blacklisted(asset_name):
                logging.warning(f"{asset_name} is blacklisted due to recent floodwait. Skipping.")
                continue
            logging.info(f"Starting asset {asset_name}")
            n = await run_with_timeout(harvest_one(client, a), timeout=60)
            if n is None:
                logging.error(f"Asset {asset_name} timed out, skipping.")
            else:
                total += n

            delay = random.uniform(2, 8)
            print(f"Delaying for {delay:.1f}s before next asset scrape...")
            await asyncio.sleep(delay)

        set_heartbeat(engine_control, telegramscanner_id)
        logging.info(f"Cycle done. Inserted={total}. Sleeping {CYCLE_DELAY_SEC}s")
        await asyncio.sleep(CYCLE_DELAY_SEC)


if __name__ == '__main__':
    try:
        asyncio.run(main_loop())
    except KeyboardInterrupt:
        log_module_event(engine_control, 'telegramscanner', telegramscanner_id, WORKER_ID, 'info', 'Stopped by user')
        print("Stopped.")
