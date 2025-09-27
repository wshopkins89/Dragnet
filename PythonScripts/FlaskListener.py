from flask import Flask, request, jsonify
import subprocess
import os
import sys
import psutil

app = Flask(__name__)

if sys.platform.startswith('win'):
    ALLOWED_SCRIPT_DIR = r"C:\DragnetScripts"
else:
    ALLOWED_SCRIPT_DIR = "/opt/dragnet_scripts"

def is_allowed_path(script_path):
    return os.path.commonpath([script_path, ALLOWED_SCRIPT_DIR]) == ALLOWED_SCRIPT_DIR

def get_os_script_name(script):
    """
    Takes a requested script (e.g. Scanner, Scanner.exe, Scanner.py)
    and returns the correct script name for the host OS.
    """
    base, ext = os.path.splitext(script)
    if sys.platform.startswith('win'):
        # Always use .exe for Windows, or .py if that's all there is
        if ext.lower() == '.py':
            return base + '.py'
        else:
            return base + '.exe'
    else:
        # Always use .py for Linux, or .sh if you want bash scripts
        return base + '.py'

@app.route('/run', methods=['POST'])
def run_script():
    data = request.get_json()
    script = data.get('script')
    args = data.get('args', [])
    if not script:
        return jsonify({"error": "No script specified"}), 400

    # Always resolve script name based on OS
    resolved_script = get_os_script_name(script)
    script_path = os.path.abspath(os.path.join(ALLOWED_SCRIPT_DIR, resolved_script))
    print(f"[INFO] Attempting to run: {script_path} with args: {args}")

    if not is_allowed_path(script_path):
        return jsonify({"error": "Access denied: bad path"}), 403
    if not os.path.isfile(script_path):
        return jsonify({"error": f"Script not found: {resolved_script}"}), 404

    ext = os.path.splitext(script_path)[1].lower()
    if sys.platform.startswith('win'):
        if ext == '.exe':
            cmd = [script_path] + args
        elif ext == '.py':
            cmd = ['python', script_path] + args
        else:
            cmd = [script_path] + args
    else:
        if ext == '.py':
            cmd = ['python3', script_path] + args
        elif ext == '':
            cmd = [script_path] + args
        else:
            cmd = [script_path] + args

    try:
        proc = subprocess.Popen(cmd)
        print(f"[INFO] Started process PID: {proc.pid}")
        return jsonify({
            "pid": proc.pid,
            "status": "launched",
            "cmd": " ".join(cmd)
        })
    except Exception as e:
        print(f"[ERROR] Failed to launch process: {e}")
        return jsonify({"error": str(e)}), 500

@app.route('/kill', methods=['POST'])
def kill_process():
    data = request.get_json()
    process_name = data.get('process')
    pid = data.get('pid')

    if process_name and process_name.lower().startswith("sysmon"):
        return jsonify({"error": "Killing sysmon is not allowed"}), 403

    ALL_DRAGNET_SCRIPTS = [
        "sysmon", "Scanner", "Obscanner", "Curator",
        "GoogleNewsCollector", "TrendsScraper", "CapitolTradesModule"
    ]

    try:
        killed = []
        # Build all possible variants for the allowed names
        def variants(name):
            b, _ = os.path.splitext(name)
            return set([
                b, b.lower(), b.capitalize(),
                b + '.exe', b + '.py',
                (b + '.exe').lower(), (b + '.py').lower()
            ])

        if process_name == "all":
            allowed_names = set()
            for n in ALL_DRAGNET_SCRIPTS:
                allowed_names.update(variants(n))
        elif process_name:
            allowed_names = variants(process_name)
        else:
            return jsonify({"error": "No process name or PID specified"}), 400

        for proc in psutil.process_iter(['pid', 'name', 'cmdline']):
            try:
                pname = (proc.info['name'] or '').lower()
                cmdline = " ".join(proc.info.get('cmdline') or []).lower()

                # Check against all variants (name and cmdline)
                for n in allowed_names:
                    if pname == n:
                        proc.kill()
                        killed.append({"pid": proc.info['pid'], "name": proc.info['name']})
                        break
                    elif pname.startswith('python'):
                        if n in cmdline:
                            proc.kill()
                            killed.append({
                                "pid": proc.info['pid'],
                                "name": proc.info['name'],
                                "cmdline": cmdline
                            })
                            break
            except Exception:
                pass
        return jsonify({"killed": killed})
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/osinfo', methods=['GET'])
def os_info():
    return jsonify({
        "platform": sys.platform,
        "script_dir": ALLOWED_SCRIPT_DIR,
        "cwd": os.getcwd(),
        "user": os.environ.get('USER') or os.environ.get('USERNAME')
    })

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5005, debug=True)
