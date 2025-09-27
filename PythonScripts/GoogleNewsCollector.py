import sys, os, time, json, random
import numpy as np
import pandas as pd
from datetime import datetime, timedelta
import pytz
from sqlalchemy import create_engine, MetaData, Table, Column, DateTime, String, Float, UniqueConstraint, inspect, text
from sqlalchemy.dialects.mysql import insert, LONGTEXT
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.keys import Keys
from selenium.common.exceptions import TimeoutException, WebDriverException
import requests
from urllib.parse import urljoin, urlparse
from selenium.webdriver.chrome.options import Options
import nltk
nltk.download('punkt_tab')
import undetected_chromedriver as uc
import chaos_monkey
import concurrent.futures
import socket

def log_module_event(control_engine, module_type, module_id, module_ip, log_level, message):
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

if len(sys.argv) < 13:
    print("Data pass incomplete. Values missing or in error")
    time.sleep(10)
    sys.exit(1)
'''
newsscraper_id = 'scrapertest'
news_db_host = '192.168.1.210'
news_db_username = 'dragnet'
news_db_password = 'dragnet5'
control_db_host = '192.168.1.210'
control_db_user = 'dragnet'
control_db_pass = 'dragnet5'
assets_db_host = '192.168.1.210'
assets_db_username = 'dragnet'
assets_db_pass = 'dragnet5'
start_asset = 'aave'
end_asset = 'bitcoin'
hours = float(24)
'''
newsscraper_id = sys.argv[1]
news_db_host = sys.argv[2]
news_db_username = sys.argv[3]
news_db_password = sys.argv[4]
control_db_host = sys.argv[5]
control_db_user = sys.argv[6]
control_db_pass = sys.argv[7]
assets_db_host = sys.argv[8]
assets_db_username = sys.argv[9]
assets_db_pass = sys.argv[10]
start_asset = sys.argv[11]
end_asset = sys.argv[12]
hours = float(sys.argv[13])

engine_assets = f'mysql+mysqldb://{assets_db_username}:{assets_db_pass}@{assets_db_host}/assets'
assets_engine = create_engine(engine_assets)
engine_control = f'mysql+mysqldb://{control_db_user}:{control_db_pass}@{control_db_host}'
control_engine = create_engine(engine_control)
engine_dragnet = f'mysql+mysqldb://{news_db_username}:{news_db_password}@{news_db_host}/googlenewsdata'
news_engine = create_engine(engine_dragnet)
now = datetime.now()

try:
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.connect(('8.8.8.8', 80))  # Google's DNS; not actually contacted
    node_ip = s.getsockname()[0]
    s.close()
except Exception as e:
    log_module_event(control_engine, 'newsscraper', newsscraper_id, node_ip, 'error', str(e))
    raise

try:
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.newsscraper_modules
                (newsscraper_id, node_ip, asset_range_start, asset_range_end, status, created_at, last_heartbeat)
            VALUES
                (:newsscraper_id, :node_ip, :asset_range_start, :asset_range_end, :status, :created_at, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                node_ip = VALUES(node_ip),
                newsscraper_id = VALUES(newsscraper_id),
                asset_range_start = VALUES(asset_range_start),
                asset_range_end = VALUES(asset_range_end),
                status = "Running",
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'node_ip': node_ip,
            'newsscraper_id': str(newsscraper_id),
            'asset_range_start': start_asset,
            'asset_range_end': end_asset,
            'status': ("Running"),
            'created_at': now,
            'last_heartbeat': now
        })
        conn.commit()
except Exception as e:
    log_module_event(control_engine, 'newsscraper', newsscraper_id, node_ip, 'error', str(e))
    raise

