using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using MySqlConnector;
using BCrypt.Net;

namespace YourNamespace
{
    public partial class PasswordChangeForm : Form
    {
        private string userName;

        // Constructor that accepts the username
        public PasswordChangeForm(string username)
        {
            InitializeComponent();
            userName = username;
            UsernameLabel.Text = "Username: " + userName;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string oldPassword = OldPasswordTextBox.Text;
                string newPassword = NewPasswordTextBox.Text;
                string newPassword2 = NewPasswordTextBox2.Text;

                // Validation logic here...

                // Hash the new password
                string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (MySqlConnection conn = new MySqlConnection("server=localhost;uid=root;password=password;database=userdata"))
                {
                    conn.Open();

                    // Verify the old password
                    string query = "SELECT password FROM users WHERE Username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", userName);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                string storedPasswordHash = reader.GetString("Password");

                                bool isVerified;
                                if (storedPasswordHash.StartsWith("$2") && storedPasswordHash.Length == 60)
                                {
                                    // Verify BCrypt password
                                    isVerified = BCrypt.Net.BCrypt.Verify(oldPassword, storedPasswordHash);
                                }
                                else
                                {
                                    // Verify plaintext password
                                    isVerified = oldPassword == storedPasswordHash;
                                }

                                if (!isVerified)
                                {
                                    MessageLabel.Text = "The old password is incorrect.";
                                    return;
                                }
                            }
                            else
                            {
                                MessageLabel.Text = "The user does not exist.";
                                return;
                            }
                        }
                    }

                    // Update the password
                    string updateQuery = "UPDATE users SET Password = @newPassword WHERE username = @username";
                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@newPassword", newPasswordHash);
                        updateCmd.Parameters.AddWithValue("@username", userName);

                        // Check how many rows were affected
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageLabel.Text = "Password changed successfully.";
                        }
                        else
                        {
                            MessageLabel.Text = "Password change failed. No rows were updated.";
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageLabel.Text = "An error occurred while changing the password: " + ex.Message;
                Console.WriteLine("Error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "An unexpected error occurred: " + ex.Message;
                Console.WriteLine("Error: " + ex.ToString());
            }
        }


        private void PasswordChangeForm_Load(object sender, EventArgs e)
        {
            // Load event logic, if needed
        }

        private void CloseLabel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

