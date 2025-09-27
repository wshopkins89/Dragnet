using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using YourNamespace;

namespace DragnetControl
{
    public partial class Authentication : Form
    {
        public Authentication()
        {
            InitializeComponent();
            GlobalVariables.UsersDBIP = "192.168.1.210";
            GlobalVariables.UsersDBUsername = "dragnet";
            GlobalVariables.usersdbPW = "dragnet5";
            GlobalVariables.UsersDBConnect = $"server={GlobalVariables.UsersDBIP};uid={GlobalVariables.UsersDBUsername};password={GlobalVariables.usersdbPW};database=userdata";
        }

        private string passwordAttempt;
       
        private bool CheckCredentials(string username, string passwordAttempt)
        {
            using (MySqlConnection conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM users WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", GlobalVariables.username);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                string storedPasswordHash = reader.GetString("password");

                                // Check if the stored password is unsalted (i.e., plaintext)
                                if (IsUnsaltedPassword(storedPasswordHash))
                                {
                                    // Check if the plaintext password matches
                                    if (storedPasswordHash == passwordAttempt)
                                    {
                                        // Force a password change
                                        PasswordChangeForm changePasswordForm = new PasswordChangeForm(GlobalVariables.username);
                                        changePasswordForm.ShowDialog();

                                        GlobalVariables.accountstatus = reader.GetInt32("accountstatus");
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    // Verify the password using BCrypt
                                    bool isVerified = BCrypt.Net.BCrypt.Verify(passwordAttempt, storedPasswordHash);
                                    if (isVerified)
                                    {
                                        GlobalVariables.accountstatus = reader.GetInt32("accountstatus");
                                        return true;
                                    }
                                }

                                return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // Helper method to determine if the password is unsalted (plaintext)
        private bool IsUnsaltedPassword(string storedPassword)
        {
            // Check if the password is not a valid BCrypt hash (i.e., it's plaintext)
            // BCrypt hashes typically start with $2a$, $2b$, $2x$, or $2y$ and are 60 characters long
            return !storedPassword.StartsWith("$2") || storedPassword.Length != 60;
        }


        // Verifies an unsalted password
        private bool VerifyUnsaltedPassword(string passwordAttempt, string storedPassword)
        {
            // Assuming storedPassword is an MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(passwordAttempt);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return hash == storedPassword;
            }
        }

        // Checks if a string is a valid hexadecimal string
        private bool IsHexString(string input)
        {
            foreach (char c in input)
            {
                bool isHexChar = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
                if (!isHexChar)
                    return false;
            }
            return true;
        }

        private void Authentication_Load(object sender, EventArgs e)
        {
            MySqlConnection conn;
            try
            {
                conn = new MySqlConnection(GlobalVariables.UsersDBConnect);
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

        private void LoginButton_Click(object sender, EventArgs e)
        {
            GlobalVariables.username = UsernameBox.Text;
            passwordAttempt = PasswordBox.Text;

            if (CheckCredentials(GlobalVariables.username, passwordAttempt))
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
                    AssetLoadingScreen assetLoad = new AssetLoadingScreen();
                    assetLoad.ShowDialog();

                    FormManager.MainControl.FormClosed += new FormClosedEventHandler(MainControl_FormClosed);
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
            if (RememberUserNameCheckBox.Checked == true)
            {
                PasswordBox.Text = "";
            }
            else if (RememberUserNameCheckBox.Checked == false)
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