def set_heartbeat(control_engine, newsscraper_id):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            INSERT INTO dragnetcontrol.newsscraper_modules (newsscraper_id, last_heartbeat)
            VALUES (:newsscraper_id, :last_heartbeat)
            ON DUPLICATE KEY UPDATE
                last_heartbeat = VALUES(last_heartbeat)
        """)
        conn.execute(query, {
            'newsscraper_id': newsscraper_id,
            'last_heartbeat': now
        })
        conn.commit()

def set_module_status(control_engine, newsscraper_id, status):
    now = datetime.now()
    with control_engine.connect() as conn:
        query = text("""
            UPDATE dragnetcontrol.newsscraper_modules
            SET status = :status, last_heartbeat = :last_heartbeat
            WHERE newsscraper_id = :newsscraper_id
        """)
        conn.execute(query, {
            'newsscraper_id': newsscraper_id,
            'status': status,
            'last_heartbeat': now
        })
        conn.commit()

USER_AGENTS = [
    # Sprinkle in a few, you can add more
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.4 Safari/605.1.15",
    "Mozilla/5.0 (Windows NT 10.0; rv:124.0) Gecko/20100101 Firefox/124.0",
]
# Path for a persistent Chrome user profile
CHROME_PROFILE_PATH = os.path.expanduser("~/.real_chrome_profile")
# ================ END CONFIG ====================
try:
    nltk.data.find('tokenizers/punkt')
except LookupError:
    nltk.download('punkt')

def fetch_assets(engine, asset_table='crypto', start_asset=None, end_asset=None):
    # Get all assets
    query = (
        f'SELECT name, SearchTerms FROM {asset_table} '
        'WHERE (Type = "Crypto" OR Type = "Stock") '
        'AND (coinbase_trackingstatus = "active" '
        'OR binance_trackingstatus = "active" '
        'OR kraken_trackingstatus = "active")'
    )
    df = pd.read_sql(query, engine)

    # Range filter by full name, not just first letter
    if start_asset is not None and end_asset is not None:
        start = start_asset.lower()
        end = end_asset.lower()
        df = df.sort_values('name')
        df = df[(df['name'].str.lower() >= start) & (df['name'].str.lower() <= end)]

    return df.to_dict(orient='records')

def create_table(engine, table_name):
    metadata = MetaData()
    news_table = Table(
        table_name,
        metadata,
        Column('timestamp', DateTime),
        Column('source', String(255)),
        Column('title', String(500), primary_key=True),
        Column('url', String(500)),
        Column('positive_score', Float),
        Column('negative_score', Float),
        Column('asset_relevance_score', Float),
        Column('temporal_relevance_score', Float),
        Column('body', LONGTEXT),
        UniqueConstraint('title', name='uix_title')
    )
    metadata.create_all(engine)
    return news_table

def check_if_table_exists(engine, table_name):
    inspector = inspect(engine)
    return table_name in inspector.get_table_names()

def article_title_exists(engine, table_name, title):
    metadata = MetaData()
    metadata.reflect(bind=engine)
    table = metadata.tables[table_name]
    with engine.connect() as conn:
        result = conn.execute(table.select().where(table.c.title == title)).fetchone()
        return result is not None

def fetch_google_news_articles(search_term):
    headers = {
        "User-Agent": random.choice(USER_AGENTS)
    }
    base_url = 'https://news.google.com'
    search_url = f'{base_url}/search?q={search_term}&hl=en-US&gl=US&ceid=US%3Aen'
    response = requests.get(search_url, headers=headers)
    soup = BeautifulSoup(response.text, 'html.parser')
    articles = soup.find_all('article', {'class': 'IFHyqb DeXSAc'})
    current_time = datetime.now(pytz.utc)
    lookback_delta = timedelta(hours=float(hours))
    cutoff_time = current_time - lookback_delta
    central_tz = pytz.timezone('US/Central')
    for article in articles:
        title_tag = article.find('a', {'class': 'JtKRv'})
        title = title_tag.text.strip() if title_tag else "No Title Found"
        link_tag = article.find('a', {'class': 'WwrzSb'})
        url = "No URL Found"
        if link_tag:
            raw_link = link_tag['href']
            if raw_link.startswith('./') or raw_link.startswith('/'):
                url = urljoin(base_url, raw_link)
            else:
                url = raw_link
        timestamp_tag = article.find('time', {'class': 'hvbAAd'})
        timestamp_str = timestamp_tag['datetime'] if timestamp_tag else None
        timestamp = datetime.fromisoformat(timestamp_str.replace("Z", "+00:00")) if timestamp_str else None
        if timestamp:
            timestamp = timestamp.astimezone(central_tz)
        if timestamp and timestamp >= cutoff_time:
            yield {'title': title, 'url': url, 'timestamp': timestamp}

def patch_webdriver_fingerprints(driver):
    # Remove some obvious Selenium fingerprints after page load
    driver.execute_script("""
        Object.defineProperty(navigator, 'webdriver', {get: () => undefined});
        window.navigator.chrome = { runtime: {} };
        Object.defineProperty(navigator, 'plugins', {get: () => [1,2,3,4,5]});
        Object.defineProperty(navigator, 'languages', {get: () => ['en-US', 'en']});
    """)

def cleanup_tabs(driver):
    main_handle = driver.current_window_handle
    # Kill popups
    for handle in driver.window_handles:
        if handle != main_handle:
            driver.switch_to.window(handle)
            driver.close()
    driver.switch_to.window(main_handle)

def scrape_article_with_browser(driver, engine, table_name, article, timeout=35):
    url = article['url']
    title = article['title']
    timestamp = article['timestamp']

    print(f"Scraping article for {table_name}: {title} ({url})")
    if article_title_exists(engine, table_name, title):
        print(f"Already have article '{title}'. Skipping.")
        return

    try:
        time.sleep(np.random.uniform(1.3, 5.5))
        driver.set_page_load_timeout(timeout)
        try:
            driver.get(url)
            time.sleep(np.random.uniform(2, 4))
            patch_webdriver_fingerprints(driver)
        except TimeoutException:
            print(f"Timeout loading {url}! Skipping.")
            try:
                driver.execute_script("window.stop();")
            except Exception:
                pass
            cleanup_tabs(driver)
            return
        except WebDriverException as we:
            print(f"WebDriverException at get({url}): {we}")
            cleanup_tabs(driver)
            return

        # Handle cloudflare or scammy redirects
        html = driver.page_source.lower()
        #if "cloudflare" in html or "cf-challenge" in html or "captcha" in html:
            #print(f"Blocked by Cloudflare or Captcha at {url}. Skipping.")
            #cleanup_tabs(driver)
            #return
        if any(keyword in html for keyword in ["verify you're human", "prove you are not a robot", "are you a human"]):
            print(f"Possible anti-bot at {url}, skipping.")
            cleanup_tabs(driver)
            return

        chaos_monkey.chaos_monkey_actions(driver, max_steps=15, verbose=False)
        cleanup_tabs(driver)

        soup = BeautifulSoup(driver.page_source, 'html.parser')
        paragraphs = soup.find_all('p')
        raw_text = "\n".join(p.get_text() for p in paragraphs)

        # Defensive: always define sentences, even if nothing is found
        sentences = []
        if raw_text and len(raw_text.strip()) > 0:
            try:
                nltk.data.find('tokenizers/punkt')
            except LookupError:
                nltk.download('punkt')
            sentences = nltk.sent_tokenize(raw_text)
        else:
            print("No article text found, skipping.")
            return

        def is_junk(sentence):
            s = sentence.lower().strip()
            return (
                len(s) < 10 or
                "copyright" in s or
                "all rights reserved" in s or
                "newsletter" in s or
                s.startswith("photo:") or
                s.startswith("read next") or
                s.endswith(".com") or
                s.endswith(".net")
            )

        filtered_sentences = [s for s in sentences if not is_junk(s)]
        body = "\n".join(filtered_sentences)

        source = urlparse(driver.current_url).netloc

        # Save to MySQL
        metadata = MetaData()
        metadata.reflect(bind=engine)
        table = metadata.tables[table_name]
        stmt = insert(table).values(
            timestamp=timestamp,
            source=source,
            title=title,
            url=driver.current_url,
            body=body,
            positive_score=None,
            negative_score=None,
            asset_relevance_score=None,
            temporal_relevance_score=None,
        ).on_duplicate_key_update(
            url=driver.current_url,
            body=body,
            timestamp=timestamp,
        )

        with engine.connect() as conn:
            conn.execute(stmt)
            conn.commit()

        print(f"Saved article '{title}' from {source}")
        time.sleep(np.random.uniform(2.2, 7.7))
        chaos_monkey.chaos_monkey_actions(driver, max_steps=15, verbose=False)

    except TimeoutException:
        print(f"Timeout (outer) scraping article '{title}'. Skipping.")
        try:
            driver.execute_script("window.stop();")
        except Exception:
            pass
    except WebDriverException as e:
        print(f"WebDriverException scraping '{title}': {e}. Trying to recover...")
        try:
            driver.execute_script("window.stop();")
        except Exception:
            pass
    except Exception as e:
        print(f"Error scraping article '{title}': {e}")

def make_driver():
    options = Options()
    options.add_argument(f"user-agent={random.choice(USER_AGENTS)}")
    width = np.random.randint(1200, 1920)
    height = np.random.randint(900, 1200)
    options.add_argument(f"--window-size={width},{height}")
    options.add_argument(f"--user-data-dir={CHROME_PROFILE_PATH}")
    options.add_argument("--start-maximized")
    options.add_argument("--disable-blink-features=AutomationControlled")
    # options.add_argument("--headless")  # Use normal Chrome for realism
    return uc.Chrome(options=options)

def run_scraper_for_assets(engine, asset_table, per_article_timeout=90):
    try:
        assets = fetch_assets(assets_engine, asset_table, start_asset, end_asset)
        random.shuffle(assets)
        driver = make_driver()
        try:
            for asset in assets:
                name = asset['name']
                search_terms_json = asset['SearchTerms']
                try:
                    search_terms = json.loads(search_terms_json)
                except Exception:
                    search_terms = [search_terms_json]
                table_name = name.lower().replace(' ', '_')
                if not check_if_table_exists(engine, table_name):
                    create_table(engine, table_name)
                random.shuffle(search_terms)
                for search_term in search_terms:
                    print(f"\n---- Google News: {name} / '{search_term}' ----")
                    time.sleep(np.random.uniform(1.5, 6.5))
                    for article in fetch_google_news_articles(f"{search_term} crypto"):
                        # NEW: Track if we hit a timeout or crash for this article
                        failed = False
                        try:
                            with concurrent.futures.ThreadPoolExecutor(max_workers=1) as executor:
                                future = executor.submit(
                                    scrape_article_with_browser, driver, engine, table_name, article, 32 + np.random.randint(0, 16)
                                )
                                future.result(timeout=per_article_timeout)
                        except concurrent.futures.TimeoutError:
                            print(f"Scraping article timed out! Restarting undetected-chromedriver and skipping this article...")
                            failed = True
                        except Exception as e:
                            print(f"Scraping crashed: {e}. Restarting undetected-chromedriver and skipping this article...")
                            failed = True
                        if failed:
                            try:
                                driver.quit()
                            except Exception:
                                pass
                            driver = make_driver()
                            # SKIP this article, go to the next one
                            continue
        finally:
            try:
                driver.quit()
            except Exception:
                pass
            print("Done.")
            set_heartbeat(control_engine, newsscraper_id)
    except Exception as e:
        set_module_status(control_engine, newsscraper_id, "Stopped")
        log_module_event(control_engine, 'newsscraper', newsscraper_id, node_ip, 'error', f"CRASH: {str(e)}")
        raise

if __name__ == "__main__":
    try:
        run_scraper_for_assets(news_engine, 'crypto')
    finally:
        print("Done.")

