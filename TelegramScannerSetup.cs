using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;

namespace DragnetControl
{
    public partial class TelegramScannerSetup : Form
    {
        string username = GlobalVariables.username;
        public TelegramScannerSetup()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.TelegramHost;
            usernameTextBox.Text = GlobalVariables.TelegramUser;
            passwordBox.Text = GlobalVariables.TelegramPW;
            phonenumberbox.Text = GlobalVariables.PhoneNumber;
            ApiKeyTextBox.Text = GlobalVariables.TelegramAPIKey;
            delayTextBox.Text = GlobalVariables.TelegramDelay.ToString();
            ApiHashTextBox.Text = GlobalVariables.TelegramAPIHash;
            timespantextbox.Text = GlobalVariables.TelegramTimespan.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                int port1;
                int port2;
                int delay;
                int TimeSpan;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET TelegramHost = @TelegramHost, TelegramUser = @TelegramUser, TelegramPW = @TelegramPW, TelegramAPIID = @TelegramAPIID, TelegramAPIHash = @TelegramAPIHash, Telegramdelay = @TelegramDelay, Telegramtimespan = @TelegramTimespan WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@TelegramHost", hostTextBox.Text);
                        GlobalVariables.TelegramHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramUser", usernameTextBox.Text);
                        GlobalVariables.TelegramUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramPW", passwordBox.Text);
                        GlobalVariables.TelegramPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramPath", phonenumberbox.Text);
                        GlobalVariables.TelephoneNumber = phonenumberbox.Text;
                        cmd.Parameters.AddWithValue("@TelegramAPIID", ApiKeyTextBox.Text);
                        GlobalVariables.TelegramAPIKey = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramAPIHash", ApiHashTextBox.Text);
                        GlobalVariables.TelegramAPIHash = ApiHashTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        GlobalVariables.TelegramDelay = delay;
                        cmd.Parameters.AddWithValue("@TelegramTimespan", timespantextbox.Text);
                        int.TryParse(timespantextbox.Text, out TimeSpan);
                        GlobalVariables.TelegramTimespan = TimeSpan;



                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();
                        if (rowsAffected > 0)
                        {
                            updatelabel.Text = "Changes Applied.";
                        }
                        else
                        {
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }

            }
        }

        private void SaveandCloseButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                int port1;
                int port2;
                int delay;
                int TimeSpan;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET TelegramHost = @TelegramHost, TelegramUser = @TelegramUser, TelegramPW = @TelegramPW, TelegramAPIID = @TelegramAPIID, TelegramAPIHash = @TelegramAPIHash, Telegramdelay = @TelegramDelay, Telegramtimespan = @TelegramTimespan WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@TelegramHost", hostTextBox.Text);
                        GlobalVariables.TelegramHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramUser", usernameTextBox.Text);
                        GlobalVariables.TelegramUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramPW", passwordBox.Text);
                        GlobalVariables.TelegramPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramPath", phonenumberbox.Text);
                        GlobalVariables.TelephoneNumber = phonenumberbox.Text;
                        cmd.Parameters.AddWithValue("@TelegramAPIID", ApiKeyTextBox.Text);
                        GlobalVariables.TelegramAPIKey = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramAPIHash", ApiHashTextBox.Text);
                        GlobalVariables.TelegramAPIHash = ApiHashTextBox.Text;
                        cmd.Parameters.AddWithValue("@TelegramDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        GlobalVariables.TelegramDelay = delay;
                        cmd.Parameters.AddWithValue("@TelegramTimespan", timespantextbox.Text);
                        int.TryParse(timespantextbox.Text, out TimeSpan);
                        GlobalVariables.TelegramTimespan = TimeSpan;

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        this.Close();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }

            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TelegramScannerSetup_Load(object sender, EventArgs e)
        {

        }
    }
}

    


