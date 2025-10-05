using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DragnetControl.Configuration;
using MySqlConnector;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DragnetControl
{
    public partial class Authentication : Form
    {
        private readonly DragnetConfiguration _configuration;
        private readonly ConfigurationLoader _configurationLoader;

        public Authentication(DragnetConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _configurationLoader = new ConfigurationLoader(_configuration);
            InitializeComponent();
        }

        private bool CheckCredentials(string username, string passwordAttempt, out bool dbError, out int accountStatus)
        {
            dbError = false;
            accountStatus = 0;

            using var conn = new MySqlConnection(_configuration.UsersDatabase.BuildConnectionString());
            try
            {
                conn.Open();

                const string query = "SELECT * FROM users WHERE username = @username";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                using var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    return false;
                }

                reader.Read();
                var storedPasswordHash = reader.GetString("password");
                var status = reader.GetInt32("accountstatus");

                if (IsUnsaltedPassword(storedPasswordHash))
                {
                    if (!VerifyUnsaltedPassword(passwordAttempt, storedPasswordHash))
                    {
                        return false;
                    }

                    accountStatus = status;

                    if (accountStatus == 1 || accountStatus == 2)
                    {
                        using var changePasswordForm = new PasswordChangeForm(username, _configuration.UsersDatabase);
                        changePasswordForm.ShowDialog();
                    }

                    return true;
                }

                var isValid = BCryptNet.Verify(passwordAttempt, storedPasswordHash);
                if (isValid)
                {
                    accountStatus = status;
                }

                return isValid;
            }
            catch (MySqlException ex)
            {
                dbError = true;

                MessageBox.Show(
                    "Unable to connect to the Users database.\n\nDetails:\n" + ex.Message,
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        private static bool IsUnsaltedPassword(string storedPassword)
        {
            return !storedPassword.StartsWith("$2", StringComparison.Ordinal) || storedPassword.Length != 60;
        }

        private static bool VerifyUnsaltedPassword(string passwordAttempt, string storedPassword)
        {
            if (storedPassword.Equals(passwordAttempt, StringComparison.Ordinal))
            {
                return true;
            }

            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(passwordAttempt);
            var hashBytes = md5.ComputeHash(inputBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
            return hash == storedPassword.ToLowerInvariant();
        }

        private void Authentication_Load(object sender, EventArgs e)
        {
            try
            {
                using var conn = new MySqlConnection(_configuration.UsersDatabase.BuildConnectionString());
                conn.Open();
                toolStripStatusLabel1.ForeColor = System.Drawing.Color.Cyan;
                toolStripStatusLabel1.Text = "User Database Connection Established";
            }
            catch (MySqlException)
            {
                toolStripStatusLabel1.ForeColor = System.Drawing.Color.Red;
                toolStripStatusLabel1.Text = "Cannot Connect to the Database";
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            var username = UsernameBox.Text;
            var passwordAttempt = PasswordBox.Text;

            var ok = CheckCredentials(username, passwordAttempt, out var dbError, out var accountStatus);

            if (dbError)
            {
                InformationLabel.ForeColor = System.Drawing.Color.Red;
                InformationLabel.Text = "Database connection failed.";
                PasswordBox.Text = string.Empty;
                return;
            }

            if (ok)
            {
                if (accountStatus == 3)
                {
                    InformationLabel.ForeColor = System.Drawing.Color.Red;
                    InformationLabel.Text = "Account Locked/Suspended.";
                    PasswordBox.Text = string.Empty;
                    return;
                }

                if (accountStatus == 1 || accountStatus == 2)
                {
                    await LoadConfigurationAndLaunchAsync(username).ConfigureAwait(true);
                }

                return;
            }

            InformationLabel.ForeColor = System.Drawing.Color.Red;
            InformationLabel.Text = "Invalid username or password.";
            PasswordBox.Text = string.Empty;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainControl_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (RememberUserNameCheckBox.Checked)
            {
                PasswordBox.Text = string.Empty;
            }
            else
            {
                UsernameBox.Text = string.Empty;
                PasswordBox.Text = string.Empty;
            }

            InformationLabel.Text = "Enter Credentials:";
            AccessRequestLabel.Show();
            ToggleLoginControls(true);
            ShowLoadingIndicators(false);
            Show();
        }

        private void ConnectionStatusStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private async Task LoadConfigurationAndLaunchAsync(string username)
        {
            InformationLabel.ForeColor = System.Drawing.Color.Black;
            InformationLabel.Text = "Login Successful. Loading configuration files.";
            PasswordBox.Text = string.Empty;
            AccessRequestLabel.Hide();
            ToggleLoginControls(false);
            ShowLoadingIndicators(true);

            try
            {
                var progress = new Progress<ConfigurationProgress>(UpdateConfigurationProgress);
                var sessionState = await _configurationLoader.LoadAsync(username, progress).ConfigureAwait(true);
                GlobalVariables.Initialize(_configuration, sessionState);
                FormManager.Configure(() => new MainControl());
                var mainControl = FormManager.MainControl;
                mainControl.FormClosed += MainControl_FormClosed;
                Hide();
                ShowLoadingIndicators(false);
                ToggleLoginControls(true);
                mainControl.Show();
            }
            catch (Exception ex)
            {
                ShowLoadingIndicators(false);
                ToggleLoginControls(true);
                AccessRequestLabel.Show();
                InformationLabel.ForeColor = System.Drawing.Color.Red;
                InformationLabel.Text = "Failed to load configuration.";
                MessageBox.Show(
                    this,
                    $"Unable to load configuration data.\n\nDetails:\n{ex.Message}",
                    "Configuration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ToggleLoginControls(bool enabled)
        {
            LoginButton.Enabled = enabled;
            ExitButton.Enabled = enabled;
            UsernameBox.Enabled = enabled;
            PasswordBox.Enabled = enabled;
            RememberUserNameCheckBox.Enabled = enabled;
            AccessRequestLabel.Enabled = enabled;
        }

        private void ShowLoadingIndicators(bool visible)
        {
            ConfigurationProgressBar.Visible = visible;
            ConfigurationStatusLabel.Visible = visible;
            if (visible)
            {
                ConfigurationProgressBar.Value = ConfigurationProgressBar.Minimum;
                ConfigurationStatusLabel.Text = "Preparing configuration...";
            }
            else
            {
                ConfigurationStatusLabel.Text = string.Empty;
            }
        }

        private void UpdateConfigurationProgress(ConfigurationProgress progress)
        {
            var value = Math.Max(ConfigurationProgressBar.Minimum, Math.Min(ConfigurationProgressBar.Maximum, progress.Value));
            ConfigurationProgressBar.Value = value;
            ConfigurationStatusLabel.Text = progress.Message;
        }
    }
}
