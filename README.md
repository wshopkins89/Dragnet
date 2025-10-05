# Dragnet Control
Overview
Dragnet Control is a Windows desktop operations console built on .NET 8 WinForms that orchestrates data-ingestion nodes, scanners, and analytics services across a distributed network. The executable boots into a credentialed authentication flow backed by MySQL, promotes legacy accounts to modern password hashing when necessary, and then loads per-user configuration before handing control to the main dashboard. Shared configuration such as database endpoints, API credentials, and scheduler settings is kept in a centralized GlobalVariables store and reused across forms via a singleton MainControl instance managed by FormManager.

Architecture
Windows control application
Real-time resource monitoring: The main dashboard wires timers and LiveCharts series to display CPU core load, RAM utilization trends, and disk capacity for the head node or any selected remote worker.

Node management: The console self-registers the head node, enumerates enabled worker records from dragnet_nodes, and materializes per-node tabs with module controls, resource scores, and connection metadata.

Asset and telemetry editing: Administrators can edit the crypto asset catalog and arbitrary Dragnet tables directly via bound DataGridView instances with guarded save flows and auto-refresh support.

Operational telemetry & remediation: The watchdog refresh loop loads curator/scanner/order-book/daemon module registries, colors rows based on heartbeat health, surfaces error logs, wipes control tables on demand, and can auto-restart unhealthy modules via scripted payloads.

Automation scheduler: An in-app scheduler persists dragnet tasks, caches schedule rows, applies day-of-week and date masks, and dispatches scripts (e.g., asset list builders or checkbox-enabled modules) only once per minute per node.

Command & control: Local scripts run in new terminals, while remote launches and kills are executed over HTTP using curl requests against each node’s listener; the console can also issue bulk shutdowns.

Continuous monitoring: A watchdog timer refreshes dashboard data asynchronously to keep module status and telemetry current without blocking the UI thread.

AI configuration workspace: The AI tab dynamically rebuilds controls for Ollama-compatible large language models, supports health checks, prompt testing, throughput measurement, and persists user-level settings back to MySQL and global state.

Worker nodes & automation scripts
Worker hosts run a Flask listener that accepts /run and /kill commands, resolves the correct executable or Python entry point per operating system, and safeguards which scripts may be launched or terminated. Nodes keep Dragnet executables (e.g., scanners, daemons, asset builders) in the shipped scripts directory or the companion PythonScripts source tree, which includes collectors, scrapers, and health utilities. Python environments rely on the bundled requirements file (Flask, SQLAlchemy, psutil, Selenium, etc.) to mirror the console’s expectations. The WinForms client talks to those listeners using JSON payloads and curl, so remote hosts should expose TCP port 5005 to the control plane.

Configuration dialogs & utilities
Dedicated setup forms let operators adjust exchange connectors, curator paths, and messaging credentials directly against the users table—each dialog updates the live GlobalVariables cache after persisting settings back to MySQL. These dialogs cover Coinbase, Kraken, Binance, Telegram scanning, curator batch settings, and more, complementing the in-dashboard editing tools.

Installation
Prerequisites
Windows workstation or server with the .NET 8 SDK / Visual Studio and WinForms workload enabled.

MySQL instances reachable for the user, Dragnet, control, news, and asset databases referenced throughout the configuration bootstrap.

Python 3 on any worker node plus access to install the listed packages (Flask, psutil, SQL drivers, Selenium, etc.).

curl available on the control host so remote launches succeed (Windows 10+ ships with it).

Clone and restore
git clone <repo-url>
cd Dragnet
dotnet restore
Configure databases
Provision the userdata schema and populate users rows with authentication data and all Dragnet connection columns consumed by the loading screen (database hosts, credentials, API keys, scheduler defaults, etc.).

Ensure the Dragnet control schema contains dragnet_nodes, dragnet_schedule, module registry tables, and the log tables referenced by the dashboard loaders.

Grant the application account read/write access to those schemas; the app builds its own connection strings from GlobalVariables.

Build and run the control client
dotnet build
dotnet run
Running starts the WinForms application, prompting for credentials before loading the main dashboard.

Prepare worker nodes
Copy the contents of PythonScripts/ and/or the prebuilt executables in scripts/ to the worker’s allowed script directory (C:\DragnetScripts on Windows or /opt/dragnet_scripts on Linux).

Install Python dependencies and start the Flask listener so it serves /run and /kill on port 5005.

Verify network reachability from the control workstation to each node’s listener endpoint; the dashboard will issue HTTP commands via curl when launching or stopping modules.

Launch Dragnet Control
Start the WinForms client, authenticate, and confirm the loading screen successfully pulls per-user configuration (status label progress will enumerate each category).

Use the node tabs to enable required modules, adjust asset ranges, and fire scripts; the dashboard will stream health metrics and logs through the watchdog refresh loop.

Employ the configuration dialogs (Coinbase/Kraken/Binance/Telegram/Curator) whenever credentials or runtime parameters need to change; changes persist immediately and update live global state.

Optional: schedule modules & tune AI
Populate the scheduler grid with recurring jobs for asset builders or module triggers; the automation service will execute them at the configured cadence across all enabled nodes.

Configure the AI tab to point at an Ollama-compatible host, run health checks, and persist prompt versions used by the PromptDaemon workflow.
