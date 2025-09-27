import sys
import os
import pandas as pd
from sqlalchemy import create_engine, update, inspect, MetaData, String, Table, Column, DateTime, Float, Integer, inspect, text, bindparam, UniqueConstraint, update
import time
import numpy as np
from urllib.parse import urlparse
from datetime import datetime, timedelta
from sqlalchemy.dialects.mysql import insert
from sqlalchemy.sql import table, literal_column
from sqlalchemy import text
from sqlalchemy.exc import SQLAlchemyError
import concurrent.futures
import pymysql
import requests
import pytz
from urllib.parse import urljoin
import json
import psycopg2
import CustomPyTrends
from CustomPyTrends.request import TrendReq
from CustomPyTrends import exceptions
import pytz
from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
import random

USERS_DB_USERNAME = 'root'
USERS_DB_PASSWORD = 'password'
USERS_DB_HOST = 'localhost'
USERS_DB_NAME = 'userdata'
users_engine = create_engine(f'mysql+pymysql://{USERS_DB_USERNAME}:{USERS_DB_PASSWORD}@{USERS_DB_HOST}/{USERS_DB_NAME}')

# Assets Setup
ASSETS_DB_USERNAME = 'root'
ASSETS_DB_PASSWORD = 'password'
ASSETS_DB_HOST = 'localhost'
ASSETS_DB_NAME = 'assets'
assets_engine = create_engine(f'mysql+pymysql://{ASSETS_DB_USERNAME}:{ASSETS_DB_PASSWORD}@{ASSETS_DB_HOST}/{ASSETS_DB_NAME}')

# Working DB Setup
WORKING_DB_USERNAME = 'root'
WORKING_DB_PASSWORD = 'password'
WORKING_DB_HOST = 'localhost'
WORKING_DB_NAME = 'dragnet'
working_engine = create_engine(f'mysql+pymysql://{WORKING_DB_USERNAME}:{WORKING_DB_PASSWORD}@{WORKING_DB_HOST}/{WORKING_DB_NAME}')

def get_user_agents():
    return [
        'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0',
        'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36',
        'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36',
        'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Safari/605.1.15',
        'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Edge/91.0.864.59',
        'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:89.0) Gecko/20100101 Firefox/89.0',
        'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0',
        'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36',
        'Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0',
        'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36'
        'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36',
        'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36',
        'Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1',
        'Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0',
        'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9',
        'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1',
        'Mozilla/5.0 (CrKey armv7l 1.5.16041) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.0 Safari/537.36'

    ]

def fetch_assets(engine, asset_table):
    query = f'SELECT name, SearchTerms FROM {asset_table} WHERE (Type = "Crypto") OR (Type = "Stock" AND Status = "Active")'
    return pd.read_sql(query, engine).to_dict(orient='records')

def get_random_user_agent():
    user_agents = get_user_agents()
    return random.choice(user_agents)

def random_delay(min_sec=2, max_sec=6):
    time.sleep(random.uniform(min_sec, max_sec))

from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By

def get_fresh_cookies(search_term="Bitcoin"):
    options = Options()
    USER_AGENT = get_random_user_agent()
    options.add_argument(f"user-agent={USER_AGENT}")
    options.add_experimental_option("excludeSwitches", ["enable-automation"])
    options.add_experimental_option('useAutomationExtension', False)

    driver = webdriver.Chrome(options=options)
    driver.get("https://trends.google.com/trends")
    random_delay()

    try:
        # Finding the search input by its ID and entering the search term
        search_input = driver.find_element(By.ID, 'i9')
        search_input.clear()
        search_input.send_keys("Cryptocurrency")
        search_input.send_keys(Keys.ENTER)  # Pressing ENTER to perform the search
        random_delay(5, 10)  # Giving some time for the search results to load

    except Exception as e:
        print(f"An error occurred while trying to perform a search: {e}")

    cookies = driver.get_cookies()
    driver.quit()

    return {cookie['name']: cookie['value'] for cookie in cookies}

def fetch_google_trends_score(search_term):
    cst = pytz.timezone('America/Chicago')  # Define Central Time Zone

    for _ in range(5):  # Trying 5 times
        try:
            # Randomly selecting a user agent
            user_agent = get_random_user_agent()

            pytrends = TrendReq(hl='en-US', retries=3, backoff_factor=20)

            # Setting up headers
            pytrends.headers.update({'User-Agent': user_agent})

            # Getting new cookies if needed
            #pytrends.cookies.update(get_fresh_cookies(user_agent))

            kw_list = [search_term]
            pytrends.build_payload(kw_list, cat=0, timeframe='now 7-d', geo='', gprop='')

            # Getting the data
            data = pytrends.interest_over_time()

            # Convert the datetime index to Central Time Zone
            data.index = data.index.tz_localize('UTC').tz_convert(cst).tz_localize(None)

            return data

        except Exception as e:
            print(f"An error occurred: {e}. Retrying...")

            # If a 429 error occurs, wait a bit before retrying
            if "429" in str(e):
                random_delay()

    # If after 5 tries, it doesn't succeed, return None or you can raise a specific exception
    print("Failed to fetch the Google Trends score after multiple attempts.")
    return None


from sqlalchemy import create_engine, MetaData, Table, update, select, exc


from sqlalchemy.sql import select, and_

from sqlalchemy.sql import text


def update_trends_data(engine, name, data, search_term):
    print(f'UpdateTrends Processing {name}')

    with engine.connect() as conn:
        try:
            # Query to get filtered table names in the schema based on 'name'
            query = text(f"SELECT table_name FROM information_schema.tables WHERE table_schema = 'dragnet' "
                         f"AND (table_name = :name OR table_name LIKE :like_name OR table_name LIKE :tether_name)")

            # Execute the query to get table names
            result = conn.execute(query, {'name': name.lower(), 'like_name': f"{name.lower()}-usd%", 'tether_name': f"{name.lower()}-tether%"})
            table_names = [row[0] for row in result.fetchall()]

            for table_name in table_names:
                print(f'{table_name} selected')

                for index, row in data.iterrows():
                    timestamp = index.to_pydatetime()
                    score = row[f'{search_term}']

                    # Insert or update data
                    upsert_query = text(f"INSERT INTO `dragnet`.`{table_name}` (timestamp, google_trends_score) "
                                        f"VALUES (:timestamp, :score) "
                                        f"ON DUPLICATE KEY UPDATE google_trends_score = VALUES(google_trends_score)")
                    conn.execute(upsert_query, {'timestamp': timestamp, 'score': score})
                    print(f"Upserted data into {table_name} at {timestamp}")

                conn.commit()  # Committing after all rows have been processed

        except Exception as e:
            print(f"An error occurred: {e}")
            conn.rollback()


asset_tables = ['crypto', 'stocks']  # You can add more tables as needed

for asset_table in asset_tables:
    assets = fetch_assets(assets_engine, asset_table)

    # Randomize the order of assets
    random.shuffle(assets)

    for asset in assets:
        name = asset['name']
        print(f'Processing {name}')
        search_terms_json = json.loads(asset['SearchTerms'])  # Assuming the search_terms column stores JSON as text

        # No need to shuffle search_terms_json as we're shuffling assets now
        for search_term in search_terms_json:
            data = fetch_google_trends_score(search_term)
            if data is not None:
                update_trends_data(working_engine, name, data, search_term)
                print(data)
            else:
                print(f"Data not fetched for {search_term}")
            time.sleep(random.uniform(5, 15))  # Random delay between each search term

        print(f"Completed processing for {name}.")
    print(f"Completed processing all assets in {asset_table}.")

print("Completed processing all assets.")