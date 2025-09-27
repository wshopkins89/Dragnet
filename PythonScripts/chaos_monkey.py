import random
import time
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By

def get_body_size(driver):
    # Returns (width, height) of the <body> or (1024, 768) as fallback
    try:
        size = driver.execute_script("""
            return [document.body ? document.body.scrollWidth : 1024, document.body ? document.body.scrollHeight : 768];
        """)
        return max(200, int(size[0])), max(200, int(size[1]))
    except Exception:
        return 1024, 768

def do_random_mouse_move(driver, actions, verbose=False):
    try:
        body = driver.find_element(By.TAG_NAME, "body")
        width, height = get_body_size(driver)
        # Never use 0-sized or ultra-small pages
        if width < 50 or height < 50:
            return
        # Stay within bounds, away from absolute edge
        x = random.randint(5, width - 10)
        y = random.randint(5, height - 10)
        actions.move_to_element_with_offset(body, x, y).perform()
        if verbose:
            print(f"Mouse move to ({x},{y})")
    except Exception as e:
        if verbose:
            print("Mouse move failed:", e)

def do_random_scroll(driver, verbose=False):
    try:
        scroll_y = random.choice([random.randint(-400, -50), random.randint(50, 400), 0])
        driver.execute_script(f"window.scrollBy(0, {scroll_y});")
        if verbose:
            print(f"Scrolled by {scroll_y}px")
    except Exception as e:
        if verbose:
            print("Scroll failed:", e)

def do_random_keypress(actions, verbose=False):
    keys = [Keys.BACKSPACE, Keys.TAB, Keys.ENTER, Keys.ARROW_DOWN, Keys.ARROW_UP]
    try:
        key = random.choice(keys)
        actions.send_keys(key).perform()
        if verbose:
            print(f"Pressed key {key}")
    except Exception as e:
        if verbose:
            print("Keypress failed:", e)

def do_random_select_text(driver, verbose=False):
    try:
        driver.execute_script("""
            var ps = document.getElementsByTagName('p');
            if(ps.length) {
                var start = Math.floor(Math.random() * ps.length);
                var end = Math.min(start + Math.floor(Math.random() * 3), ps.length - 1);
                var range = document.createRange();
                var sel = window.getSelection();
                range.setStart(ps[start], 0);
                range.setEnd(ps[end], 0);
                sel.removeAllRanges();
                sel.addRange(range);
            }
        """)
        if verbose:
            print("Selected random text.")
    except Exception as e:
        if verbose:
            print("Text selection failed:", e)

def do_random_double_click(driver, actions, verbose=False):
    try:
        tags = ['p', 'a', 'span', 'h2', 'img']
        elems = []
        for tag in tags:
            found = driver.find_elements(By.TAG_NAME, tag)
            if found:
                elems += found
        if elems:
            elem = random.choice(elems)
            actions.move_to_element(elem).double_click(elem).perform()
            if verbose:
                print("Double-clicked random element.")
    except Exception as e:
        if verbose:
            print("Double click failed:", e)

def do_random_tab_action(driver, verbose=False):
    try:
        if random.random() < 0.5:
            driver.execute_script("window.open('about:blank','_blank');")
            time.sleep(random.uniform(0.5, 2.5))
            if len(driver.window_handles) > 1:
                driver.switch_to.window(driver.window_handles[-1])
                driver.close()
                driver.switch_to.window(driver.window_handles[0])
                if verbose:
                    print("Opened and closed a new tab.")
        else:
            if len(driver.window_handles) > 1:
                idx = random.randint(0, len(driver.window_handles) - 1)
                driver.switch_to.window(driver.window_handles[idx])
                time.sleep(random.uniform(0.1, 0.8))
                driver.switch_to.window(driver.window_handles[0])
                if verbose:
                    print(f"Switched to tab {idx} and back.")
    except Exception as e:
        if verbose:
            print("Tab action failed:", e)

def do_random_pause(verbose=False):
    t = random.uniform(0.25, 7.0)
    if verbose:
        print(f"Pausing for {t:.2f} seconds.")
    time.sleep(t)

def do_random_devtools(actions, verbose=False):
    try:
        actions.send_keys(Keys.F12).perform()
        time.sleep(random.uniform(0.1, 1.0))
        actions.send_keys(Keys.F12).perform()
        if verbose:
            print("Opened and closed DevTools.")
    except Exception as e:
        if verbose:
            print("DevTools action failed:", e)

def chaos_monkey_actions(driver, max_steps=20, verbose=False):
    actions = ActionChains(driver)
    for _ in range(random.randint(4, max_steps)):
        roll = random.random()
        if roll < 0.20:
            do_random_mouse_move(driver, actions, verbose)
        elif roll < 0.32:
            do_random_scroll(driver, verbose)
        elif roll < 0.38:
            do_random_keypress(actions, verbose)
        elif roll < 0.46:
            do_random_select_text(driver, verbose)
        elif roll < 0.54:
            do_random_double_click(driver, actions, verbose)
        elif roll < 0.62:
            do_random_tab_action(driver, verbose)
        elif roll < 0.70:
            do_random_pause(verbose)
        elif roll < 0.74:
            do_random_devtools(actions, verbose)
        else:
            # Sometimes... do absolutely nothing
            t = random.uniform(0.1, 0.6)
            if verbose:
                print(f"Idle for {t:.2f}s")
            time.sleep(t)
