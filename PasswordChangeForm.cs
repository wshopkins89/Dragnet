using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DragnetControl.Configuration;
using MySqlConnector;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DragnetControl
{
    public partial class PasswordChangeForm : Form
    {
        private readonly string _username;
        private readonly DatabaseCredentials _credentials;

        public PasswordChangeForm(string username, DatabaseCredentials credentials)
        {
            InitializeComponent();
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            UsernameLabel.Text = "Username: " + _username;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                var oldPassword = OldPasswordTextBox.Text;
                var newPassword = NewPasswordTextBox.Text;
                var newPassword2 = NewPasswordTextBox2.Text;

                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                {
                    MessageLabel.Text = "New password must be at least 8 characters long.";
                    return;
                }

                if (!string.Equals(newPassword, newPassword2, StringComparison.Ordinal))
                {
                    MessageLabel.Text = "New passwords do not match.";
                    return;
                }

                var newPasswordHash = BCryptNet.HashPassword(newPassword);

                using var conn = new MySqlConnection(_credentials.BuildConnectionString());
                conn.Open();

                const string query = "SELECT password FROM users WHERE Username = @username";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", _username);

                using var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    MessageLabel.Text = "The user does not exist.";
                    return;
                }

                reader.Read();
                var storedPasswordHash = reader.GetString("Password");

                var isVerified = IsBcryptHash(storedPasswordHash)
                    ? BCryptNet.Verify(oldPassword, storedPasswordHash)
                    : VerifyUnsaltedPassword(oldPassword, storedPasswordHash);

                if (!isVerified)
                {
                    MessageLabel.Text = "The old password is incorrect.";
                    return;
                }

                reader.Close();

                const string updateQuery = "UPDATE users SET Password = @newPassword WHERE username = @username";
                using var updateCmd = new MySqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@newPassword", newPasswordHash);
                updateCmd.Parameters.AddWithValue("@username", _username);

                var rowsAffected = updateCmd.ExecuteNonQuery();
                MessageLabel.Text = rowsAffected > 0
                    ? "Password changed successfully."
                    : "Password change failed. No rows were updated.";
            }
            catch (MySqlException ex)
            {
                MessageLabel.Text = "An error occurred while changing the password: " + ex.Message;
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "An unexpected error occurred: " + ex.Message;
            }
        }

        private void PasswordChangeForm_Load(object sender, EventArgs e)
        {
        }

        private void CloseLabel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static bool IsBcryptHash(string storedPassword)
        {
            return storedPassword.StartsWith("$2", StringComparison.Ordinal) && storedPassword.Length == 60;
        }

        private static bool VerifyUnsaltedPassword(string passwordAttempt, string storedPassword)
        {
            if (string.Equals(passwordAttempt, storedPassword, StringComparison.Ordinal))
            {
                return true;
            }

            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(passwordAttempt);
            var hashBytes = md5.ComputeHash(inputBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
            return hash == storedPassword.ToLowerInvariant();
        }
    }
}
