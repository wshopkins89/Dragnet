using System.Security.Cryptography;
using System.Text;
using YourNamespace;
using MySqlConnector;

namespace DragnetControl
{
    public partial class Authentication : Form
    {
        public Authentication()
        {
            InitializeComponent();
            GlobalVariables.UsersDBIP = "localhost";
            GlobalVariables.UsersDBUsername = "dragnet";
            GlobalVariables.usersdbPW = "dragnet5";
            GlobalVariables.UsersDBConnect =
                $"server={GlobalVariables.UsersDBIP};uid={GlobalVariables.UsersDBUsername};password={GlobalVariables.usersdbPW};database=userdata";
        }

        private string passwordAttempt;

        private bool CheckCredentials(string username, string passwordAttempt, out bool dbError)
        {
            dbError = false;

            using (var conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                try
                {
                    conn.Open();

                    const string query = "SELECT * FROM users WHERE username = @username";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // BUGFIX: bind the method parameter, not the global
                        cmd.Parameters.AddWithValue("@username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                return false;

                            reader.Read();
                            string storedPasswordHash = reader.GetString("password");

                            // If not a BCrypt hash, treat as legacy/unsalted
                            if (IsUnsaltedPassword(storedPasswordHash))
                            {
                                // If your legacy was plaintext:
                                if (storedPasswordHash == passwordAttempt)
                                {
                                    // Force password change
                                    using (var changePasswordForm = new PasswordChangeForm(username))
                                    {
                                        changePasswordForm.ShowDialog();
                                    }
                                    GlobalVariables.accountstatus = reader.GetInt32("accountstatus");
                                    return true;
                                }

                                return false;
                            }
                            else
                            {
                                // BCrypt verification
                                bool ok = BCrypt.Net.BCrypt.Verify(passwordAttempt, storedPasswordHash);
                                if (ok)
                                {
                                    GlobalVariables.accountstatus = reader.GetInt32("accountstatus");
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    dbError = true;

                    // POPUP on login attempt failure due to DB connectivity (or any MySQL error)
                    MessageBox.Show(
                        "Unable to connect to the Users database.\n\nDetails:\n" + ex.Message,
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return false;
                }
            }
        }

        // Helper: consider non-BCrypt (not $2*) and not length 60 as "unsalted/legacy"
        private bool IsUnsaltedPassword(string storedPassword)
        {
            return !storedPassword.StartsWith("$2") || storedPassword.Length != 60;
        }

        private bool VerifyUnsaltedPassword(string passwordAttempt, string storedPassword)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(passwordAttempt);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return hash == storedPassword;
            }
        }

        private void Authentication_Load(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
                {
                    conn.Open();
                    toolStripStatusLabel1.ForeColor = System.Drawing.Color.Cyan;
                    toolStripStatusLabel1.Text = "User Database Connection Established";
                }
            }
            catch (MySqlException)
            {
                toolStripStatusLabel1.ForeColor = System.Drawing.Color.Red;
                toolStripStatusLabel1.Text = "Cannot Connect to the Database";
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            GlobalVariables.username = UsernameBox.Text;
            passwordAttempt = PasswordBox.Text;

            bool dbError;
            bool ok = CheckCredentials(GlobalVariables.username, passwordAttempt, out dbError);

            if (dbError)
            {
                // We already showed a MessageBox in CheckCredentials.
                // Make the UI reflect the failure state:
                InformationLabel.ForeColor = System.Drawing.Color.Red;
                InformationLabel.Text = "Database connection failed.";
                PasswordBox.Text = "";
                return;
            }

            if (ok)
            {
                if (GlobalVariables.accountstatus == 3)
                {
                    InformationLabel.ForeColor = System.Drawing.Color.Red;
                    InformationLabel.Text = "Account Locked/Suspended.";
                    PasswordBox.Text = "";
                }
                else if (GlobalVariables.accountstatus == 1 || GlobalVariables.accountstatus == 2)
                {
                    InformationLabel.ForeColor = System.Drawing.Color.Black;
                    InformationLabel.Text = "Login Successful. Loading configuration files.";
                    PasswordBox.Text = "";
                    AccessRequestLabel.Hide();
                    this.Hide();

                    using (var assetLoad = new AssetLoadingScreen())
                    {
                        assetLoad.ShowDialog();
                    }

                    FormManager.MainControl.FormClosed += MainControl_FormClosed;
                }
            }
            else
            {
                InformationLabel.ForeColor = System.Drawing.Color.Red;
                InformationLabel.Text = "Invalid username or password.";
                PasswordBox.Text = "";
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (RememberUserNameCheckBox.Checked)
            {
                PasswordBox.Text = "";
            }
            else
            {
                UsernameBox.Text = "";
                PasswordBox.Text = "";
            }
            InformationLabel.Text = "Enter Credentials:";
            AccessRequestLabel.Show();
            this.Show();
        }

        private void ConnectionStatusStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
    }
}