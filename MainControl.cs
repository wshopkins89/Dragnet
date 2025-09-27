
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.VisualBasic.Devices;
using MySqlConnector;
using Newtonsoft.Json;
using PinEntryControl;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;


namespace DragnetControl
{
    public partial class MainControl : Form
    {
        string username = GlobalVariables.username;
        string assetIP = GlobalVariables.assetIP;
        int accountstatus = GlobalVariables.accountstatus;
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer scheduleTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer watchdogTimer = new System.Windows.Forms.Timer();
        private CancellationTokenSource monitorTokenSource;


        private ColumnSeries<double> cpuSeries;
        private PerformanceCounter[] cpuCounters;
        private ObservableCollection<double> cpuUsage;
        private ObservableCollection<double> usedValues = new ObservableCollection<double> { 0 };
        private ObservableCollection<double> freeValues = new ObservableCollection<double> { 0 };
        private PieSeries<double> usedSeries;
        private PieSeries<double> freeSeries;
        private ObservableCollection<double> localRamHistory = new ObservableCollection<double>();
        private ObservableCollection<double> ramHistory = new ObservableCollection<double>();
        private LineSeries<double> ramSeries;
        private const int maxPoints = 60;
        private string StatusJson = "{}";
        private dynamic currentNodeStatus = null;
        DataTable cryptoassets = new DataTable();
        DataTable adt = new DataTable();
        private MySqlDataAdapter adapter;
        private MySqlDataAdapter dragnetAdapter;
        private DataTable dragnetDt;
        private MySqlCommandBuilder dragnetBuilder;
        private string CurrentTableName;
        private float newstimeframe = 3;
        private readonly HashSet<(string Script, string Ip, string MinuteKey)> firedThisMinute = new();
        private DateTime lastResetDate = DateTime.MinValue;
        private List<ScheduleRow> cachedSchedule = new();
        private DateTime scheduleCacheExpiry = DateTime.MinValue;
        private bool _eventsWired = false;
        private static readonly object _restartGuardLock = new object();
        private static readonly HashSet<string> _inFlight = new HashSet<string>();
        private static readonly Dictionary<string, DateTime> _lastStart = new Dictionary<string, DateTime>();
        private const int RestartThrottleSeconds = 10; // tweak to taste
        private bool _checkInProgress = false;
        public MainControl()
        {
            InitializeComponent();

            // Timer setup
            timer1.Interval = 100;
            timer1.Tick += Timer_Tick;


            timer2.Interval = 1000;
            timer2.Tick += Timer2_Tick;

            timer3.Interval = 10000;
            timer3.Tick += Timer3_Tick;


            drivepathlabel.Text = GlobalVariables.DragnetDBIP.ToString();

            // CPU
            cpuCounters = Enumerable.Range(0, Environment.ProcessorCount)
                .Select(i => new PerformanceCounter("Processor", "% Processor Time", i.ToString()))
                .ToArray();
            foreach (var c in cpuCounters) c.NextValue();
            System.Threading.Thread.Sleep(100);

            cpuUsage = new ObservableCollection<double>(Enumerable.Repeat(0.0, cpuCounters.Length));
            cpuSeries = ChartConstructor.BuildCpuSeries(cpuUsage);
            cartesianChart1.Series = new ISeries[] { cpuSeries };
            cartesianChart1.XAxes = ChartConstructor.BuildCpuXAxis(cpuUsage.Count);
            cartesianChart1.YAxes = ChartConstructor.BuildCpuYAxis();

            // Disk
            usedSeries = ChartConstructor.BuildDiskUsedSeries(usedValues);
            freeSeries = ChartConstructor.BuildDiskFreeSeries(freeValues);
            pieChart1.Series = new ISeries[] { usedSeries, freeSeries };
            UpdateDiskChartValues();

            // RAM
            localRamHistory = new ObservableCollection<double>(Enumerable.Repeat(0.0, maxPoints));
            ramSeries = ChartConstructor.BuildRamSeries(localRamHistory);
            ramChart.Series = new ISeries[] { ramSeries };
            ramChart.XAxes = ChartConstructor.BuildRamXAxis();
            ramChart.YAxes = ChartConstructor.BuildRamYAxis();

            LoadEnabledDragnetNodes();
            assetsDataGridView.RowHeadersVisible = false;
            assetsDataGridView.Font = new Font("Audiowide", 10);
            dragnetTablesDataGridView.RowHeadersVisible = false;
            dragnetTablesDataGridView.Font = new Font("Audiowide", 10);
            LoadCryptoAssetDatabase();
            LoadTableNamesIntoGrid();
            for (int i = 0; i < daysOfWeekChecklist.Items.Count; i++)
            {
                daysOfWeekChecklist.SetItemChecked(i, true);
            }
            RefreshScheduleGrid();
            scheduleGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            scheduleGridView.MultiSelect = false;
            WireAiConfigUi();
            LoadPromptListAsync();
            WireUpEventsOnce();
        }
        private sealed class PromptRow
        {
            public string Name { get; set; } = "";
            public string Version { get; set; } = "";
            public bool IsActive { get; set; }
            public string Display => $"{Name} ({Version})" + (IsActive ? "  [active]" : "");
        }

