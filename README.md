## Dragnet Control

### Overview
**Dragnet Control** is a Windows desktop operations console built on **.NET 8 WinForms** that orchestrates data-ingestion nodes, scanners, and analytics services across a distributed network.

The executable boots into a credentialed authentication flow backed by **MySQL**, promotes legacy accounts to modern password hashing when necessary, and then loads per-user configuration before handing control to the main dashboard. Shared configuration such as database endpoints, API credentials, and scheduler settings is kept in a centralized **GlobalVariables** store and reused across forms via a singleton **MainControl** instance managed by **FormManager**.

---

## Architecture

### Windows Control Application
**Real-time resource monitoring:**  
The main dashboard wires timers and LiveCharts series to display CPU core load, RAM utilization trends, and disk capacity for the head node or any selected remote worker.

### Node Management
The console self-registers the head node, enumerates enabled worker records from `dragnet_nodes`, and materializes per-node tabs with module controls, resource scores, and connection metadata.

### Asset and Telemetry Editing
Administrators can edit the crypto asset catalog and arbitrary Dragnet tables directly via bound **DataGridView** instances with guarded save flows and auto-refresh support.

### Operational Telemetry & Remediation
The watchdog refresh loop loads curator/scanner/order-book/daemon module registries, colors rows based on heartbeat health, surfaces error logs, wipes control tables on demand, and can auto-restart unhealthy modules via scripted payloads.

### Automation Scheduler
An in-app scheduler persists Dragnet tasks, caches schedule rows, applies day-of-week and date masks, and dispatches scripts (e.g., asset list builders or checkbox-enabled modules) only once per minute per node.

### Command & Control
Local scripts run in new terminals, while remote launches and kills are executed over HTTP using `curl` requests against each node’s listener; the console can also issue bulk shutdowns.

### Continuous Monitoring
A watchdog timer refreshes dashboard data asynchronously to keep module status and telemetry current without blocking the UI thread.

---

## AI Configuration Workspace
The **AI** tab dynamically rebuilds controls for **Ollama-compatible large language models**, supports health checks, prompt testing, throughput measurement, and persists user-level settings back to MySQL and global state.

---

## Worker Nodes & Automation Scripts
Worker hosts run a **Flask listener** that accepts `/run` and `/kill` commands, resolves the correct executable or Python entry point per operating system, and safeguards which scripts may be launched or terminated.

Nodes keep Dragnet executables (e.g., scanners, daemons, asset builders) in the shipped `scripts/` directory or the companion **PythonScripts** source tree, which includes collectors, scrapers, and health utilities. Python environments rely on the bundled **requirements.txt** (Flask, SQLAlchemy, psutil, Selenium, etc.) to mirror the console’s expectations.  

The WinForms client communicates with those listeners using JSON payloads and `curl`, so remote hosts should expose **TCP port 5005** to the control plane.

---

## Configuration Dialogs & Utilities
Dedicated setup forms let operators adjust exchange connectors, curator paths, and messaging credentials directly against the `users` table.  
Each dialog updates the live **GlobalVariables** cache after persisting settings back to MySQL.  

These dialogs cover **Coinbase, Kraken, Binance, Telegram scanning, curator batch settings**, and more, complementing the in-dashboard editing tools.

---

## Installation

### Prerequisites
- Windows workstation or server with the **.NET 8 SDK / Visual Studio** (WinForms workload enabled)  
- Reachable **MySQL instances** for `users`, `dragnet`, `control`, `news`, and `assets` databases  
- **Python 3** on any worker node with access to install the listed packages (Flask, psutil, SQL drivers, Selenium, etc.)  
- `curl` available on the control host (included in Windows 10+)

---

### Clone and Restore
```bash
git clone <repo-url>
cd Dragnet
dotnet restore
