using System;
using System.Windows.Forms;
using MySqlConnector;

namespace DragnetControl
{
    public partial class KrakenScannerSetup : Form
    {
        string username = GlobalVariables.username;
        public KrakenScannerSetup()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.KrakenScannerHost;
            usernameTextBox.Text = GlobalVariables.KrakenScannerUser;
            passwordBox.Text = GlobalVariables.KrakenScannerPW;
            pathbox.Text = GlobalVariables.KrakenPath;
            portbox1.Text = GlobalVariables.KrakenScannerPort1.ToString();
            portbox2.Text = GlobalVariables.KrakenScannerPort2.ToString();
            ApiKeyTextBox.Text = GlobalVariables.KrakenAPIKey;
            delayTextBox.Text = GlobalVariables.CryptoDelay.ToString();
            ApiSecretTextBox.Text = GlobalVariables.KrakenPrivateKey;
            timespantextbox.Text = GlobalVariables.CryptoTimeSpan.ToString();
            granularitytextbox.Text = GlobalVariables.CryptoGranularity.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                int port1;
                int port2;
                int delay;
                int granularity;
                int TimeSpan;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET KrakenHost = @KrakenHost, KrakenScannerUser = @KrakenScannerUser, KrakenScannerPW = @KrakenScannerPW, KrakenPath = @KrakenPath, KrakenScannerPort1 = @KrakenScannerPort1, KrakenScannerPort2 = @KrakenScannerPort2, KrakenAPI = @KrakenAPI, KrakenPrivateKey = @KrakenPrivateKey, CryptoDelay = @CryptoDelay, CryptoTimeframe = @CryptoTimeFrame, CryptoGranularity = @CryptoGranularity WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@KrakenHost", hostTextBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenScannerUser", usernameTextBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenScannerPW", passwordBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenPath", pathbox.Text);
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@KrakenScannerPort1", port1);
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@KrakenScannerPort2", port2);
                        cmd.Parameters.AddWithValue("@KrakenAPI", ApiKeyTextBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenPrivateKey", ApiSecretTextBox.Text);
                        cmd.Parameters.AddWithValue("@CryptoDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        cmd.Parameters.AddWithValue("@CryptoTimeFrame", timespantextbox.Text);
                        int.TryParse(timespantextbox.Text, out TimeSpan);
                        cmd.Parameters.AddWithValue("@cryptogranularity", granularitytextbox.Text);
                        int.TryParse(granularitytextbox.Text, out granularity);
                        GlobalVariables.UpdateSessionState(state =>
                        {
                            state.KrakenScannerHost = hostTextBox.Text;
                            state.KrakenScannerUser = usernameTextBox.Text;
                            state.KrakenScannerPassword = passwordBox.Text;
                            state.KrakenPath = pathbox.Text;
                            state.KrakenScannerPort1 = port1;
                            state.KrakenScannerPort2 = port2;
                            state.KrakenApiKey = ApiKeyTextBox.Text;
                            state.KrakenPrivateKey = ApiSecretTextBox.Text;
                            state.CryptoDelay = delay;
                            state.CryptoTimeSpan = TimeSpan;
                            state.CryptoGranularity = granularity;
                        });



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
                int granularity;
                int TimeSpan;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET KrakenHost = @KrakenHost, KrakenScannerUser = @KrakenScannerUser, KrakenScannerPW = @KrakenScannerPW, KrakenPath = @KrakenPath, KrakenScannerPort1 = @KrakenScannerPort1, KrakenScannerPort2 = @KrakenScannerPort2, KrakenAPI = @KrakenAPI, KrakenPrivateKey = @KrakenPrivateKey, CryptoDelay = @CryptoDelay, CryptoTimeframe = @CryptoTimeFrame, CryptoGranularity = @CryptoGranularity WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@KrakenHost", hostTextBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenScannerUser", usernameTextBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenScannerPW", passwordBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenPath", pathbox.Text);
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@KrakenScannerPort1", port1);
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@KrakenScannerPort2", port2);
                        cmd.Parameters.AddWithValue("@KrakenAPI", ApiKeyTextBox.Text);
                        cmd.Parameters.AddWithValue("@KrakenPrivateKey", ApiSecretTextBox.Text);
                        cmd.Parameters.AddWithValue("@CryptoDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        cmd.Parameters.AddWithValue("@CryptoTimeFrame", timespantextbox.Text);
                        int.TryParse(timespantextbox.Text, out TimeSpan);
                        cmd.Parameters.AddWithValue("@cryptogranularity", granularitytextbox.Text);
                        int.TryParse(granularitytextbox.Text, out granularity);
                        GlobalVariables.UpdateSessionState(state =>
                        {
                            state.KrakenScannerHost = hostTextBox.Text;
                            state.KrakenScannerUser = usernameTextBox.Text;
                            state.KrakenScannerPassword = passwordBox.Text;
                            state.KrakenPath = pathbox.Text;
                            state.KrakenScannerPort1 = port1;
                            state.KrakenScannerPort2 = port2;
                            state.KrakenApiKey = ApiKeyTextBox.Text;
                            state.KrakenPrivateKey = ApiSecretTextBox.Text;
                            state.CryptoDelay = delay;
                            state.CryptoTimeSpan = TimeSpan;
                            state.CryptoGranularity = granularity;
                        });


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


      
    }
}

    