        private static string Sha256Hex(string s)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s ?? string.Empty));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        public class PromptListItem
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public bool IsActive { get; set; }

            public override string ToString()
            {
                // The text shown in the ListBox
                return $"{Name} (v{Version}){(IsActive ? " [Active]" : "")}";
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            GetCPUUsage();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            UpdateRamChartValues();
        }
        private void Timer3_Tick(object sender, EventArgs e)
        {
            UpdateDiskChartValues();
            UpdateRamChartValues();

        }

        private void GetCPUUsage()
        {
            string ip = ExtractIpFromTab(tabControl1.SelectedTab.Text);

            if (IsLocalIP(ip))
            {
                for (int i = 0; i < cpuCounters.Length; i++)
                {
                    cpuUsage[i] = (double)cpuCounters[i].NextValue();
                }
                drivepathlabel.Text = ip; // Could also use "localhost" or the local IP here
            }
            else
            {
                var selectedTab = tabControl1.SelectedTab;
                if (selectedTab == null) return;
                try
                {
                    GetNodeStatusFromDatabase(ip);
                    if (!string.IsNullOrWhiteSpace(StatusJson))
                    {
                        dynamic parsed = JsonConvert.DeserializeObject<dynamic>(StatusJson);
                        var cpuList = ((IEnumerable<dynamic>)parsed.cpu).Select(x => (double)x).ToArray();
                        for (int i = 0; i < cpuUsage.Count && i < cpuList.Length; i++)
                            cpuUsage[i] = cpuList[i];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CPU DB Fetch Error] {ex.Message}");
                }
                drivepathlabel.Text = ip;
            }
        }
        private void UpdateRamChartValues()
        {
            string ip = ExtractIpFromTab(tabControl1.SelectedTab.Text);

            if (IsLocalIP(ip))
            {
                ramSeries.Values = localRamHistory;
                var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
                ulong total = ci.TotalPhysicalMemory;
                ulong avail = ci.AvailablePhysicalMemory;
                double usedPercent = 100.0 * (total - avail) / total;

                if (localRamHistory.Count >= maxPoints)
                    localRamHistory.RemoveAt(0);

                localRamHistory.Add(usedPercent);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(StatusJson)) return;

                try
                {
                    ramSeries.Values = ramHistory;
                    dynamic parsed = JsonConvert.DeserializeObject<dynamic>(StatusJson);
                    double ramUsed = (double)parsed.ram.used;
                    double ramTotal = (double)parsed.ram.total;
                    double ramPercent = (ramUsed / ramTotal) * 100;

                    if (ramHistory.Count >= maxPoints)
                        ramHistory.RemoveAt(0);

                    ramHistory.Add(ramPercent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RAM JSON Parse Error] {ex.Message}");
                }
            }
        }
        private sealed class ScheduleRow
        {
            public string ScriptType { get; init; }
            public TimeSpan Trigger;                 // time-of-day (HH:mm)
            public string DaysMask;                  // "default" or 7-char 0/1 (Sun..Sat)
            public HashSet<string> CalendarDates;    // "yyyy-MM-dd" set
        }
        private static DateTime TruncateToMinute(DateTime dt)
    => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);

        private static string MinuteKey(DateTime dtUtc)
            => dtUtc.ToString("yyyyMMddHHmm"); // stable, no locale issues
        private void UpdateDiskChartValues()
        {
            string ip = ExtractIpFromTab(tabControl1.SelectedTab.Text);

            if (IsLocalIP(ip))
            {
                var (used, free, total) = GetDriveUsage("C");
                usedValues[0] = used;
                freeValues[0] = free;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(StatusJson)) return;

                try
                {
                    dynamic parsed = JsonConvert.DeserializeObject<dynamic>(StatusJson);
                    double diskUsed = (double)parsed.disk.used;
                    double diskTotal = (double)parsed.disk.total;
                    double diskFree = diskTotal - diskUsed;
                    usedValues[0] = diskUsed;
                    freeValues[0] = diskFree;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Disk JSON Parse Error] {ex.Message}");
                }
            }
        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear remote CPU history
            if (cpuUsage != null)
                for (int i = 0; i < cpuUsage.Count; i++)
                    cpuUsage[i] = 0.0;
            cartesianChart1.Series = Array.Empty<ISeries>();

            // Clear remote RAM history
            if (ramHistory != null)
                ramHistory.Clear();
            ramChart.Series = new ISeries[] { ramSeries };

            // Clear remote Disk chart
            if (usedValues != null && usedValues.Count > 0)
                usedValues[0] = 0.0;
            if (freeValues != null && freeValues.Count > 0)
                freeValues[0] = 0.0;
            pieChart1.Series = Array.Empty<ISeries>();
            GetCPUUsage();
            UpdateDiskChartValues();
            UpdateRamChartValues();
        }

        public static (int cpuScore, int ramScore) GetResourceScores()
        {
            // CPU scoring by logical processors
            int cpuCount = Environment.ProcessorCount;
            int cpuScore;
            if (cpuCount < 4)
                cpuScore = 1;
            else if (cpuCount < 8)
                cpuScore = 2;
            else
                cpuScore = 3;

            // RAM scoring by installed RAM (GB)
            var computerInfo = new ComputerInfo();
            double ramTotalGB = computerInfo.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0;
            int ramScore;
            if (ramTotalGB < 4)
                ramScore = 1;
            else if (ramTotalGB < 16)
                ramScore = 2;
            else
                ramScore = 3;

            return (cpuScore, ramScore);
        }

        public (double used, double free, double total) GetDriveUsage(string driveLetter)
        {
            var drive = new DriveInfo(driveLetter);
            double total = drive.TotalSize / 1e9;
            double free = drive.AvailableFreeSpace / 1e9;
            double used = total - free;
            return (used, free, total);
        }
        public string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName();
            var ip = Dns.GetHostAddresses(hostName)
                .FirstOrDefault(addr =>
                    addr.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(addr));
            return ip?.ToString() ?? "127.0.0.1";
        }

        private void Setup_Host_Node()
        {
            // -- Setup the head node values --
            string ip = "localhost";  // or use GetLocalIPv4() for your LAN IP
            string hostname = System.Environment.MachineName;
            int port = 3306;
            string username = GlobalVariables.DragnetDBUser;
            string password = GlobalVariables.DragnetDBPassword;
            string note = "Control head";
            (int cpuscore, int ramscore) = GetResourceScores();
            bool enabled = true;

            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();

                // Check if a node with this IP exists
                string checkSql = "SELECT COUNT(*) FROM dragnet_nodes WHERE ip_address = @ip";
                int nodeCount = 0;
                using (var cmd = new MySqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    nodeCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (nodeCount == 0)
                {
                    // Insert new node for this IP
                    string insertSql = @"INSERT INTO dragnet_nodes 
            (hostname, ip_address, note, port, username, password, cpu_score, ram_score, enabled) 
            VALUES (@hostname, @ip, @note, @port, @username, @password, @cpu, @ram, @enabled)";
                    using (var cmd = new MySqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@hostname", hostname);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.Parameters.AddWithValue("@port", port);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@cpu", cpuscore);
                        cmd.Parameters.AddWithValue("@ram", ramscore);
                        cmd.Parameters.AddWithValue("@enabled", enabled ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Update existing node for this IP
                    string updateSql = @"UPDATE dragnet_nodes SET
                hostname = @hostname,
                note = @note,
                port = @port,
                username = @username,
                password = @password,
                cpu_score = @cpu,
                ram_score = @ram,
                enabled = @enabled
            WHERE ip_address = @ip";
                    using (var cmd = new MySqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@hostname", hostname);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.Parameters.AddWithValue("@port", port);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@cpu", cpuscore);
                        cmd.Parameters.AddWithValue("@ram", ramscore);
                        cmd.Parameters.AddWithValue("@enabled", enabled ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            timer1.Start();
            timer2.Start();
            timer3.Start();
        }
        private void LoadEnabledDragnetNodes()
        {
            Setup_Host_Node();
            tabControl1.TabPages.Clear();

            using var conn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM dragnet_nodes WHERE enabled = 1;";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string hostname = reader.IsDBNull(reader.GetOrdinal("hostname")) ? "" : reader.GetString("hostname");
                    if (string.IsNullOrWhiteSpace(hostname)) hostname = "Unknown";
                    string ip = reader.IsDBNull(reader.GetOrdinal("ip_address")) ? "" : reader.GetString("ip_address");
                    string note = reader.IsDBNull(reader.GetOrdinal("note")) ? "" : reader.GetString("note");
                    int port = reader.IsDBNull(reader.GetOrdinal("port")) ? 0 : reader.GetInt32("port");
                    string username = reader.IsDBNull(reader.GetOrdinal("username")) ? "" : reader.GetString("username");
                    string password = reader.IsDBNull(reader.GetOrdinal("password")) ? "" : reader.GetString("password");
                    int cpuScore = reader.IsDBNull(reader.GetOrdinal("cpu_score")) ? 0 : reader.GetInt32("cpu_score");
                    int ramScore = reader.IsDBNull(reader.GetOrdinal("ram_score")) ? 0 : reader.GetInt32("ram_score");
                    bool enabled = reader.IsDBNull(reader.GetOrdinal("enabled")) ? false : reader.GetBoolean("enabled");

                    AddNodeTab(hostname, ip, note, port, username, password, cpuScore, ramScore, enabled);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Dragnet nodes: {ex.Message}");
            }
        }
        private void AddNodeTab(string hostname, string ip, string note, int port, string username, string password, int cpuScore, int ramScore, bool enabled)
        {
            string label = string.IsNullOrWhiteSpace(hostname) || hostname == "Unknown"
                ? $"Node ({ip})"
                : $"{hostname} ({ip})";

            TabPage nodeTab = new TabPage(label)
            {
                ToolTipText = note,
                BackColor = Color.Gray
            };

            // --- Add large hostname label ---
            string displayHost = string.IsNullOrWhiteSpace(hostname) || hostname == "Unknown" ? ip : hostname;
            var hostnameLabel = new Label()
            {
                Text = displayHost,
                Location = new Point(0, 0),
                Width = 340,
                Height = 32,
                Font = new Font("Audiowide", 18F, FontStyle.Bold), // Fallback handled below
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            int y = 30;

            var IPLabel = new Label()
            {
                Text = ip,
                Location = new Point(0, y),
                Width = 340,
                Height = 32,
                Font = new Font("Audiowide", 18F, FontStyle.Bold), // Fallback handled below
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            y += 30;
            // If Audiowide isn't installed, fallback to Segoe UI or Arial
            if (!FontFamily.Families.Any(f => f.Name == "Audiowide"))
                hostnameLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);

            nodeTab.Controls.Add(hostnameLabel);
            nodeTab.Controls.Add(IPLabel);

            var usernameLabel = new Label() { Text = "Username:", Location = new Point(10, y), ForeColor = Color.White };
            var usernameBox = new TextBox() { Text = username, Location = new Point(109, y), Width = 100 };
            y += 30;

            var passwordLabel = new Label() { Text = "Password:", Location = new Point(10, y), ForeColor = Color.White };
            var passwordBox = new TextBox() { Text = password, Location = new Point(109, y), Width = 100, UseSystemPasswordChar = true };
            y += 30;

            var portLabel = new Label() { Text = "Port:", Location = new Point(10, y), ForeColor = Color.White };
            var portBox = new TextBox() { Text = port.ToString(), Location = new Point(109, y), Width = 60 };
            y += 30;

            var noteLabel = new Label() { Text = "Note:", Location = new Point(10, y), ForeColor = Color.White };
            var noteBox = new TextBox() { Text = note, Location = new Point(109, y), Width = 150 };
            y += 30;

            var cpuScoreLabel = new Label() { Text = "CPU Score:", Location = new Point(10, y), ForeColor = Color.White };
            var cpuScoreBox = new ComboBox() { Location = new Point(109, y), Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
            cpuScoreBox.Items.AddRange(new object[] { "1", "2", "3" });
            cpuScoreBox.SelectedItem = cpuScore.ToString();
            y += 30;

            var ramScoreLabel = new Label() { Text = "RAM Score:", Location = new Point(10, y), ForeColor = Color.White };
            var ramScoreBox = new ComboBox() { Location = new Point(109, y), Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
            ramScoreBox.Items.AddRange(new object[] { "1", "2", "3" });
            ramScoreBox.SelectedItem = ramScore.ToString();
            y += 30;

            var enabledBox = new System.Windows.Forms.CheckBox() { Text = "Enabled", Location = new Point(10, y), Checked = enabled, ForeColor = Color.White };
            y += 30;

            var saveButton = new Button() { Text = "Save", BackColor = DefaultBackColor, Location = new Point(110, y) };
            y += 40;

            saveButton.Click += (sender, e) =>
            {
                string newUsername = usernameBox.Text;
                string newPassword = passwordBox.Text;
                int newPort = int.TryParse(portBox.Text, out var p) ? p : 0;
                string newNote = noteBox.Text;
                int newCpuScore = int.TryParse(cpuScoreBox.SelectedItem?.ToString(), out var c) ? c : 0;
                int newRamScore = int.TryParse(ramScoreBox.SelectedItem?.ToString(), out var r) ? r : 0;
                bool newEnabled = enabledBox.Checked;

                using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string sql = @"UPDATE dragnet_nodes SET
                             username = @username,
                             password = @password,
                             port = @port,
                             enabled = @enabled,
                             cpu_score = @cpu_score,
                             ram_score = @ram_score,
                             note = @note
                           WHERE ip = @ip";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", newUsername);
                        cmd.Parameters.AddWithValue("@password", newPassword);
                        cmd.Parameters.AddWithValue("@port", newPort);
                        cmd.Parameters.AddWithValue("@enabled", newEnabled ? 1 : 0);
                        cmd.Parameters.AddWithValue("@cpu_score", newCpuScore);
                        cmd.Parameters.AddWithValue("@ram_score", newRamScore);
                        cmd.Parameters.AddWithValue("@note", newNote);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Node updated!", "Success");
            };

            nodeTab.Controls.Add(usernameLabel);
            nodeTab.Controls.Add(usernameBox);
            nodeTab.Controls.Add(passwordLabel);
            nodeTab.Controls.Add(passwordBox);
            nodeTab.Controls.Add(portLabel);
            nodeTab.Controls.Add(portBox);
            nodeTab.Controls.Add(noteLabel);
            nodeTab.Controls.Add(noteBox);
            nodeTab.Controls.Add(cpuScoreLabel);
            nodeTab.Controls.Add(cpuScoreBox);
            nodeTab.Controls.Add(ramScoreLabel);
            nodeTab.Controls.Add(ramScoreBox);
            nodeTab.Controls.Add(enabledBox);
            nodeTab.Controls.Add(saveButton);

            var SettingsLabel = new Label()
            {
                Text = "Module Settings",
                Location = new Point(0, y),
                Width = 340,
                Height = 32,
                Font = new Font("Audiowide", 18F, FontStyle.Bold), // Fallback handled below
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            // If Audiowide isn't installed, fallback to Segoe UI or Arial
            if (!FontFamily.Families.Any(f => f.Name == "Audiowide"))
                SettingsLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            nodeTab.Controls.Add(SettingsLabel);
            y += 37;
            var ScannerIDTextBox = new System.Windows.Forms.TextBox() { Name = "ScannerIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var ScannerBox = new System.Windows.Forms.CheckBox() { Text = "Crypto Scanner", Location = new Point(10, y), Checked = enabled, ForeColor = Color.White, AutoSize = true };
            var ScannerIDlabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };

            y += 30;
            var ScannerBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "ScannerBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var ScannerBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "ScannerBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var ScannerBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var ScannerBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };
            nodeTab.Controls.Add(ScannerBox);
            nodeTab.Controls.Add(ScannerIDlabel);
            nodeTab.Controls.Add(ScannerIDTextBox);
            nodeTab.Controls.Add(ScannerBlockStartLabel);
            nodeTab.Controls.Add(ScannerBlockStartTextBox);
            nodeTab.Controls.Add(ScannerBlockEndLabel);
            nodeTab.Controls.Add(ScannerBlockEndTextBox);
            y += 27;
            var OBScannerIDTextBox = new System.Windows.Forms.TextBox() { Name = "OBScannerIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var OBScannerBox = new System.Windows.Forms.CheckBox() { Text = "Orderbook Scanner", Location = new Point(10, y), Checked = enabled, ForeColor = Color.White, AutoSize = true };
            var OBScannerIDlabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };
            y += 30;
            var OBScannerBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "OBScannerBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var OBScannerBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "OBScannerBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var OBScannerBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var OBScannerBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };
            nodeTab.Controls.Add(OBScannerBox);
            nodeTab.Controls.Add(OBScannerIDlabel);
            nodeTab.Controls.Add(OBScannerIDTextBox);
            nodeTab.Controls.Add(OBScannerBlockStartLabel);
            nodeTab.Controls.Add(OBScannerBlockStartTextBox);
            nodeTab.Controls.Add(OBScannerBlockEndLabel);
            nodeTab.Controls.Add(OBScannerBlockEndTextBox);
            y += 27;
            var CuratorIDTextBox = new System.Windows.Forms.TextBox() { Name = "CuratorIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var CuratorBox = new System.Windows.Forms.CheckBox() { Text = "Curator Enabled", Location = new Point(10, y), Checked = enabled, ForeColor = Color.White, AutoSize = true };
            var CuratorIDlabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };
            y += 27;
            var workersTextBox = new System.Windows.Forms.TextBox() { Name = "CuratorWorkersTextBox", Text = "", Location = new Point(150, y), Width = 20 };
            var batchsizeTextBox = new System.Windows.Forms.TextBox() { Name = "CuratorBatchSizeTextBox", Text = "", Location = new Point(280, y), Width = 20 };
            y += 3;
            var workersLabel = new System.Windows.Forms.Label() { Text = "Concurrent Workers", Location = new Point(10, y), ForeColor = Color.White, AutoSize = true };
            var batchsizeLabel = new System.Windows.Forms.Label() { Text = "Batch Size", Location = new Point(200, y), ForeColor = Color.White, AutoSize = true };
            y += 27;
            var CuratorBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "CuratorBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var CuratorBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "CuratorBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var CuratorBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var CuratorBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };
            nodeTab.Controls.Add(CuratorBox);
            nodeTab.Controls.Add(CuratorIDlabel);
            nodeTab.Controls.Add(CuratorIDTextBox);
            nodeTab.Controls.Add(workersLabel);
            nodeTab.Controls.Add(batchsizeLabel);
            nodeTab.Controls.Add(workersTextBox);
            nodeTab.Controls.Add(batchsizeTextBox);
            nodeTab.Controls.Add(CuratorBlockStartLabel);
            nodeTab.Controls.Add(CuratorBlockStartTextBox);
            nodeTab.Controls.Add(CuratorBlockEndLabel);
            nodeTab.Controls.Add(CuratorBlockEndTextBox);
            y += 27;
            var NewsScraperIDTextBox = new System.Windows.Forms.TextBox() { Name = "NewsScraperIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var NewsScraperBox = new System.Windows.Forms.CheckBox() { Name = "NewsScraperBox", Text = "News Scraper", Location = new Point(10, y), Checked = false, ForeColor = Color.White, AutoSize = true };
            var NewsScraperlabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };
            y += 30;
            var NewsScraperBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "NewsScraperBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var NewsScraperBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "NewsScraperBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var NewsScraperBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var NewsScraperBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };
            nodeTab.Controls.Add(NewsScraperBox);
            nodeTab.Controls.Add(NewsScraperlabel);
            nodeTab.Controls.Add(NewsScraperIDTextBox);
            nodeTab.Controls.Add(NewsScraperBlockStartLabel);
            nodeTab.Controls.Add(NewsScraperBlockStartTextBox);
            nodeTab.Controls.Add(NewsScraperBlockEndLabel);
            nodeTab.Controls.Add(NewsScraperBlockEndTextBox);
            y += 27;
            var TrendsScraperIDTextBox = new System.Windows.Forms.TextBox() { Name = "TrendsScraperIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var TrendsScraperBox = new System.Windows.Forms.CheckBox() { Text = "Trends Scraper", Location = new Point(10, y), Checked = false, ForeColor = Color.White, AutoSize = true };
            var TrendsScraperlabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };
            y += 30;
            var TrendsScraperBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "TrendsScraperBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var TrendsScraperBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "TrendsSCraperBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var TrendsScraperBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var TrendsScraperBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };
            nodeTab.Controls.Add(TrendsScraperBox);
            nodeTab.Controls.Add(TrendsScraperlabel);
            nodeTab.Controls.Add(TrendsScraperIDTextBox);
            nodeTab.Controls.Add(TrendsScraperBlockStartLabel);
            nodeTab.Controls.Add(TrendsScraperBlockStartTextBox);
            nodeTab.Controls.Add(TrendsScraperBlockEndLabel);
            nodeTab.Controls.Add(TrendsScraperBlockEndTextBox);

            y += 27;
            var TelegramScannerIDTextBox = new System.Windows.Forms.TextBox() { Name = "TelegramScannerIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var TelegramScannerBox = new System.Windows.Forms.CheckBox() { Text = "Telegram Scanner", Location = new Point(10, y), Checked = false, ForeColor = Color.White, AutoSize = true };
            var TelegramScannerLabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };
            y += 30;
            var TelegramScannerBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "TelegramScannerBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var TelegramScannerBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "TelegramScannerBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var TelegramScannerBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var TelegramScannerBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };

            nodeTab.Controls.Add(TelegramScannerLabel);
            nodeTab.Controls.Add(TelegramScannerBlockStartLabel);
            nodeTab.Controls.Add(TelegramScannerBlockEndLabel);
            nodeTab.Controls.Add(TelegramScannerBox);
            nodeTab.Controls.Add(TelegramScannerIDTextBox);
            nodeTab.Controls.Add(TelegramScannerBlockStartTextBox);
            nodeTab.Controls.Add(TelegramScannerBlockEndTextBox);

            y += 27;
            var RetroScannerIDTextBox = new System.Windows.Forms.TextBox() { Name = "RetroScannerIDTextBox", Text = "", Enabled = false, Location = new Point(260, y), Width = 100 };
            y += 3;
            var RetroScannerBox = new System.Windows.Forms.CheckBox() { Text = "Historical Scanner", Location = new Point(10, y), Checked = false, ForeColor = Color.White, AutoSize = true };
            var RetroScannerLabel = new Label() { Text = "Module ID:", Location = new Point(160, y), ForeColor = Color.White };
            y += 30;
            var RetroScannerBlockStartTextBox = new System.Windows.Forms.TextBox() { Name = "RetroScannerBlockStartTextBox", Text = "", Location = new Point(120, y), Width = 50 };
            var RetroScannerBlockEndTextBox = new System.Windows.Forms.TextBox() { Name = "RetroScannerBlockEndTextBox", Text = "", Location = new Point(290, y), Width = 50 };
            y += 3;
            var RetroScannerBlockStartLabel = new System.Windows.Forms.Label() { Text = "Start at Block:", Location = new Point(10, y), ForeColor = Color.White };
            var RetroScannerBlockEndLabel = new System.Windows.Forms.Label() { Text = "End at Block:", Location = new Point(190, y), ForeColor = Color.White };
            y += 30;
            var RetroScannerStartDateTextBox = new System.Windows.Forms.MaskedTextBox() { Name = "RetroScannerStartDateTextBox", Location = new Point(90, y), Width = 100, Mask = "00/00/0000", Text = DateTime.Now.ToString("MM/dd/yyyy") };
            var RetroScannerEndDateTextBox = new System.Windows.Forms.MaskedTextBox() { Name = "RetroScannerEndDateTextBox", Text = "", Location = new Point(270, y), Width = 100, Mask = "00/00/0000" };
            y += 3;
            var RetroScannerStartDateLabel = new System.Windows.Forms.Label() { Text = "Start Date:", Location = new Point(10, y), ForeColor = Color.White };
            var RetroScannerEndDateLabel = new System.Windows.Forms.Label() { Text = "End Date:", Location = new Point(200, y), ForeColor = Color.White };
            y += 30;
            var NewsDaemonCheckBox = new System.Windows.Forms.CheckBox() { Text = "News Daemon", Location = new Point(10, y), Checked = false, ForeColor = Color.White, AutoSize = true };

            nodeTab.Controls.Add(RetroScannerLabel);
            nodeTab.Controls.Add(RetroScannerBlockStartLabel);
            nodeTab.Controls.Add(RetroScannerBlockEndLabel);
            nodeTab.Controls.Add(RetroScannerStartDateLabel);
            nodeTab.Controls.Add(RetroScannerEndDateLabel);
            nodeTab.Controls.Add(RetroScannerBox);
            nodeTab.Controls.Add(RetroScannerIDTextBox);
            nodeTab.Controls.Add(RetroScannerBlockStartTextBox);
            nodeTab.Controls.Add(RetroScannerBlockEndTextBox);
            nodeTab.Controls.Add(RetroScannerStartDateTextBox);
            nodeTab.Controls.Add(RetroScannerEndDateTextBox);
            RetroScannerStartDateTextBox.BringToFront();
            RetroScannerEndDateTextBox.BringToFront();
            nodeTab.Controls.Add(NewsDaemonCheckBox);

            tabControl1.TabPages.Add(nodeTab);
        }

        private void GetNodeStatusFromDatabase(string ip)
        {
            string query = "SELECT status FROM dragnet_nodes WHERE ip_address = @ip";

            using var conn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ip", ip); // <-- Parameter name matches query

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                StatusJson = reader.IsDBNull(0) ? "{}" : reader.GetString(0);
            }
        }
        private void LoadCryptoAssetDatabase()
        {
            var conn = new MySqlConnection(GlobalVariables.AssetDBConnect);
            string query = "SELECT * FROM assets.crypto";
            adapter = new MySqlDataAdapter(query, conn);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
            adt = new DataTable(); // make sure you're resetting it
            conn.Open();
            adapter.Fill(adt); // use the same adapter that builder is bound to
            conn.Close();
            adt.RowChanged += DataTable_Changed;
            adt.RowDeleted += DataTable_Changed;
            adt.RowChanging += DataTable_Changed;
            adt.TableNewRow += DataTable_Changed;

            assetsSaveButton.Enabled = false;
            assetsDataGridView.DataSource = adt;

            if (assetsDataGridView.Columns.Contains("cryptocol"))
                assetsDataGridView.Columns["cryptocol"].Visible = false;
        }

        private void assetsSaveButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "Are you sure you want to save all changes to the database?\nThis action cannot be undone.",
                "Confirm Save",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    adapter.Update(adt);
                    MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCryptoAssetDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving changes:\n{ex.Message}", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Save canceled. No changes were committed.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string ExtractIpFromTab(string tabLabel)
        {
            int start = tabLabel.IndexOf('(');
            int end = tabLabel.IndexOf(')');
            if (start >= 0 && end > start)
                return tabLabel.Substring(start + 1, end - start - 1).Trim();
            return "";
        }
        private void DataTable_Changed(object sender, EventArgs e)
        {
            // Enable save button only if there are pending changes
            assetsSaveButton.Enabled = adt.GetChanges() != null;
        }
        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            string filterText = assetsSearchBox.Text.Replace("'", "''"); // prevent SQL-like filter errors

            if (string.IsNullOrWhiteSpace(filterText))
            {
                (assetsDataGridView.DataSource as DataTable).DefaultView.RowFilter = "";
            }
            else
            {
                // This will search all columns
                string filter = string.Join(" OR ",
                    (assetsDataGridView.DataSource as DataTable).Columns
                    .Cast<DataColumn>()
                    .Where(c => c.DataType == typeof(string) || c.DataType == typeof(int) || c.DataType == typeof(double) || c.DataType == typeof(float))
                    .Select(c => $"CONVERT([{c.ColumnName}], 'System.String') LIKE '%{filterText}%'"));

                (assetsDataGridView.DataSource as DataTable).DefaultView.RowFilter = filter;
            }
        }
        private void regenDatabasesButton_Click(object sender, EventArgs e)
        {
            if (coinbaseRegenCheckBox.Checked)
            {
                string payload = $"{{\\\"script\\\":\\\"CoinbaseBuildAssetList.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{GlobalVariables.assetDBName}\\\",\\\"crypto\\\"]}}";
                FireOffCommand("localhost", payload);
            }
            Thread.Sleep(2000);
            if (krakenRegenCheckBox.Checked)
            {
                string payload = $"{{\\\"script\\\":\\\"KrakenBuildAssetList.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{GlobalVariables.assetDBName}\\\",\\\"crypto\\\"]}}";
                FireOffCommand("localhost", payload);
            }
            Thread.Sleep(2000);
            if (binanceRegenCheckBox.Checked)
            {
                string payload = $"{{\\\"script\\\":\\\"BinanceBuildAssetList.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{GlobalVariables.assetDBName}\\\",\\\"crypto\\\"]}}";
                FireOffCommand("localhost", payload);
            }
        }
        private void LoadTableNamesIntoGrid()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.DragnetDBConnect))
                {
                    conn.Open();
                    string query = "SHOW TABLES;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Optional: rename the default column header
                dt.Columns[0].ColumnName = "Asset";
                dragnetTablesDataGridView.DataSource = dt;
                dragnetTablesDataGridView.Columns["Asset"].Width = 200;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }
        }
        private void rootDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DragnetDBEdit dragnetDBEdit = new DragnetDBEdit();
            dragnetDBEdit.Show();
        }

        private void coinbaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CoinbaseScannerSetup coinbaseScannerSetup = new CoinbaseScannerSetup();
            coinbaseScannerSetup.Show();
        }

        private void krakenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KrakenScannerSetup krakenScannerSetup = new KrakenScannerSetup();
            krakenScannerSetup.Show();
        }

        private void binanceUSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinanceScannerSetup binanceScannerSetup = new BinanceScannerSetup();
            binanceScannerSetup.Show();
        }

        private void etradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TelegramScannerSetup etradeScannerSetup = new TelegramScannerSetup();
            etradeScannerSetup.Show();
        }

        private void assetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetDBEdit assetDBEdit = new AssetDBEdit();
            assetDBEdit.Show();
        }
        private void addNodeButton_Click(object sender, EventArgs e)
        {
            using (var addNodeForm = new AddNode())
            {
                addNodeForm.NodeAdded += (s, e2) =>
                {
                    LoadEnabledDragnetNodes();
                };
                addNodeForm.ShowDialog();
            }
        }
        private void autoDelegateButton_Click(object sender, EventArgs e)
        {
            WipeDragnetControlTables();
            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT ip_address, username, password, port, enabled FROM dragnet_nodes", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ip = reader.GetString("ip_address");
                        string username = reader.GetString("username");
                        string password = reader.GetString("password");

                        // FIXED KILL PAYLOAD
                        string killPayload = $"{{\\\"process\\\":\\\"sysmon.exe\\\"}}";
                        string jsonPayload = $"{{\\\"script\\\":\\\"sysmon.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\".5\\\"]}}";
                        string killArgs = $"-X POST -H \"Content-Type: application/json\" -d \"{killPayload}\" http://{ip}:5005/kill";
                        string curlArgs = $"-X POST -H \"Content-Type: application/json\" -d \"{jsonPayload}\" http://{ip}:5005/run";
                        try
                        {
                            ProcessStartInfo psi = new ProcessStartInfo("curl", killArgs)
                            {
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            using (var proc = Process.Start(psi))
                            {
                                proc.StandardOutput.ReadToEnd();
                                proc.StandardError.ReadToEnd();
                                proc.WaitForExit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[EXCEPTION]: {ex.Message}");
                        }

                        // Optional: Give node a brief moment to kill before launching new process
                        System.Threading.Thread.Sleep(3000);

                        try
                        {
                            ProcessStartInfo psi = new ProcessStartInfo("curl", curlArgs)
                            {
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            using (var proc = Process.Start(psi))
                            {
                                proc.StandardOutput.ReadToEnd();
                                proc.StandardError.ReadToEnd();
                                proc.WaitForExit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[EXCEPTION]: {ex.Message}");
                        }
                    }
                }
            }
            System.Threading.Thread.Sleep(3000);
            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT ip_address, hostname, cpu_score, ram_score FROM dragnet_nodes", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ip = reader.IsDBNull(0) ? "" : reader.GetString(0);
                        string hostname = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        int cpuScore = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        int ramScore = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);

                        // Find the TabPage by IP address in the label
                        TabPage tab = tabControl1.TabPages
                            .Cast<TabPage>()
                            .FirstOrDefault(t => t.Text.EndsWith($"({ip})"));

                        if (tab != null)
                        {
                            // Compute the new tab text (just like AddNodeTab does)
                            string newTabLabel = string.IsNullOrWhiteSpace(hostname) || hostname == "Unknown"
                                ? $"Node ({ip})"
                                : $"{hostname} ({ip})";
                            tab.Text = newTabLabel;

                            // Update hostname label (always at (0,0))
                            var hostnameLabel = tab.Controls.OfType<Label>().FirstOrDefault(l => l.Location == new Point(0, 0));
                            if (hostnameLabel != null)
                                hostnameLabel.Text = string.IsNullOrWhiteSpace(hostname) || hostname == "Unknown" ? ip : hostname;

                            // Update CPU score ComboBox
                            var cpuLabel = tab.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Text == "CPU Score:");
                            if (cpuLabel != null)
                            {
                                var cpuBox = tab.Controls.OfType<ComboBox>().FirstOrDefault(cb => Math.Abs(cb.Location.Y - cpuLabel.Location.Y) < 10);
                                if (cpuBox != null && cpuScore > 0)
                                    cpuBox.SelectedItem = cpuScore.ToString();
                            }

                            // Update RAM score ComboBox
                            var ramLabel = tab.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Text == "RAM Score:");
                            if (ramLabel != null)
                            {
                                var ramBox = tab.Controls.OfType<ComboBox>().FirstOrDefault(cb => Math.Abs(cb.Location.Y - ramLabel.Location.Y) < 10);
                                if (ramBox != null && ramScore > 0)
                                    ramBox.SelectedItem = ramScore.ToString();
                            }
                            var curatorEnabled = tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Curator Enabled" && cb.Checked);
                            if (curatorEnabled)
                            {
                                // Calculate params
                                var (concurrentWorkers, batchSize) = CalculateCuratorParams(cpuScore, ramScore);

                                // Set to appropriate TextBoxes
                                var workersBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorWorkersTextBox");
                                if (workersBox != null)
                                    workersBox.Text = concurrentWorkers.ToString();

                                var batchBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorBatchSizeTextBox");
                                if (batchBox != null)
                                    batchBox.Text = batchSize.ToString();
                            }
                        }
                    }
                }
            }
            // Step 2: Retrieve all assets
            List<string> assetNames = new List<string>();
            using (var assetConn = new MySqlConnection(GlobalVariables.AssetDBConnect))
            {
                assetConn.Open();
                using var assetCmd = new MySqlCommand("SELECT name FROM crypto ORDER BY name ASC;", assetConn);
                using var assetReader = assetCmd.ExecuteReader();
                while (assetReader.Read())
                    assetNames.Add(assetReader.GetString("name").ToUpper());
            }

            // Step 3: Identify active nodes per module
            var activeNodes = new List<TabPage>();
            foreach (TabPage tab in tabControl1.TabPages)
                activeNodes.Add(tab);

            // Step 3: Identify active tabs for each module based on current UI checkbox state
            var scannerTabs = tabControl1.TabPages.Cast<TabPage>()
                .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Crypto Scanner" && cb.Checked)).ToList();

            var obScannerTabs = tabControl1.TabPages.Cast<TabPage>()
                .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Orderbook Scanner" && cb.Checked)).ToList();

            var curatorTabs = tabControl1.TabPages.Cast<TabPage>()
                .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Curator Enabled" && cb.Checked)).ToList();

            var TelegramScannerTabs = tabControl1.TabPages.Cast<TabPage>()
               .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Telegram Scanner" && cb.Checked)).ToList();

            var NewsScraperTabs = tabControl1.TabPages.Cast<TabPage>()
                .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "News Scraper" && cb.Checked)).ToList();

            var TrendsScraperTabs = tabControl1.TabPages.Cast<TabPage>()
                .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Trends Scraper" && cb.Checked)).ToList();

            var HistoricalScannerTabs = tabControl1.TabPages.Cast<TabPage>()
                .Where(tab => tab.Controls.OfType<System.Windows.Forms.CheckBox>().Any(cb => cb.Text == "Historical Scanner" && cb.Checked)).ToList();

            int activeCryptoScanners = scannerTabs.Count; // Crypto Scanner tabs
            int activeOBScanners = obScannerTabs.Count;   // Orderbook Scanner tabs
            int activeTelegramTabs = TelegramScannerTabs.Count; // Telegram Scraper tabs
            int activeNewsScrapers = NewsScraperTabs.Count; // News Scraper tabs
            int activeTrendsScrapers = TrendsScraperTabs.Count; // Trends Scraper tabs
            int capitoltradescount = HistoricalScannerTabs.Count; // historical scanner tabs

            // --- Calculate delays (QPS = queries per second; Coinbase = 10 QPS hard limit) ---
            double coinbaseQps = 10.0;
            double safety = 0.9;

            // For Crypto Scanner
            double cryptoDelay = 1.0 / ((coinbaseQps * safety) / Math.Max(1, activeCryptoScanners));

            // --- Update users table (example: set for current/active user, or for all) ---
            using (var userConn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                userConn.Open();
                // Update CryptoDelay
                var updateCrypto = new MySqlCommand("UPDATE users SET CryptoDelay=@delay WHERE username=@username", userConn);
                updateCrypto.Parameters.AddWithValue("@delay", cryptoDelay);
                updateCrypto.Parameters.AddWithValue("@username", GlobalVariables.username);
                updateCrypto.ExecuteNonQuery();
                GlobalVariables.CryptoDelay = (float)cryptoDelay;
            }

            // Helper function to split asset list
            List<(string start, string end)> ComputeAssetBlocks(List<string> assets, int partitions)
            {
                var blocks = new List<(string, string)>();
                if (partitions <= 0) return blocks;

                int blockSize = assets.Count / partitions;
                int remainder = assets.Count % partitions;
                int currentIndex = 0;

                for (int i = 0; i < partitions; i++)
                {
                    int currentBlockSize = blockSize + (i < remainder ? 1 : 0);
                    string start = assets[currentIndex];
                    string end = assets[Math.Min(currentIndex + currentBlockSize - 1, assets.Count - 1)];
                    blocks.Add((start, end));
                    currentIndex += currentBlockSize;
                }
                return blocks;
            }

            // Step 4: Compute blocks
            var scannerBlocks = ComputeAssetBlocks(assetNames, scannerTabs.Count);
            var obScannerBlocks = ComputeAssetBlocks(assetNames, obScannerTabs.Count);
            var curatorBlocks = ComputeAssetBlocks(assetNames, curatorTabs.Count);
            var telegramBlocks = ComputeAssetBlocks(assetNames, TelegramScannerTabs.Count);
            var newsScraperBlocks = ComputeAssetBlocks(assetNames, NewsScraperTabs.Count);
            var trendsScraperBlocks = ComputeAssetBlocks(assetNames, TrendsScraperTabs.Count);
            var HistoricalScannerBlocks = ComputeAssetBlocks(assetNames, HistoricalScannerTabs.Count);

            // Step 5: Populate TextBoxes automatically
            void SetBlockRange(TabPage tab, string module, (string start, string end) block)
            {
                var startLabel = tab.Controls.OfType<Label>().First(lbl => lbl.Text == "Start at Block:" && lbl.Location.Y > tab.Controls.OfType<System.Windows.Forms.CheckBox>().First(cb => cb.Text == module).Location.Y);
                var endLabel = tab.Controls.OfType<Label>().First(lbl => lbl.Text == "End at Block:" && lbl.Location.Y > tab.Controls.OfType<System.Windows.Forms.CheckBox>().First(cb => cb.Text == module).Location.Y);

                var startBox = tab.Controls.OfType<TextBox>().First(tb => tb.Location.X == startLabel.Location.X + 110 && Math.Abs(tb.Location.Y - startLabel.Location.Y) < 10);
                var endBox = tab.Controls.OfType<TextBox>().First(tb => tb.Location.X == endLabel.Location.X + 100 && Math.Abs(tb.Location.Y - endLabel.Location.Y) < 10);

                startBox.Text = block.start;
                endBox.Text = block.end;
            }

            for (int i = 0; i < scannerTabs.Count; i++)
                SetBlockRange(scannerTabs[i], "Crypto Scanner", scannerBlocks[i]);

            for (int i = 0; i < obScannerTabs.Count; i++)
                SetBlockRange(obScannerTabs[i], "Orderbook Scanner", obScannerBlocks[i]);

            for (int i = 0; i < curatorTabs.Count; i++)
                SetBlockRange(curatorTabs[i], "Curator Enabled", curatorBlocks[i]);

            for (int i = 0; i < TelegramScannerTabs.Count; i++)
                SetBlockRange(TelegramScannerTabs[i], "Telegram Scanner", telegramBlocks[i]);

            for (int i = 0; i < NewsScraperTabs.Count; i++)
                SetBlockRange(NewsScraperTabs[i], "News Scraper", newsScraperBlocks[i]);

            for (int i = 0; i < TrendsScraperTabs.Count; i++)
                SetBlockRange(TrendsScraperTabs[i], "Trends Scraper", trendsScraperBlocks[i]);

            for (int i = 0; i < HistoricalScannerTabs.Count; i++)
                SetBlockRange(HistoricalScannerTabs[i], "Historical Scanner", HistoricalScannerBlocks[i]);

            // Set Scanner IDs
            for (int i = 0; i < scannerTabs.Count; i++)
            {
                var tab = scannerTabs[i];
                // Extract node IP from tab label (e.g. "Node (10.0.0.44)" or "MyHost (10.0.0.44)")
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string scannerID = GenerateModuleID("Scanner", ip);

                var scannerIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "ScannerIDTextBox");
                if (scannerIDBox != null)
                    scannerIDBox.Text = scannerID;
            }

            // Set OBScanner IDs
            for (int i = 0; i < obScannerTabs.Count; i++)
            {
                var tab = obScannerTabs[i];
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string obScannerID = GenerateModuleID("OBScanner", ip);

                var obScannerIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "OBScannerIDTextBox");
                if (obScannerIDBox != null)
                    obScannerIDBox.Text = obScannerID;
            }

            // Set Curator IDs
            for (int i = 0; i < curatorTabs.Count; i++)
            {
                var tab = curatorTabs[i];
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string curatorID = GenerateModuleID("Curator", ip);

                var curatorIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorIDTextBox");
                if (curatorIDBox != null)
                    curatorIDBox.Text = curatorID;
            }

            // Set Telegram Scraper IDs
            for (int i = 0; i < TelegramScannerTabs.Count; i++)
            {
                var tab = TelegramScannerTabs[i];
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string TelegramScannerID = GenerateModuleID("TelegramScanner", ip);

                var telegramScannerIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TelegramScannerIDTextBox");
                if (telegramScannerIDBox != null)
                    telegramScannerIDBox.Text = TelegramScannerID;
            }

            // Set NewsScraper IDs
            for (int i = 0; i < NewsScraperTabs.Count; i++)
            {
                var tab = NewsScraperTabs[i];
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string newsScraperID = GenerateModuleID("News", ip);
                var newsScraperIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "NewsScraperIDTextBox");
                if (newsScraperIDBox != null)
                    newsScraperIDBox.Text = newsScraperID;
            }
            // Set TrendsScraper IDs
            for (int i = 0; i < TrendsScraperTabs.Count; i++)
            {
                var tab = TrendsScraperTabs[i];
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string trendsScraperID = GenerateModuleID("Trends", ip);
                var trendsScraperIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TrendsScraperIDTextBox");
                if (trendsScraperIDBox != null)
                    trendsScraperIDBox.Text = trendsScraperID;
            }

            // Set CapitolTrades IDs
            for (int i = 0; i < HistoricalScannerTabs.Count; i++)
            {
                var tab = HistoricalScannerTabs[i];
                string ip = tab.Text.Substring(tab.Text.LastIndexOf('(') + 1).TrimEnd(')');
                string capitolTradesID = GenerateModuleID("Retroscanner", ip);
                var capitolTradesIDBox = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "RetroScannerIDTextBox");
                if (capitolTradesIDBox != null)
                    capitolTradesIDBox.Text = capitolTradesID;
            }

            MessageBox.Show("Auto-delegate completed and asset ranges set.", "Done");
        }
        // Helper for ID generation
        private (int concurrentWorkers, int batchSize) CalculateCuratorParams(int cpuScore, int ramScore, int ramPerWorkerMb = 512, int batchRamCostMb = 128, int minBatch = 10, int maxBatch = 1000)
        {
            // Calculate concurrent workers
            int workersByCpu = cpuScore;
            int workersByRam = Math.Max(1, ramScore / ramPerWorkerMb);
            int concurrentWorkers = Math.Max(1, Math.Min(workersByCpu, workersByRam));

            // Calculate batch size
            int batchSize = Math.Max(minBatch, Math.Min(maxBatch, ramScore / batchRamCostMb));

            return (concurrentWorkers, batchSize);
        }

        string GenerateModuleID(string moduleType, string ip)
        {
            // Use last two octets as node-short (e.g. "0144" for 1.44, or fallback to whole IP)
            var octets = ip.Split('.');
            string nodeShort = octets.Length == 4
                ? $"{int.Parse(octets[2]):00}{int.Parse(octets[3]):00}"
                : ip.Replace(".", "");
            string timestamp = DateTime.Now.ToString("MMddHHmmss"); // Short, readable, unique-ish
            int rand = new Random().Next(10, 99); // To avoid rare duplicates
            return $"{moduleType}-{nodeShort}-{timestamp}-{rand}";
        }
        private (int concurrentWorkers, int batchSize) CalculateCuratorParams(int cpuScore, int ramScore)
        {
            int[] cpuWorkersMap = { 1, 2, 4 };   // score 1,2,3
            int[] ramWorkersMap = { 1, 3, 8 };   // score 1,2,3

            // Clamp to 1-3 (in case you ever tweak scoring)
            int cpuIdx = Math.Max(0, Math.Min(2, cpuScore - 1));
            int ramIdx = Math.Max(0, Math.Min(2, ramScore - 1));

            int concurrentWorkers = Math.Min(cpuWorkersMap[cpuIdx], ramWorkersMap[ramIdx]);

            int[] batchMap = { 10, 50, 100 };
            int batchSize = batchMap[ramIdx];

            return (concurrentWorkers, batchSize);
        }

        private void DragnetStartButton_Click(object sender, EventArgs e)
        {
            using (var pinForm = new PinEntry())
            {
                var result = pinForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (DragnetStartButton.Text == "Dragnet Start")
                    {
                        WipeDragnetControlTables();
                        FireScripts();
                        Thread.Sleep(1000); // Give it a moment to start
                        StartWatchdog();
                        StartScheduleTimer();
                        DragnetStartButton.Text = "Dragnet Stop";
                    }
                    else
                    {
                        DragnetStartButton.Text = "Dragnet Start";
                        watchdogTimer.Stop();
                        scheduleTimer.Stop();
                        ShutdownAllNodes();
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect PIN. Please try again.", "PIN Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void FireScripts()
        {
            float cryptoDelay = GlobalVariables.CryptoDelay;


            using (var userConn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                userConn.Open();
                var cmd = new MySqlCommand("SELECT CryptoDelay FROM users WHERE username = @username LIMIT 1", userConn);
                cmd.Parameters.AddWithValue("@username", GlobalVariables.username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        // else, keeps the default "0.5"
                    }
                    // else, keeps the default "0.5" if user not found
                }
            }

            foreach (TabPage tab in tabControl1.TabPages)
            {
                string ip = tab.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Text.Contains("."))?.Text ?? "";

                // --- Crypto Scanner ---
                var scannerBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Crypto Scanner");
                if (scannerBox != null && scannerBox.Checked)
                {
                    string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "ScannerBlockStartTextBox")?.Text.ToLower() ?? "A";
                    string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "ScannerBlockEndTextBox")?.Text.ToLower() ?? "Z";
                    string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "ScannerIDTextBox")?.Text.ToLower() ?? "";

                    string payload = $"{{\\\"script\\\":\\\"Scanner.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{cryptoDelay}\\\",\\\"{GlobalVariables.coinbaseAPIKey}\\\",\\\"{GlobalVariables.CoinbaseSecret}\\\",\\\"{GlobalVariables.CoinbasePassphrase}\\\",\\\"{GlobalVariables.BinanceAPI}\\\",\\\"{GlobalVariables.CryptoGranularity}\\\",\\\"{GlobalVariables.CryptoTimeSpan}\\\"]}}";
                    FireOffCommand(ip, payload);
                }

                // --- Orderbook Scanner ---
                var obBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Orderbook Scanner");
                if (obBox != null && obBox.Checked)
                {
                    string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "OBScannerBlockStartTextBox")?.Text.ToLower() ?? "A";
                    string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "OBScannerBlockEndTextBox")?.Text.ToLower() ?? "Z";
                    string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "OBScannerIDTextBox")?.Text.ToLower() ?? "";

                    string payload = $"{{\\\"script\\\":\\\"Obscanner.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{cryptoDelay}\\\",\\\"{GlobalVariables.coinbaseAPIKey}\\\",\\\"{GlobalVariables.CoinbaseSecret}\\\",\\\"{GlobalVariables.CoinbasePassphrase}\\\",\\\"{GlobalVariables.BinanceAPI}\\\",\\\"{GlobalVariables.CryptoGranularity}\\\",\\\"{GlobalVariables.CryptoTimeSpan}\\\"]}}";
                    FireOffCommand(ip, payload);
                }

                // --- Curator ---
                var curatorBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Curator Enabled");
                if (curatorBox != null && curatorBox.Checked)
                {
                    string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorBlockStartTextBox")?.Text.ToLower() ?? "A";
                    string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorBlockEndTextBox")?.Text.ToLower() ?? "Z";
                    string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorIDTextBox")?.Text.ToLower() ?? "";
                    string workers = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorWorkersTextBox")?.Text ?? "1";
                    string batchSize = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorBatchSizeTextBox")?.Text ?? "100";

                    string payload = $"{{\\\"script\\\":\\\"Curator.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{batchSize}\\\",\\\"{workers}\\\"]}}";
                    FireOffCommand(ip, payload);
                }

                var RetroScannerBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Historical Scanner");
                if (RetroScannerBox != null && RetroScannerBox.Checked)
                {
                    string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "RetroScannerBlockStartTextBox")?.Text.ToLower() ?? "A";
                    string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "RetroScannerBlockEndTextBox")?.Text.ToLower() ?? "Z";
                    string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "RetroScannerIDTextBox")?.Text.ToLower() ?? "";
                    string startdate = tab.Controls.OfType<MaskedTextBox>().FirstOrDefault(tb => tb.Name == "RetroScannerStartDateTextBox")?.Text.Replace("/", "") ?? "";
                    string enddate = tab.Controls.OfType<MaskedTextBox>().FirstOrDefault(tb => tb.Name == "RetroScannerEndDateTextBox")?.Text.Replace("/", "") ?? "";

                    string payload = $"{{\\\"script\\\":\\\"RetroScannerDragnet5.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{startdate}\\\",\\\"{enddate}\\\",\\\"{cryptoDelay}\\\"]}}";
                    FireOffCommand(ip, payload);
                }

                var TelegramScannerBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Telegram Scanner");
                if (TelegramScannerBox != null && TelegramScannerBox.Checked)
                {
                    string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TelegramScannerBlockStartTextBox")?.Text.ToLower() ?? "A";
                    string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TelegramScannerBlockEndTextBox")?.Text.ToLower() ?? "Z";
                    string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TelegramScannerIDTextBox")?.Text.ToLower() ?? "";
                    string startdate = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TelegramScannerBlockStartTextBox")?.Text ?? "";
                    string enddate = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TelegramScannerBlockEndTextBox")?.Text ?? "";
                    string session_name = "DragnetSweep";

                    string payload = $"{{\\\"script\\\":\\\"TelegramScanner.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{session_name}\\\",\\\"{GlobalVariables.TelegramAPIKey}\\\",\\\"{GlobalVariables.TelegramAPIHash}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{GlobalVariables.PhoneNumber}\\\",\\\"{GlobalVariables.TelegramDelay}\\\",\\\"{GlobalVariables.TelegramTimespan}\\\"]}}";
                    FireOffCommand(ip, payload);
                }
                var NewsDaemonCheckBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "News Daemon");
                if (NewsDaemonCheckBox != null && NewsDaemonCheckBox.Checked)
                {
                    string id = "PromptDaemon";
                    string payload = $"{{\\\"script\\\":\\\"PromptDaemon.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.newsIP}\\\",\\\"{GlobalVariables.newsUser}\\\",\\\"{GlobalVariables.newsPW}\\\",\\\"{GlobalVariables.DataDumpIP}\\\",\\\"{GlobalVariables.DataDumpUser}\\\",\\\"{GlobalVariables.DataDumpPW}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.LLMHost}\\\",\\\"{GlobalVariables.LLMPort}\\\",\\\"{GlobalVariables.ActiveLLMPrompt}\\\",\\\"{GlobalVariables.ActiveLLMPromptVersion}\\\",\\\"{newstimeframe}\\\",\\\"{GlobalVariables.LLMModel}\\\",\\\"{GlobalVariables.LLMContextWindow}\\\"]}}";
                    FireOffCommand(ip, payload);
                }
            }

            MessageBox.Show("All enabled scripts fired to all nodes.", "Fire Scripts", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private bool IsLocalIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return true; // Treat blank as localhost

            ip = ip.Trim().ToLowerInvariant();

            if (ip == "localhost" || ip == "127.0.0.1" || ip == "::1")
                return true;

            try
            {
                // Get all local addresses (IPv4 and IPv6)
                var localAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                    .Select(a => a.ToString().ToLowerInvariant())
                    .ToList();

                return localAddresses.Contains(ip);
            }
            catch
            {
                // Fallback: assume not local if DNS fails
                return false;
            }
        }
        public void RestartModuleFromRow(DataRow row, string moduleType)
        {
            string ip = row["Node"].ToString();
            string id = row["ID"].ToString();
            string start = row.Table.Columns.Contains("Start") ? row["Start"].ToString() : "A";
            string end = row.Table.Columns.Contains("End") ? row["End"].ToString() : "Z";
            float cryptoDelay = GlobalVariables.CryptoDelay;

            string script;
            string payload = "";

            switch (moduleType?.ToLowerInvariant())
            {
                case "orderbook":
                    script = "Obscanner.exe";
                    payload =
                        $"{{\\\"script\\\":\\\"{script}\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{cryptoDelay}\\\",\\\"{GlobalVariables.coinbaseAPIKey}\\\",\\\"{GlobalVariables.CoinbaseSecret}\\\",\\\"{GlobalVariables.CoinbasePassphrase}\\\",\\\"{GlobalVariables.BinanceAPI}\\\",\\\"{GlobalVariables.CryptoGranularity}\\\",\\\"{GlobalVariables.CryptoTimeSpan}\\\"]}}";
                    break;

                case "scanner":
                    script = "Scanner.exe";
                    payload =
                        $"{{\\\"script\\\":\\\"{script}\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{cryptoDelay}\\\",\\\"{GlobalVariables.coinbaseAPIKey}\\\",\\\"{GlobalVariables.CoinbaseSecret}\\\",\\\"{GlobalVariables.CoinbasePassphrase}\\\",\\\"{GlobalVariables.BinanceAPI}\\\",\\\"{GlobalVariables.CryptoGranularity}\\\",\\\"{GlobalVariables.CryptoTimeSpan}\\\"]}}";
                    break;

                case "curator":
                    // Read UI knobs (workers/batch) from the tab that matches this node and is enabled
                    TabPage curatorTab = tabControl1.TabPages.Cast<TabPage>()
                        .FirstOrDefault(t => t.Text.EndsWith($"({ip})") &&
                                             t.Controls.OfType<System.Windows.Forms.CheckBox>()
                                              .Any(cb => cb.Text == "Curator Enabled" && cb.Checked));

                    string workers = "1";
                    string batchSize = "10";
                    if (curatorTab != null)
                    {
                        var workersBox = curatorTab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorWorkersTextBox");
                        var batchBox = curatorTab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CuratorBatchSizeTextBox");
                        if (workersBox != null) workers = workersBox.Text;
                        if (batchBox != null) batchSize = batchBox.Text;
                    }
                    script = "Curator.exe";
                    payload =
                        $"{{\\\"script\\\":\\\"{script}\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{batchSize}\\\",\\\"{workers}\\\"]}}";
                    break;

                default:
                    // unknown moduleType; nothing to do
                    return;
            }

            if (string.IsNullOrWhiteSpace(payload)) return;

            // --- minimal de-dupe / throttle guard (per node+script+id+range) ---
            string key = $"{ip}|{id}|{moduleType}|{start}-{end}";
            lock (_restartGuardLock)
            {
                // Block concurrent re-entries
                if (_inFlight.Contains(key))
                {
                    // already launching this exact thing
                    return;
                }

                // Throttle rapid repeats
                if (_lastStart.TryGetValue(key, out var last) &&
                    (DateTime.UtcNow - last).TotalSeconds < RestartThrottleSeconds)
                {
                    return; // recent run already happened
                }

                _inFlight.Add(key);
                _lastStart[key] = DateTime.UtcNow; // remember last start time
            }

            try
            {
                FireOffCommand(ip, payload);
            }
            finally
            {
                lock (_restartGuardLock)
                {
                    _inFlight.Remove(key);
                    // keep _lastStart timestamp so the throttle window remains in effect
                }
            }
        }
        private void FireOffCommand(string ip, string jsonPayload)
        {
            if (IsLocalIP(ip))
            {
                // Parse script and args from double-escaped JSON string
                string cleaned = jsonPayload.Replace("\\\"", "\"").Replace(@"\\", @"\");
                var scriptMatch = System.Text.RegularExpressions.Regex.Match(cleaned, "\"script\"\\s*:\\s*\"([^\"]+)\"");
                var argsMatch = System.Text.RegularExpressions.Regex.Match(cleaned, "\"args\"\\s*:\\s*\\[(.*?)\\]");
                string script = scriptMatch.Success ? scriptMatch.Groups[1].Value : "";
                var args = argsMatch.Success
                    ? argsMatch.Groups[1].Value.Split(new[] { "\",\"" }, StringSplitOptions.None).Select(a => a.Trim('"')).ToList()
                    : new List<string>();

                string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, script);
                string argLine = string.Join(" ", args.Select(a => $"\"{a}\""));

                try
                {
                    // Start in a new terminal window (cmd.exe)
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k \"{exePath} {argLine}\"", // /k = keep window open, /c = close after finish
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                    };
                    Process.Start(psi); // No WaitForExit, just fire and forget!
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[EXCEPTION - LOCAL]: {ex.Message}", "Script Launch Error");
                }
            }
            else
            {
                // Remote node logic (unchanged)
                string curlArgs = $"-X POST -H \"Content-Type: application/json\" -d \"{jsonPayload}\" http://{ip}:5005/run";
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("curl", curlArgs)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var proc = Process.Start(psi))
                    {
                        proc.StandardOutput.ReadToEnd();
                        proc.StandardError.ReadToEnd();
                        proc.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[EXCEPTION]: {ex.Message}", "Remote Script Error");
                }
            }
        }
        // --- Utility: implement this to get the correct delay or other DB values ---
        private string GetUserField(string fieldName)
        {
            // Query the field value from your users table, e.g.
            // SELECT fieldName FROM users WHERE username=... LIMIT 1
            // Return as string
            return "0.5"; // Placeholder, plug in your DB code here
        }

        private void deleteNodeButton_Click(object sender, EventArgs e)
        {
            // 1. Get the selected tab
            TabPage selectedTab = tabControl1.SelectedTab;
            if (selectedTab == null)
            {
                MessageBox.Show("No node selected.", "Delete Node", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Confirm with the user
            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete node \"{selectedTab.Text}\"?\nThis action cannot be undone.",
                "Confirm Node Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
                return;

            // 3. Extract IP address from the tab label (e.g. "Node (10.0.0.44)" or "MyHost (10.0.0.44)")
            string tabText = selectedTab.Text;
            int parenStart = tabText.LastIndexOf('(');
            int parenEnd = tabText.LastIndexOf(')');
            if (parenStart == -1 || parenEnd == -1 || parenEnd <= parenStart)
            {
                MessageBox.Show("Could not determine node IP from tab label.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string ip = tabText.Substring(parenStart + 1, parenEnd - parenStart - 1).Trim();

            // 4. Delete from database
            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();
                string sql = "DELETE FROM dragnet_nodes WHERE ip_address = @ip";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("Node not found in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            // 5. Remove the tab
            tabControl1.TabPages.Remove(selectedTab);

            MessageBox.Show($"Node \"{selectedTab.Text}\" deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void LoadDragnetLocks()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT * FROM dragnet_locks ORDER BY asset_name;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Optional: rename the default column header
                locksDataGridView.ScrollBars = ScrollBars.None;
                locksDataGridView.DataSource = dt;
                locksDataGridView.RowHeadersVisible = false;
                dt.Columns[0].ColumnName = "Asset";
                locksDataGridView.Columns["Asset"].Width = 123;
                dt.Columns[1].ColumnName = "Module";
                locksDataGridView.Columns["Module"].Width = 200;
                dt.Columns[2].ColumnName = "Expiration";
                locksDataGridView.Columns["Expiration"].Width = 160;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }
        }
        private void LoadCuratorList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT curator_id, node_ip, asset_range_start, asset_range_end, status, last_heartbeat FROM curator_modules ORDER BY node_ip;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Node";
                dt.Columns[2].ColumnName = "Start";
                dt.Columns[3].ColumnName = "End";
                dt.Columns[4].ColumnName = "Status";
                dt.Columns[5].ColumnName = "Heartbeat";
                curatorScriptsDataGridView.ScrollBars = ScrollBars.None;
                curatorScriptsDataGridView.DataSource = dt;
                ColorRowsByStatus(curatorScriptsDataGridView);
                curatorScriptsDataGridView.RowHeadersVisible = false;
                curatorScriptsDataGridView.Columns["ID"].Width = 140;
                curatorScriptsDataGridView.Columns["Node"].Width = 103;
                curatorScriptsDataGridView.Columns["Start"].Width = 45;
                curatorScriptsDataGridView.Columns["End"].Width = 40;
                curatorScriptsDataGridView.Columns["Status"].Width = 60;
                curatorScriptsDataGridView.Columns["Heartbeat"].Width = 95;
                foreach (DataGridViewRow dgRow in curatorScriptsDataGridView.Rows)
                {
                    if (dgRow.IsNewRow) continue;
                    if (dgRow.DefaultCellStyle.BackColor == Color.Red)
                    {
                        DataRowView drv = dgRow.DataBoundItem as DataRowView;
                        if (drv != null)
                        {
                            RestartModuleFromRow(drv.Row, "curator");
                            Console.WriteLine($"[RESTART] Auto-restarted orderbook module on {drv.Row["Node"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }
        }
        private void LoadScannerList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT scanner_id, node_ip, asset_range_start, asset_range_end, status, last_heartbeat FROM scanner_modules ORDER BY node_ip;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Node";
                dt.Columns[2].ColumnName = "Start";
                dt.Columns[3].ColumnName = "End";
                dt.Columns[4].ColumnName = "Status";
                dt.Columns[5].ColumnName = "Heartbeat";
                scannerDataGridView.ScrollBars = ScrollBars.None;
                scannerDataGridView.DataSource = dt;
                ColorRowsByStatus(scannerDataGridView);
                scannerDataGridView.RowHeadersVisible = false;
                scannerDataGridView.Columns["ID"].Width = 140;
                scannerDataGridView.Columns["Node"].Width = 103;
                scannerDataGridView.Columns["Start"].Width = 45;
                scannerDataGridView.Columns["End"].Width = 40;
                scannerDataGridView.Columns["Status"].Width = 60;
                scannerDataGridView.Columns["Heartbeat"].Width = 95;
                foreach (DataGridViewRow dgRow in scannerDataGridView.Rows)
                {
                    if (dgRow.IsNewRow) continue;
                    if (dgRow.DefaultCellStyle.BackColor == Color.Red)
                    {
                        DataRowView drv = dgRow.DataBoundItem as DataRowView;
                        if (drv != null)
                        {
                            RestartModuleFromRow(drv.Row, "scanner");
                            Console.WriteLine($"[RESTART] Auto-restarted orderbook module on {drv.Row["Node"]}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }
        }
        private void LoadObScannerList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT Obscanner_id, node_ip, asset_range_start, asset_range_end, status, last_heartbeat FROM orderbook_modules ORDER BY node_ip;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Node";
                dt.Columns[2].ColumnName = "Start";
                dt.Columns[3].ColumnName = "End";
                dt.Columns[4].ColumnName = "Status";
                dt.Columns[5].ColumnName = "Heartbeat";
                orderBookDataGridView.ScrollBars = ScrollBars.None;
                orderBookDataGridView.DataSource = dt;
                ColorRowsByStatus(orderBookDataGridView);
                orderBookDataGridView.RowHeadersVisible = false;
                orderBookDataGridView.Columns["ID"].Width = 140;
                orderBookDataGridView.Columns["Node"].Width = 103;
                orderBookDataGridView.Columns["Start"].Width = 45;
                orderBookDataGridView.Columns["End"].Width = 40;
                orderBookDataGridView.Columns["Status"].Width = 60;
                orderBookDataGridView.Columns["Heartbeat"].Width = 95;

                foreach (DataGridViewRow dgRow in orderBookDataGridView.Rows)
                {
                    if (dgRow.IsNewRow) continue;
                    if (dgRow.DefaultCellStyle.BackColor == Color.Red)
                    {
                        DataRowView drv = dgRow.DataBoundItem as DataRowView;
                        if (drv != null)
                        {
                            RestartModuleFromRow(drv.Row, "orderbook");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }

        }
        private void LoadNewsScraperList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT newsscraper_id, node_ip, asset_range_start, asset_range_end, status, last_heartbeat FROM newsscraper_modules ORDER BY node_ip;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Node";
                dt.Columns[2].ColumnName = "Start";
                dt.Columns[3].ColumnName = "End";
                dt.Columns[4].ColumnName = "Status";
                dt.Columns[5].ColumnName = "Heartbeat";
                newsScraperDataGridView.ScrollBars = ScrollBars.None;
                newsScraperDataGridView.DataSource = dt;
                ColorRowsByStatus(orderBookDataGridView);
                newsScraperDataGridView.RowHeadersVisible = false;
                newsScraperDataGridView.Columns["ID"].Width = 140;
                newsScraperDataGridView.Columns["Node"].Width = 103;
                newsScraperDataGridView.Columns["Start"].Width = 45;
                newsScraperDataGridView.Columns["End"].Width = 40;
                newsScraperDataGridView.Columns["Status"].Width = 60;
                newsScraperDataGridView.Columns["Heartbeat"].Width = 95;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }

        }

        private void LoadTelegramScannerList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT telegramscanner_id, node_ip, asset_range_start, asset_range_end, status, last_heartbeat FROM telegramscanner_modules ORDER BY node_ip;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Node";
                dt.Columns[2].ColumnName = "Start";
                dt.Columns[3].ColumnName = "End";
                dt.Columns[4].ColumnName = "Status";
                dt.Columns[5].ColumnName = "Heartbeat";
                telegramScannerDataGridView.ScrollBars = ScrollBars.None;
                telegramScannerDataGridView.DataSource = dt;
                ColorRowsByStatus(orderBookDataGridView);
                telegramScannerDataGridView.RowHeadersVisible = false;
                telegramScannerDataGridView.Columns["ID"].Width = 140;
                telegramScannerDataGridView.Columns["Node"].Width = 103;
                telegramScannerDataGridView.Columns["Start"].Width = 45;
                telegramScannerDataGridView.Columns["End"].Width = 40;
                telegramScannerDataGridView.Columns["Status"].Width = 60;
                telegramScannerDataGridView.Columns["Heartbeat"].Width = 95;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }

        }

        private void LoadDaemonList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT daemon_id, node_ip, status, last_heartbeat FROM daemon_modules ORDER BY node_ip;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Node";
                dt.Columns[2].ColumnName = "Status";
                dt.Columns[3].ColumnName = "Heartbeat";
                daemonsDataGridView.ScrollBars = ScrollBars.None;
                daemonsDataGridView.DataSource = dt;
                daemonsDataGridView.RowHeadersVisible = false;
                daemonsDataGridView.Columns["ID"].Width = 140;
                daemonsDataGridView.Columns["Node"].Width = 143;
                daemonsDataGridView.Columns["Status"].Width = 105;
                daemonsDataGridView.Columns["Heartbeat"].Width = 95;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }

        }
        private void ColorRowsByStatus(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                // Defensive: skip new row placeholder, etc.
                if (row.IsNewRow) continue;

                // Status check
                string status = row.Cells["status"].Value?.ToString().ToLower() ?? "";

                // Heartbeat check
                string heartbeatStr = row.Cells["heartbeat"].Value?.ToString();
                DateTime heartbeat;
                bool heartbeatParsed = DateTime.TryParse(heartbeatStr, out heartbeat);
                double secondsAgo = heartbeatParsed ? (DateTime.UtcNow - heartbeat.ToUniversalTime()).TotalSeconds : 99999;

                // Default: normal
                row.DefaultCellStyle.BackColor = Color.White;

                // If stopped, red
                if (status == "stopped")
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                // If heartbeat missing/parse error, light red
                else if (!heartbeatParsed)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
                // Heartbeat: yellow for 5-15s, red for 15+
                else if (secondsAgo > 60)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                else if (secondsAgo > 5)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void LoadErrorLog()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    string query = "SELECT * FROM module_logs ORDER BY timestamp;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Optional: rename the default column header
                ErrorDataGridView.ScrollBars = ScrollBars.None;
                ErrorDataGridView.DataSource = dt;
                ErrorDataGridView.RowHeadersVisible = false;
                dt.Columns[0].ColumnName = "Timestamp";
                ErrorDataGridView.Columns["Timestamp"].Width = 150;
                dt.Columns[1].ColumnName = "Module Type";
                ErrorDataGridView.Columns["Module Type"].Width = 100;
                dt.Columns[2].ColumnName = "ID";
                ErrorDataGridView.Columns["ID"].Width = 150;
                dt.Columns[3].ColumnName = "IP Address";
                ErrorDataGridView.Columns["IP Address"].Width = 150;
                dt.Columns[4].ColumnName = "Log Level";
                ErrorDataGridView.Columns["Log Level"].Width = 100;
                dt.Columns[5].ColumnName = "Error";
                ErrorDataGridView.Columns["Error"].Width = 325;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table names: " + ex.Message);
            }
        }
        public void WipeDragnetControlTables()
        {
            string[] tables = new string[]
            {
                "curator_modules",
                "dragnet_locks",
                "module_logs",
                "orderbook_modules",
                "scanner_modules",
                "newsscraper_modules",
                "daemon_modules",
                "telegramscanner_modules"
            };

            try
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
                {
                    conn.Open();
                    foreach (string table in tables)
                    {
                        string sql = $"DELETE FROM {table};";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine($"Deleted all entries from {table}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void UpdateDashboard()
        {
            // Check one main UI control to see if you're on UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateDashboard)); // Hop to UI thread, then call again
                return;
            }
            // Now, always on UI thread:
            LoadDaemonList();
            LoadDragnetLocks();
            LoadCuratorList();
            LoadScannerList();
            LoadObScannerList();
            LoadNewsScraperList();
            LoadTelegramScannerList();
            LoadErrorLog();
        }

        private void dragnetTablesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dragnetTablesDataGridView.Rows[e.RowIndex];
                CurrentTableName = row.Cells[0].Value.ToString(); // Column 0, since it’s just the name
                LoadDragnetTable(CurrentTableName);
            }
        }

        private void LoadDragnetTable(string tableName)
        {
            var conn = new MySqlConnection(GlobalVariables.DragnetDBConnect);
            string query = $"SELECT * FROM `{tableName}` ORDER BY timestamp DESC LIMIT 33";
            dragnetAdapter = new MySqlDataAdapter(query, conn);
            dragnetBuilder = new MySqlCommandBuilder(dragnetAdapter);
            dragnetDt = new DataTable();
            conn.Open();
            dragnetAdapter.Fill(dragnetDt);
            conn.Close();

            // Event wiring (do not duplicate handlers)
            dragnetDt.RowChanged += DragnetDataTable_Changed;
            dragnetDt.RowDeleted += DragnetDataTable_Changed;
            dragnetDt.RowChanging += DragnetDataTable_Changed;
            dragnetDt.TableNewRow += DragnetDataTable_Changed;

            dragnetDt.PrimaryKey = new DataColumn[] { dragnetDt.Columns["timestamp"] };

            DragnetDataGridView.DataSource = dragnetDt;
            DragnetDataGridView.AllowUserToAddRows = false;
            DragnetDataGridView.RowHeadersVisible = false;
            if (DragnetDataGridView.Columns.Contains("timestamp"))
                DragnetDataGridView.Columns["timestamp"].ReadOnly = true;

            dragnetCommitButton.Enabled = false;
        }

        private void DragnetDataTable_Changed(object sender, EventArgs e)
        {
            dragnetCommitButton.Enabled = true;
        }
        private void dragnetCommitButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to save all changes to the database?\nThis action cannot be undone.",
                "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            using (var conn = new MySqlConnection(GlobalVariables.DragnetDBConnect))
            {
                conn.Open();
                int updateCount = 0;
                foreach (DataRow row in dragnetDt.Rows)
                {
                    if (row.RowState == DataRowState.Modified)
                    {
                        // Build SET clause dynamically (for all columns except PK)
                        List<string> setClauses = new List<string>();
                        List<MySqlParameter> parameters = new List<MySqlParameter>();
                        foreach (DataColumn col in dragnetDt.Columns)
                        {
                            if (col.ColumnName == "timestamp") continue; // skip PK in SET
                                                                         // Use the exact column name, as present in the DataTable and DB
                            string safeParamName = col.ColumnName.Replace("%", "Pct").Replace(".", "_"); // For the parameter only!
                            setClauses.Add($"`{col.ColumnName}` = @{safeParamName}");
                            parameters.Add(new MySqlParameter($"@{safeParamName}", row[col.ColumnName] ?? DBNull.Value));
                        }

                        string setClause = string.Join(", ", setClauses);

                        // Only WHERE by the PK (timestamp)
                        string sql = $"UPDATE `{CurrentTableName}` SET {setClause} WHERE `timestamp` = @timestamp";
                        var cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddRange(parameters.ToArray());
                        cmd.Parameters.Add(new MySqlParameter("@timestamp", row["timestamp"]));

                        try
                        {
                            int rows = cmd.ExecuteNonQuery();
                            updateCount += rows;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to update row with timestamp {row["timestamp"]}: {ex.Message}");
                        }
                    }
                }
                MessageBox.Show($"Saved {updateCount} rows.");
                conn.Close();
            }

            // Optionally reload the table after saving
            if (!string.IsNullOrEmpty(CurrentTableName))
                LoadDragnetTable(CurrentTableName);

            dragnetCommitButton.Enabled = false;
        }
        private void dragnetRefreshButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentTableName))
                LoadDragnetTable(CurrentTableName);
        }

        private void DragnetSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string filterText = DragnetSearchTextBox.Text.Replace("'", "''"); // prevent SQL-like filter errors

            if (string.IsNullOrWhiteSpace(filterText))
            {
                (dragnetTablesDataGridView.DataSource as DataTable).DefaultView.RowFilter = "";
            }
            else
            {
                // This will search all columns
                string filter = string.Join(" OR ",
                    (dragnetTablesDataGridView.DataSource as DataTable).Columns
                    .Cast<DataColumn>()
                    .Where(c => c.DataType == typeof(string) || c.DataType == typeof(int) || c.DataType == typeof(double) || c.DataType == typeof(float))
                    .Select(c => $"CONVERT([{c.ColumnName}], 'System.String') LIKE '%{filterText}%'"));

                (dragnetTablesDataGridView.DataSource as DataTable).DefaultView.RowFilter = filter;
            }
        }
        public void StartWatchdog()
        {
            watchdogTimer.Interval = 5000; // 5 seconds (or whatever you like)
            watchdogTimer.Tick += WatchdogTimer_Tick;
            watchdogTimer.Start();
        }
        private void WatchdogTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => UpdateDashboard());
        }

        public void ShutdownAllNodes()
        {
            List<string> nodeIPs = new List<string>();

            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();
                string sql = "SELECT ip_address FROM dragnet_nodes WHERE enabled = 1;";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nodeIPs.Add(reader["ip_address"].ToString());
                    }
                }
            }

            // Always include localhost as a failsafe
            if (!nodeIPs.Contains("127.0.0.1"))
                nodeIPs.Add("127.0.0.1");

            foreach (string ip in nodeIPs)
            {
                KillOffCommand(ip, "all");
            }
        }

        public void KillOffCommand(string ip, string processNameOrAll = "all")
        {
            if (IsLocalIP(ip))
            {
                // Localhost: kill all relevant EXEs directly
                // If 'all', kill all known script EXEs. Otherwise, kill just the one requested.
                string[] processNames;
                if (processNameOrAll == "all")
                {
                    processNames = new string[] {
                "Scanner.exe", "Obscanner.exe", "Curator.exe",
                "GoogleNewsCollector.exe", "TrendsScraper.exe", "CapitolTradesModule.exe", "RetroScannerDragnet5.exe", "PromptDaemon.exe", "TelegramScanner.exe"
                // Add any others you run here
            };
                }
                else
                {
                    processNames = new string[] { processNameOrAll };
                }

                foreach (var procName in processNames)
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("taskkill", $"/F /IM {procName}")
                        {
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using (var proc = Process.Start(psi))
                        {
                            proc.StandardOutput.ReadToEnd();
                            proc.StandardError.ReadToEnd();
                            proc.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"[KILL EXCEPTION - LOCAL]: {ex.Message}", "Script Kill Error");
                    }
                }
            }
            else
            {
                // Remote node: send kill command to /kill endpoint
                string jsonPayload = $"{{\\\"process\\\":\\\"{processNameOrAll}\\\"}}";
                string curlArgs = $"-X POST -H \"Content-Type: application/json\" -d \"{jsonPayload}\" http://{ip}:5005/kill";
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("curl", curlArgs)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var proc = Process.Start(psi))
                    {
                        proc.StandardOutput.ReadToEnd();
                        proc.StandardError.ReadToEnd();
                        proc.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[KILL EXCEPTION]: {ex.Message}", "Remote Script Kill Error");
                }
            }
        }
        private void saveTaskButton_Click(object sender, EventArgs e)
        {
            string scriptType = scriptTypeComboBox.SelectedItem?.ToString();
            string triggerTime = triggerTimeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(scriptType) || string.IsNullOrEmpty(triggerTime))
            {
                MessageBox.Show("Please select a script type and enter a valid time.");
                return;
            }

            // === Handle Days of Week ===
            string daysOfWeek = "";
            bool defaultMode = false;

            foreach (object item in daysOfWeekChecklist.CheckedItems)
            {
                if (item.ToString().ToLower().Contains("default"))
                {
                    defaultMode = true;
                    break;
                }
            }

            if (defaultMode)
            {
                daysOfWeek = "default";
            }
            else
            {
                string[] weekdays = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                foreach (string day in weekdays)
                {
                    daysOfWeek += daysOfWeekChecklist.CheckedItems.Contains(day) ? "1" : "0";
                }
            }

            // === Handle Calendar Dates – Manual JSON Array Format ===
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            var selectedDates = monthCalendar1.BoldedDates;
            for (int i = 0; i < selectedDates.Length; i++)
            {
                sb.Append("\"");
                sb.Append(selectedDates[i].ToString("yyyy-MM-dd"));
                sb.Append("\"");

                if (i < selectedDates.Length - 1)
                    sb.Append(",");
            }

            sb.Append("]");
            string calendarDatesJson = sb.ToString();

            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();
                string query = @"
            INSERT INTO dragnet_schedule (script_type, trigger_time, days_of_week, calendar_dates, created_at)
            VALUES (@type, @time, @days, @dates, NOW());";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type", scriptType);
                    cmd.Parameters.AddWithValue("@time", TimeSpan.Parse(triggerTime));
                    cmd.Parameters.AddWithValue("@days", daysOfWeek);
                    cmd.Parameters.AddWithValue("@dates", calendarDatesJson);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Task saved.");
            RefreshScheduleGrid(); // Optional
        }

        private void RefreshScheduleGrid()
        {

            string query = "SELECT * FROM dragnet_schedule ORDER BY trigger_time;";

            DataTable table = new DataTable();

            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();

                using (var cmd = new MySqlCommand(query, conn))
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }

            // Optional: Translate days_of_week and calendar_dates into something readable
            foreach (DataRow row in table.Rows)
            {
                string dow = row["days_of_week"]?.ToString();

                if (dow == "default")
                {
                    row["days_of_week"] = "All Days";
                }
                else if (dow.Length == 7)
                {
                    string[] names = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                    var activeDays = new List<string>();

                    for (int i = 0; i < 7; i++)
                    {
                        if (dow[i] == '1') activeDays.Add(names[i]);
                    }

                    row["days_of_week"] = string.Join(", ", activeDays);
                }

                // Format calendar_dates from raw JSON string to readable list
                string rawJson = row["calendar_dates"]?.ToString();
                if (!string.IsNullOrWhiteSpace(rawJson) && rawJson.StartsWith("["))
                {
                    try
                    {
                        // Manual parse — zero dependencies
                        string formatted = rawJson
                            .Replace("[", "")
                            .Replace("]", "")
                            .Replace("\"", "")
                            .Trim();

                        row["calendar_dates"] = string.IsNullOrWhiteSpace(formatted) ? "(none)" : formatted;
                    }
                    catch
                    {
                        row["calendar_dates"] = "(corrupted)";
                    }
                }
            }

            scheduleGridView.DataSource = table;
            // === Column width adjustments ===
            if (scheduleGridView.Columns.Contains("script_type"))
                scheduleGridView.Columns["script_type"].Width = 180;

            if (scheduleGridView.Columns.Contains("trigger_time"))
                scheduleGridView.Columns["trigger_time"].Width = 100;

            if (scheduleGridView.Columns.Contains("days_of_week"))
                scheduleGridView.Columns["days_of_week"].Width = 197;

            if (scheduleGridView.Columns.Contains("calendar_dates"))
                scheduleGridView.Columns["calendar_dates"].Width = 300;

            if (scheduleGridView.Columns.Contains("created_at"))
                scheduleGridView.Columns["created_at"].Width = 150;
            scheduleGridView.RowHeadersVisible = false;
        }
        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime clickedDate = e.Start.Date;

            var current = monthCalendar1.BoldedDates.ToList();

            if (current.Contains(clickedDate))
            {
                // Remove date from bolded list
                current.Remove(clickedDate);
            }
            else
            {
                // Add date to bolded list
                current.Add(clickedDate);
            }

            monthCalendar1.BoldedDates = current.ToArray();
            monthCalendar1.UpdateBoldedDates();
        }
        private void deleteTaskButton_Click(object sender, EventArgs e)
        {
            if (scheduleGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a task to delete.");
                return;
            }

            DataGridViewRow selectedRow = scheduleGridView.SelectedRows[0];

            string scriptType = selectedRow.Cells["script_type"].Value.ToString();
            string triggerTime = selectedRow.Cells["trigger_time"].Value.ToString();

            var result = MessageBox.Show(
                $"Are you sure you want to delete:\n\nScript: {scriptType}\nTime: {triggerTime}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            using (var conn = new MySqlConnection(GlobalVariables.ControlDBConnect))
            {
                conn.Open();
                string query = @"DELETE FROM dragnet_schedule WHERE script_type = @type AND trigger_time = @time LIMIT 1;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type", scriptType);
                    cmd.Parameters.AddWithValue("@time", TimeSpan.Parse(triggerTime));
                    int affected = cmd.ExecuteNonQuery();

                    if (affected > 0)
                        MessageBox.Show("Schedule deleted.");
                    else
                        MessageBox.Show("No matching schedule found.");
                }
            }
            RefreshScheduleGrid();
        }

        private void CheckAndFireScheduledScripts()
        {
            if (_checkInProgress) return;
            _checkInProgress = true;
            try
            {
                // Use UTC consistently to avoid DST surprises
                DateTime nowUtc = DateTime.UtcNow;
                DateTime minuteUtc = TruncateToMinute(nowUtc);

                // Reset once per UTC day
                if (minuteUtc.Date != lastResetDate)
                {
                    firedThisMinute.Clear();
                    lastResetDate = minuteUtc.Date;
                    Console.WriteLine("[SCHEDULER] Reset fired set.");
                }

                // Refresh schedule cache every 60s
                if (nowUtc >= scheduleCacheExpiry)
                {
                    cachedSchedule = LoadScheduleRows();     // pulls from DB
                    scheduleCacheExpiry = nowUtc.AddSeconds(60);
                }

                // Evaluate rows for THIS minute
                foreach (var row in cachedSchedule)
                {
                    if (!ShouldRunThisMinute(row, minuteUtc))
                        continue;

                    FireScheduledScripts(row.ScriptType, minuteUtc);
                }
            }
            finally
            {
                _checkInProgress = false;
            }
        }
        private bool ShouldRunThisMinute(ScheduleRow row, DateTime minuteUtc)
        {
            // 1) Time-of-day match
            // If your trigger_time is stored as local time, convert; better: store UTC or store local+timezone.
            var triggerLocal = row.Trigger; // HH:mm
            var localNow = minuteUtc.ToLocalTime();
            if (new TimeSpan(localNow.Hour, localNow.Minute, 0) != triggerLocal)
                return false;

            // 2) Calendar dates (optional)
            if (row.CalendarDates is { Count: > 0 })
            {
                string todayLocal = localNow.ToString("yyyy-MM-dd");
                if (!row.CalendarDates.Contains(todayLocal))
                    return false;
            }

            // 3) Day-of-week mask
            if (!string.Equals(row.DaysMask, "default", StringComparison.OrdinalIgnoreCase))
            {
                // Mask is Sunday=0 .. Saturday=6
                int idx = (int)localNow.DayOfWeek;
                if (row.DaysMask.Length != 7 || row.DaysMask[idx] != '1')
                    return false;
            }

            return true;
        }
        private void FireScheduledScripts(string label, DateTime minuteUtc)
        {
            string minuteKey = MinuteKey(minuteUtc);

            foreach (TabPage tab in tabControl1.TabPages)
            {
                string ip = tab.Controls.OfType<Label>()
                            .FirstOrDefault(lbl => lbl.Text.Contains("."))?.Text ?? "localhost";

                var key = (label, ip, minuteKey);
                if (firedThisMinute.Contains(key))
                    continue;

                // Mark BEFORE firing to avoid double-fires if something is slow
                //firedThisMinute.Add(key);

                // --- Direct update scripts ---
                if (label == "Coinbase Update")
                {
                    string payload = $"{{\\\"script\\\":\\\"CoinbaseBuildAssetList.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{GlobalVariables.assetDBName}\\\",\\\"crypto\\\"]}}";
                    FireOffCommand(ip, payload);
                }
                if (label == "Binance Update")
                {
                    string payload = $"{{\\\"script\\\":\\\"BinanceBuildAssetList.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{GlobalVariables.assetDBName}\\\",\\\"crypto\\\"]}}";
                    FireOffCommand(ip, payload);
                }
                if (label == "Kraken Update")
                {
                    string payload = $"{{\\\"script\\\":\\\"KrakenBuildAssetList.exe\\\",\\\"args\\\":[\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{GlobalVariables.assetDBName}\\\",\\\"crypto\\\"]}}";
                    FireOffCommand(ip, payload);
                }

                // --- Checkbox-controlled scripts ---
                var box = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == label);
                if (box != null && box.Checked)
                {
                    // News Scraper
                    var newsBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "News Scraper");
                    if (newsBox != null && newsBox.Checked)
                    {
                        string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "NewsScraperBlockStartTextBox")?.Text.ToLower() ?? "A";
                        string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "NewsScraperBlockEndTextBox")?.Text.ToLower() ?? "Z";
                        string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "NewsScraperIDTextBox")?.Text.ToLower() ?? "";

                        string payload = $"{{\\\"script\\\":\\\"GoogleNewsCollector.exe\\\",\\\"args\\\":[\\\"{id}\\\",\\\"{GlobalVariables.DragnetDBIP}\\\",\\\"{GlobalVariables.DragnetDBUser}\\\",\\\"{GlobalVariables.DragnetDBPassword}\\\",\\\"{GlobalVariables.DragnetControlIP}\\\",\\\"{GlobalVariables.DragnetControlUser}\\\",\\\"{GlobalVariables.DragnetControlPassword}\\\",\\\"{GlobalVariables.assetIP}\\\",\\\"{GlobalVariables.assetUser}\\\",\\\"{GlobalVariables.assetPW}\\\",\\\"{start}\\\",\\\"{end}\\\",\\\"{newstimeframe}\\\"]}}";
                        FireOffCommand(ip, payload);
                    }

                    // Trends Scraper
                    var trendsBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Trends Scraper");
                    if (trendsBox != null && trendsBox.Checked)
                    {
                        string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TrendsScraperBlockStartTextBox")?.Text.ToLower() ?? "A";
                        string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TrendsScraperBlockEndTextBox")?.Text.ToLower() ?? "Z";
                        string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "TrendsScraperIDTextBox")?.Text.ToLower() ?? "";

                        string payload = $"{{\"script\":\"TrendsScraper.exe\",\"args\":[\"{start}\",\"{end}\",\"{id}\",\"\"]}}";
                        FireOffCommand(ip, payload);
                    }

                    // CapitolTrades
                    var capBox = tab.Controls.OfType<CheckBox>().FirstOrDefault(cb => cb.Text == "Capitol Trades");
                    if (capBox != null && capBox.Checked)
                    {
                        string start = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CapitolTradesBlockStartTextBox")?.Text.ToLower() ?? "A";
                        string end = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CapitolTradesBlockEndTextBox")?.Text.ToLower() ?? "Z";
                        string id = tab.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "CapitolTradesIDTextBox")?.Text.ToLower() ?? "";

                        string payload = $"{{\"script\":\"CapitolTradesModule.exe\",\"args\":[\"{start}\",\"{end}\",\"{id}\",\"\"]}}";
                        FireOffCommand(ip, payload);
                    }
                }
                // Mark BEFORE firing to avoid double-fires if something is slow
                firedThisMinute.Add(key);
            }
        }
        private List<ScheduleRow> LoadScheduleRows()
        {
            var list = new List<ScheduleRow>();
            using var conn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            conn.Open();
            const string sql = "SELECT script_type, trigger_time, days_of_week, calendar_dates FROM dragnet_schedule;";
            using var cmd = new MySqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var dates = new HashSet<string>(StringComparer.Ordinal);
                var raw = rdr["calendar_dates"] as string;
                if (!string.IsNullOrWhiteSpace(raw) && raw.TrimStart().StartsWith("["))
                {
                    try
                    {
                        // Expecting ["2025-08-08","2025-08-15"]
                        var arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(raw);
                        if (arr != null) foreach (var d in arr) dates.Add(d);
                    }
                    catch { /* ignore malformed */ }
                }

                list.Add(new ScheduleRow
                {
                    ScriptType = rdr["script_type"].ToString(),
                    Trigger = TimeSpan.Parse(rdr["trigger_time"].ToString()).Subtract(TimeSpan.FromSeconds(TimeSpan.Parse(rdr["trigger_time"].ToString()).Seconds)),
                    DaysMask = rdr["days_of_week"]?.ToString() ?? "default",
                    CalendarDates = dates
                });
            }
            return list;
        }

        public void StartScheduleTimer()
        {
            scheduleTimer.Interval = 10000; // 5 seconds (or whatever you like)
            scheduleTimer.Tick += scheduleTimer_Tick;
            scheduleTimer.Start();
        }
        private void scheduleTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => CheckAndFireScheduledScripts());
        }
        private void WireAiConfigUi()
        {
            // populate once
            if (AITypeComboBox.Items.Count == 0)
            {
                AITypeComboBox.Items.Add("Select Model Type");
                AITypeComboBox.Items.Add("Generative Transformer (Ollama)"); // phi-3, gpt-oss-20b, llama...
                AITypeComboBox.Items.Add("Encoder-only Transformer (BERT)");
                AITypeComboBox.Items.Add("LSTM Trainer (Time Series)");
            }

            AITypeComboBox.SelectedIndexChanged -= AITypeComboBox_SelectedIndexChanged;
            AITypeComboBox.SelectedIndexChanged += AITypeComboBox_SelectedIndexChanged;

            if (AITypeComboBox.SelectedIndex < 0)
                AITypeComboBox.SelectedIndex = 0; // default
        }

        private void AITypeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            RebuildAiConfigUi(AITypeComboBox.SelectedItem?.ToString() ?? "");
        }

        // --- 2) Rebuild dispatcher ---
        private void RebuildAiConfigUi(string selection)
        {
            var aiTab = dragnetTabControl.TabPages["AiConfigTab"]
                       ?? dragnetTabControl.TabPages.Cast<TabPage>()
                           .First(tp => tp.Text.Equals("AiConfigTab", StringComparison.OrdinalIgnoreCase));

            // Find the combo on the tab (only if it actually lives there)
            var combo = aiTab.Controls.OfType<ComboBox>()
                         .FirstOrDefault(c => c.Name == "AITypeComboBox");

            // Dispose everything EXCEPT the combo (and perhaps its label)
            var toDispose = aiTab.Controls.Cast<Control>()
                .Where(c => !ReferenceEquals(c, combo) && c?.Name != "nodetypelabel")
                .ToList();
            foreach (var c in toDispose) { c.Dispose(); }
            foreach (var c in toDispose) { aiTab.Controls.Remove(c); }

            switch (selection)
            {
                case "Select Model Type":
                    break;
                case "Generative Transformer (Ollama)":
                    BuildOllamaGenerativePane(aiTab);
                    break;
                //case "Encoder-only Transformer (BERT)":
                //BuildBertPane(aiTab);
                //break;
                //case "LSTM Trainer (Time Series)":
                //BuildLstmPane(aiTab);
                //break;
                default:
                    aiTab.Controls.Add(new Label { Left = 12, Top = 12, AutoSize = true, Text = "Select an AI type." });
                    break;
            }

            aiTab.ResumeLayout();
        }

        // --- 3) Builder: Generative Transformer (Ollama) ---
        // --- 3) Builder: Generative Transformer (Ollama) ---
        private void BuildOllamaGenerativePane(TabPage host)
        {
            // ---- tiny inline helpers (stay self-contained) ----
            void Log(TextBox target, string message)
                => target.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");

            string Trunc(string s, int n) => string.IsNullOrEmpty(s) ? s : (s.Length <= n ? s : s.Substring(0, n) + "…");
            int? ParseSeed(string s) => int.TryParse(s, out var v) ? v : (int?)null;

            async Task<string> GenerateAsync(
                string apiBase, string model, string systemPrompt, string userPrompt,
                int numCtx, double temperature, double topP, int? seed, int gpuLayers, int mirostat, string stopCsv)
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

                var opts = new
                {
                    num_ctx = numCtx,
                    temperature,
                    top_p = topP,
                    seed,
                    num_gpu = gpuLayers,
                    mirostat,
                    stop = string.IsNullOrWhiteSpace(stopCsv)
                        ? null
                        : stopCsv.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray()
                };

                var payload = new Dictionary<string, object?>
                {
                    ["model"] = model,
                    ["prompt"] = userPrompt,
                    ["stream"] = false,
                    ["options"] = opts
                };
                if (!string.IsNullOrWhiteSpace(systemPrompt))
                    payload["system"] = systemPrompt;

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                using var req = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await http.PostAsync($"{apiBase}/api/generate", req);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)resp.StatusCode}: {body}");

                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("response", out var r))
                        return r.GetString() ?? "";
                }
                catch { /* fall through */ }

                return body;
            }

            async Task SaveGlobalsToDbAsync(string hostVal, string portVal, string modelVal, int context)
            {
                using var cn = new MySql.Data.MySqlClient.MySqlConnection(GlobalVariables.UsersDBConnect);
                await cn.OpenAsync();

                // Update by username; insert if not present
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    @"UPDATE userdata.users 
              SET LLMHost = @host, LLMPort = @port, LLMModel = @model, LLMContextWindow = @context
              WHERE username = @u;", cn))
                {
                    cmd.Parameters.AddWithValue("@host", hostVal);
                    cmd.Parameters.AddWithValue("@port", portVal);
                    cmd.Parameters.AddWithValue("@model", modelVal);
                    cmd.Parameters.AddWithValue("@context", context);
                    cmd.Parameters.AddWithValue("@u", username);
                    var rows = await cmd.ExecuteNonQueryAsync();

                    if (rows == 0)
                    {
                        using var cmdIns = new MySql.Data.MySqlClient.MySqlCommand(
                            @"INSERT INTO userdata.users (username, LLMHost, LLMPort, LLMModel, LLMContextWindow)
                      VALUES (@u, @host, @port, @model);", cn);
                        cmdIns.Parameters.AddWithValue("@u", username);
                        cmdIns.Parameters.AddWithValue("@host", hostVal);
                        cmdIns.Parameters.AddWithValue("@port", portVal);
                        cmdIns.Parameters.AddWithValue("@model", modelVal);
                        cmdIns.Parameters.AddWithValue("@context", context);
                        await cmdIns.ExecuteNonQueryAsync();
                    }
                }
            }

            // ---- layout helpers (your style) ----
            int y = 12; int xL = 12; int xR = 220; int W = 460; int H = 24; int G = 8;
            y += 50;

            Label L(string t) { var l = new Label { Left = xL, Top = y + 4, Width = 200, Text = t }; host.Controls.Add(l); return l; }
            TextBox T(string val, bool multi = false, int mh = 96)
            {
                var tb = new TextBox { Left = xR, Top = y, Width = W, Height = multi ? mh : H, Multiline = multi, ScrollBars = multi ? ScrollBars.Vertical : ScrollBars.None, Text = val };
                host.Controls.Add(tb); return tb;
            }
            NumericUpDown N(decimal v, decimal min, decimal max, decimal inc = 1, int width = 120)
            {
                var n = new NumericUpDown { Left = xR, Top = y, Width = width, Minimum = min, Maximum = max, Increment = inc, DecimalPlaces = (inc < 1 ? 3 : 0), Value = v };
                host.Controls.Add(n); return n;
            }
            CheckBox C(string text, bool val) { var c = new CheckBox { Left = xR, Top = y, Width = 260, Text = text, Checked = val }; host.Controls.Add(c); return c; }

            // ---- Fields (prefill from GlobalVariables) ----
            L("LLM Host"); var txtHost = T(GlobalVariables.LLMHost ?? "localhost"); y += H + G;
            L("LLM Port"); var txtPort = T(GlobalVariables.LLMPort ?? "11434"); y += H + G;
            L("Model (name:tag)"); var model = T(GlobalVariables.LLMModel ?? "phi3"); y += H + G;

            // Derived API base (read-only)
            Func<string> ApiBase = () => $"http://{txtHost.Text.Trim()}:{txtPort.Text.Trim()}";
            L($"Ollama API Base (user: {username})"); var apiBaseBox = T(ApiBase(), false); apiBaseBox.ReadOnly = true; y += H + G;
            txtHost.TextChanged += (_, __) => apiBaseBox.Text = ApiBase();
            txtPort.TextChanged += (_, __) => apiBaseBox.Text = ApiBase();

            L("Context (num_ctx)"); var numCtx = N(GlobalVariables.LLMContextWindow, 256, 131072); y += H + G;
            L("Temperature"); var temp = N(0.7m, 0, 2, 0.05m); y += H + G;
            L("Top-p"); var topP = N(0.9m, 0, 1, 0.05m); y += H + G;

            L("Seed (blank = random)"); var seedBox = T(""); y += H + G;
            L("GPU Layers"); var gpuLayers = N(999, 0, 5000); y += H + G;
            L("Mirostat (0/1/2)"); var mirostat = N(0, 0, 2); y += H + G;

            L("Stop sequences (comma)"); var stopSeqs = T(""); y += H + G;

            var sticky = C("Use sticky System Prompt", false); y += H + G;
            L("System Prompt (optional)"); var sysPrompt = T("", multi: true); y += 96 + G;

            var warm = C("Warm on Start (send warmup prompt)", true); y += H + G;
            L("Warmup Prompt"); var warmPrompt = T("ping"); y += H + G;

            // Prompt input + Query button
            L("Prompt"); var promptBox = T("", multi: true, mh: 96); y += 96 + G;

            // Buttons
            var btnHealth = new Button { Left = xL, Top = y, Width = 90, Height = 28, Text = "Health" };
            var btnStart = new Button { Left = xL + 100, Top = y, Width = 90, Height = 28, Text = "Start" };
            var btnStop = new Button { Left = xL + 200, Top = y, Width = 90, Height = 28, Text = "Stop" };
            var btnTest = new Button { Left = xL + 300, Top = y, Width = 100, Height = 28, Text = "Test Prompt" };
            var btnQuery = new Button { Left = xL + 410, Top = y, Width = 90, Height = 28, Text = "Query" };
            var btnSave = new Button { Left = xL + 510, Top = y, Width = 90, Height = 28, Text = "Save" };
            host.Controls.AddRange(new Control[] { btnHealth, btnStart, btnStop, btnTest, btnQuery, btnSave });
            y += 36 + G;

            var log = new TextBox { Left = xL, Top = y, Width = xR + W - xL, Height = 120, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };
            host.Controls.Add(log);

            // ---- Actions ----
            btnHealth.Click += async (_, __) =>
            {
                try
                {
                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                    var r = await http.GetAsync($"{ApiBase()}/api/version");
                    var body = await r.Content.ReadAsStringAsync();
                    Log(log, r.IsSuccessStatusCode ? $"[HEALTH OK] {body}" : $"[HEALTH FAIL] {r.StatusCode} {body}");
                }
                catch (Exception ex) { Log(log, $"[HEALTH ERROR] {ex.Message}"); }
            };

            btnStart.Click += async (_, __) =>
            {
                try
                {
                    if (warm.Checked)
                    {
                        var resp = await GenerateAsync(ApiBase(), model.Text, sticky.Checked ? sysPrompt.Text : null, warmPrompt.Text,
                                                       (int)numCtx.Value, (double)temp.Value, (double)topP.Value,
                                                       ParseSeed(seedBox.Text), (int)gpuLayers.Value, (int)mirostat.Value, stopSeqs.Text);
                        Log(log, $"[START] Warmed '{model.Text}'. Sample: {Trunc(resp, 140)}");
                    }
                    else
                    {
                        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                        var tags = await http.GetStringAsync($"{ApiBase()}/api/tags");
                        Log(log, $"[START] Runtime ready. Tags: {Trunc(tags, 140)}");
                    }
                }
                catch (Exception ex) { Log(log, $"[START ERROR] {ex.Message}"); }
            };

            btnStop.Click += async (_, __) =>
            {
                try
                {
                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                    var r = await http.PostAsync($"{ApiBase()}/api/stop", new StringContent("{}", Encoding.UTF8, "application/json"));
                    Log(log, r.IsSuccessStatusCode ? "[STOP] Requested stop." : $"[STOP FAIL] {r.StatusCode}");
                }
                catch (Exception ex) { Log(log, $"[STOP ERROR] {ex.Message}"); }
            };

            btnTest.Click += async (_, __) =>
            {
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    var outp = await GenerateAsync(ApiBase(), model.Text, sticky.Checked ? sysPrompt.Text : null,
                                                   "Say 'ready' once.", (int)numCtx.Value, (double)temp.Value, (double)topP.Value,
                                                   ParseSeed(seedBox.Text), (int)gpuLayers.Value, (int)mirostat.Value, stopSeqs.Text);
                    sw.Stop();
                    var toks = Math.Max(1, outp.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length);
                    var tps = toks / Math.Max(0.001, sw.Elapsed.TotalSeconds);
                    Log(log, $"[TEST] {toks} toks in {sw.ElapsedMilliseconds} ms (~{tps:0.0} tok/s). Output: {outp.Trim()}");
                }
                catch (Exception ex) { Log(log, $"[TEST ERROR] {ex.Message}"); }
            };

            btnQuery.Click += async (_, __) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(promptBox.Text))
                    {
                        Log(log, "[QUERY] Prompt is empty.");
                        return;
                    }
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    var outp = await GenerateAsync(ApiBase(), model.Text, sticky.Checked ? sysPrompt.Text : null,
                                                   promptBox.Text, (int)numCtx.Value, (double)temp.Value, (double)topP.Value,
                                                   ParseSeed(seedBox.Text), (int)gpuLayers.Value, (int)mirostat.Value, stopSeqs.Text);
                    sw.Stop();
                    Log(log, $"[QUERY OK] {Trunc(outp.Trim(), 4000)}");
                }
                catch (Exception ex) { Log(log, $"[QUERY ERROR] {ex.Message}"); }
            };

            btnSave.Click += async (_, __) =>
            {
                try
                {
                    var h = txtHost.Text.Trim();
                    var p = txtPort.Text.Trim();
                    var m = model.Text.Trim();
                    var c = (int)numCtx.Value;

                    await SaveGlobalsToDbAsync(h, p, m, c);

                    // reflect into globals
                    GlobalVariables.LLMHost = h;
                    GlobalVariables.LLMPort = p;
                    GlobalVariables.LLMModel = m;
                    GlobalVariables.LLMContextWindow = c;

                    apiBaseBox.Text = ApiBase();
                    Log(log, $"[SAVE] Settings saved for user '{username}' and applied to GlobalVariables.");
                }
                catch (Exception ex) { Log(log, $"[SAVE ERROR] {ex.Message}"); }
            };
        }



        // --- 4) Tiny helpers used above ---
        private static int? ParseSeed(string s) => int.TryParse(s, out var v) ? v : (int?)null;

        private static void Log(TextBox tb, string msg) =>
            tb.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");

        private static string Trunc(string s, int max) =>
            string.IsNullOrEmpty(s) || s.Length <= max ? s : s[..max] + "…";

        private static async Task<string> OllamaGenerate(
            string apiBase, string model, string? systemPrompt, string prompt,
            int numCtx, double temp, double topP, int? seed, int gpuLayers, int mirostat, string stopCsv)
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            var stop = stopCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => t.Trim()).ToArray();
            var payload = new
            {
                model,
                prompt = string.IsNullOrWhiteSpace(systemPrompt) ? prompt : $"{systemPrompt}\n\n{prompt}",
                stream = false,
                options = new
                {
                    num_ctx = numCtx,
                    temperature = temp,
                    top_p = topP,
                    seed = seed,
                    gpu_layers = gpuLayers,
                    mirostat = mirostat,
                    stop = stop.Length > 0 ? stop : null
                }
            };
            var req = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await http.PostAsync($"{apiBase}/api/generate", req);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode) throw new Exception($"{resp.StatusCode}: {body}");
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            return doc.RootElement.TryGetProperty("response", out var r) ? r.GetString() ?? "" : body;
        }
        private async Task LoadPromptListAsync(string? filter = null)
        {
            const string sqlBase = @"
SELECT name, version, is_active
FROM dragnetcontrol.prompt_registry
/**where**/
ORDER BY name ASC, updated_at DESC;";

            var where = string.IsNullOrWhiteSpace(filter) ? "" : "WHERE name LIKE @q OR version LIKE @q";

            using var cn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            await cn.OpenAsync();
            using var cmd = new MySqlCommand(sqlBase.Replace("/**where**/", where), cn);
            if (!string.IsNullOrWhiteSpace(filter))
                cmd.Parameters.AddWithValue("@q", $"%{filter}%");

            var items = new List<PromptListItem>();
            using (var rd = await cmd.ExecuteReaderAsync())
                while (await rd.ReadAsync())
                    items.Add(new PromptListItem
                    {
                        Name = rd.GetString("name"),
                        Version = rd.GetString("version"),
                        IsActive = rd.GetBoolean("is_active")
                    });

            // (optional) preserve selection
            var sel = promptListBox.SelectedItem as PromptListItem;

            promptListBox.BeginUpdate();
            promptListBox.Items.Clear();
            foreach (var it in items) promptListBox.Items.Add(it);
            promptListBox.EndUpdate();

            // (optional) reselect previously selected item
            if (sel is not null)
            {
                for (int i = 0; i < promptListBox.Items.Count; i++)
                {
                    if (promptListBox.Items[i] is PromptListItem it &&
                        it.Name.Equals(sel.Name, StringComparison.OrdinalIgnoreCase) &&
                        it.Version.Equals(sel.Version, StringComparison.OrdinalIgnoreCase))
                    {
                        promptListBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private async Task LoadSelectedPromptIntoEditorAsync()
        {
            if (promptListBox.SelectedItem is not PromptListItem it) return;

            const string sql = @"
SELECT name, version, `text` AS prompt_text
FROM dragnetcontrol.prompt_registry
WHERE name=@n AND version=@v
LIMIT 1;";

            using var cn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            await cn.OpenAsync();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@n", it.Name);
            cmd.Parameters.AddWithValue("@v", it.Version);

            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                txtPromptName.Text = rd.GetString("name");
                numPromptVersion.Text = rd.GetString("version");   // allow non-numeric
                var body = rd.GetString("prompt_text");
                txtPromptBody.Text = body;
                lblBodyLength.Text = $"{body.Length:N0} chars, {txtPromptBody.Lines.Length:N0} lines";
            }
        }


        private void ClearEditor()
        {
            txtPromptName.Text = "";
            numPromptVersion.Value = 1;
            lblBodyLength.Text = "";
            txtPromptBody.Text = "";
        }
        private async Task ReloadCurrentAsync()
        {
            var name = txtPromptName.Text.Trim();
            var version = GetVersionText();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(version)) return;

            const string sql = @"
SELECT `text`
FROM dragnetcontrol.prompt_registry
WHERE name=@n AND version=@v
LIMIT 1;";

            using var cn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            await cn.OpenAsync();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@v", version);

            var result = await cmd.ExecuteScalarAsync();
            if (result is string body)
            {
                txtPromptBody.Text = body;
                lblBodyLength.Text = $"{body.Length:N0} chars, {txtPromptBody.Lines.Length:N0} lines";
            }
        }

        private string GetVersionText()
        {
            // If user typed non-numeric in the control (we set Text earlier), honor that.
            var t = numPromptVersion.Text?.Trim();
            if (!string.IsNullOrEmpty(t)) return t;
            return numPromptVersion.Value.ToString("0");
        }
        private async Task SaveCurrentAsync()
        {
            var name = txtPromptName.Text.Trim();
            var version = GetVersionText();
            var body = txtPromptBody.Text ?? "";   // <-- from editor
            var makeActive = false;                // wire to a checkbox if you add one
            var createdBy = Environment.UserName;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(version))
            {
                MessageBox.Show("Prompt Name and Version are required.");
                return;
            }

            var checksum = Sha256Hex(body);

            const string upsert = @"
INSERT INTO dragnetcontrol.prompt_registry
  (name, version, `text`, checksum, created_by)
VALUES
  (@n, @v, @t, @c, @u)
ON DUPLICATE KEY UPDATE
  `text`      = VALUES(`text`),
  checksum    = VALUES(checksum),
  created_by  = VALUES(created_by),
  updated_at  = CURRENT_TIMESTAMP;";

            using var cn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            await cn.OpenAsync();
            using var tx = await cn.BeginTransactionAsync();

            try
            {
                using (var cmd = new MySqlCommand(upsert, cn, (MySqlTransaction)tx))
                {
                    cmd.Parameters.Add("@n", MySqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@v", MySqlDbType.VarChar).Value = version;
                    cmd.Parameters.Add("@t", MySqlDbType.LongText).Value = body;   // <-- important
                    cmd.Parameters.Add("@c", MySqlDbType.VarChar).Value = checksum;
                    cmd.Parameters.Add("@u", MySqlDbType.VarChar).Value = createdBy;
                    await cmd.ExecuteNonQueryAsync();
                }

                if (makeActive)
                {
                    using (var cmd0 = new MySqlCommand(
                        "UPDATE dragnetcontrol.prompt_registry SET is_active=0 WHERE name=@n;", cn, (MySqlTransaction)tx))
                    { cmd0.Parameters.AddWithValue("@n", name); await cmd0.ExecuteNonQueryAsync(); }

                    using (var cmd1 = new MySqlCommand(
                        "UPDATE dragnetcontrol.prompt_registry SET is_active=1 WHERE name=@n AND version=@v;", cn, (MySqlTransaction)tx))
                    { cmd1.Parameters.AddWithValue("@n", name); cmd1.Parameters.AddWithValue("@v", version); await cmd1.ExecuteNonQueryAsync(); }
                }

                await tx.CommitAsync();
                await LoadPromptListAsync();
                lblStatus.Text = $"Saved prompt '{name} ({version})' successfully.";
                ReselectPromptInList(name, version);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                lblStatus.Text = ($"Save failed:\n{ex.Message}");
            }
        }

        private void ReselectPromptInList(string name, string version)
        {
            for (int i = 0; i < promptListBox.Items.Count; i++)
            {
                if (promptListBox.Items[i] is PromptListItem it &&
                    it.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    it.Version.Equals(version, StringComparison.OrdinalIgnoreCase))
                {
                    promptListBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private async Task DeleteCurrentAsync()
        {
            var name = txtPromptName.Text.Trim();
            var version = GetVersionText();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(version)) return;

            var confirm = MessageBox.Show($"Delete prompt '{name} ({version})' ?", "Confirm delete",
                                          MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (confirm != DialogResult.OK) return;

            const string sql = @"DELETE FROM dragnetcontrol.prompt_registry WHERE name=@n AND version=@v;";
            using var cn = new MySqlConnection(GlobalVariables.ControlDBConnect);
            await cn.OpenAsync();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@v", version);
            await cmd.ExecuteNonQueryAsync();

            await LoadPromptListAsync();
            lblStatus.Text = "Prompt deleted successfully.";
            ClearEditor();
        }
        private void WireUpEventsOnce()
        {
            if (_eventsWired) return;
            _eventsWired = true;

            promptListBox.SelectedIndexChanged += async (_, __) => await LoadSelectedPromptIntoEditorAsync();
            btnNewPrompt.Click += (_, __) => ClearEditor();
            btnReloadPrompt.Click += async (_, __) => await ReloadCurrentAsync();
            btnSavePrompt.Click += async (_, __) => await SaveCurrentAsync();
            btnDeletePrompt.Click += async (_, __) => await DeleteCurrentAsync();

            // optional search support if you added txtSearchPrompt
            if (Controls.Find("txtSearchPrompt", true) is { Length: > 0 } found &&
                found[0] is TextBox txtSearchPrompt)
            {
                txtSearchPrompt.TextChanged += async (_, __) =>
                    await LoadPromptListAsync(txtSearchPrompt.Text);
            }
        }

        private async Task SaveActivePromptAsync(string promptName, string promptVersion)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Cannot save: 'username' is empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(promptName) || string.IsNullOrWhiteSpace(promptVersion))
            {
                MessageBox.Show("Prompt Name and Version are required.");
                return;
            }

            const string updateSql = @"
UPDATE userdata.users
SET `LLMPromptName` = @pn,
    `LLMPromptVersion` = @pv
WHERE username = @u
LIMIT 1;";

            try
            {
                using var cn = new MySql.Data.MySqlClient.MySqlConnection(GlobalVariables.UsersDBConnect);
                await cn.OpenAsync();

                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(updateSql, cn)
                {
                    CommandTimeout = 10
                };
                cmd.Parameters.AddWithValue("@u", username.Trim());
                cmd.Parameters.AddWithValue("@pn", promptName);
                cmd.Parameters.AddWithValue("@pv", promptVersion);

                var rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                {
                    // No matching user row — do NOT insert (per your requirement).
                    MessageBox.Show(
                        $"No existing user row for '{username}'. Not creating a new one.",
                        "Update skipped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GlobalVariables.ActiveLLMPrompt = promptName;
                GlobalVariables.ActiveLLMPromptVersion = promptVersion;
                if (lblStatus != null)
                    lblStatus.Text = $"Updated active prompt '{promptName} ({promptVersion})' for '{username}'.";
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                    lblStatus.Text = $"Save failed: {ex.Message}";
                else
                    MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void setActivePrompt_Click(object sender, EventArgs e)
        {
            var promptName = txtPromptName.Text.Trim();
            var version = GetVersionText();

            if (string.IsNullOrWhiteSpace(promptName) || string.IsNullOrWhiteSpace(version))
            {
                MessageBox.Show("Prompt Name and Version are required.");
                return;
            }

            await SaveActivePromptAsync(promptName, version);
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}



 


