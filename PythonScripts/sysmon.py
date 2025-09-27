import sys
import time
import socket
import json
import psutil
from datetime import datetime
from sqlalchemy import create_engine, text
import pytz
import io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# --- Configuration from command line ---
if len(sys.argv) < 5:
    print("Usage: python node_status_reporter.py <control_ip> <mysql_user> <mysql_pass>")
    sys.exit(1)

CONTROL_IP = sys.argv[1]
MYSQL_USER = sys.argv[2]
MYSQL_PASS = sys.argv[3]
UPDATE_INTERVAL = float(sys.argv[4])

#CONTROL_IP = "192.168.1.210"
#MYSQL_USER = "dragnet"
#MYSQL_PASS = "dragnet5"
#UPDATE_INTERVAL = .5

CONTROL_DB = "dragnetcontrol"
TABLE_NAME = "dragnet_nodes"

# Identify this node
NODE_NAME = socket.gethostname()

# SQLAlchemy DB engine
engine = create_engine(f"mysql+mysqldb://{MYSQL_USER}:{MYSQL_PASS}@{CONTROL_IP}/{CONTROL_DB}")

def get_system_stats():
    return {
        "cpu": psutil.cpu_percent(interval=1, percpu=True),
        "ram": {
            "used": int(psutil.virtual_memory().used / 1024 / 1024),   # MB
            "total": int(psutil.virtual_memory().total / 1024 / 1024)
        },
        "disk": {
            "used": int(psutil.disk_usage('/').used / 1024 / 1024 / 1024),   # GB
            "total": int(psutil.disk_usage('/').total / 1024 / 1024 / 1024)
        }
    }

def get_local_ip():
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))  # Doesn't send data, just triggers routing
        ip = s.getsockname()[0]
        s.close()
        return ip
    except:
        return "127.0.0.1"  # fallback

NODE_IP = get_local_ip()

def set_hostname():
    cpu_score, ram_score = get_resource_scores()
    with engine.begin() as conn:
        result = conn.execute(text(f"""
            SELECT 1 FROM `{TABLE_NAME}`
            WHERE ip_address = :ip
        """), {"ip": NODE_IP}).fetchone()

        if result:
            print(f"[{NODE_NAME}] Overwriting hostname for {NODE_IP} → {NODE_NAME} and scoring resources")
            conn.execute(text(f"""
                UPDATE `{TABLE_NAME}`
                SET hostname = :hostname,
                    cpu_score = :cpu_score,
                    ram_score = :ram_score
                WHERE ip_address = :ip
            """), {
                "hostname": NODE_NAME,
                "cpu_score": cpu_score,
                "ram_score": ram_score,
                "ip": NODE_IP
            })


def get_resource_scores():
    cpu_count = psutil.cpu_count(logical=True)
    if cpu_count < 4:
        cpu_score = 1
    elif cpu_count < 8:
        cpu_score = 2
    else:
        cpu_score = 3

    ram_total_gb = psutil.virtual_memory().total / 1024 / 1024 / 1024
    if ram_total_gb < 4:
        ram_score = 1
    elif ram_total_gb < 16:
        ram_score = 2
    else:
        ram_score = 3

    return cpu_score, ram_score

def update_node_status():
    status_json = get_system_stats()
    central = pytz.timezone('America/Chicago')
    now = datetime.now(central)

    with engine.begin() as conn:
        # Check if the node already exists by IP
        existing = conn.execute(text(f"""
            SELECT ip_address FROM `{TABLE_NAME}`
            WHERE ip_address = :ip
        """), {"ip": NODE_IP}).fetchone()

        if existing:
            # Update ONLY status + last_seen (don't touch hostname)
            conn.execute(text(f"""
                UPDATE `{TABLE_NAME}`
                SET status = :status,
                    last_seen = :seen
                WHERE ip_address = :ip
            """), {
                "status": json.dumps(status_json),
                "seen": now,
                "ip": NODE_IP
            })
        else:
            cpu_score, ram_score = get_resource_scores()
            # First time → insert new row with hostname and scores
            conn.execute(text(f"""
                       INSERT INTO `{TABLE_NAME}` (hostname, ip_address, status, last_seen, cpu_score, ram_score)
                       VALUES (:hostname, :ip, :status, :seen, :cpu_score, :ram_score)
                   """), {
                "hostname": NODE_NAME,
                "ip": NODE_IP,
                "status": json.dumps(status_json),
                "seen": now,
                "cpu_score": cpu_score,
                "ram_score": ram_score
            })

if __name__ == "__main__":
    print(f"[{NODE_NAME}] Status reporter started (every {UPDATE_INTERVAL}s).")
    set_hostname()
    while True:
        try:
            update_node_status()
        except Exception as e:
            print(f"[{NODE_NAME}] ERROR: {e}")
        time.sleep(UPDATE_INTERVAL)
