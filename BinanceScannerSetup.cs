using System;
using System.Windows.Forms;
using MySqlConnector;

namespace DragnetControl
{
    public partial class BinanceScannerSetup : Form
    {
       

        string username = GlobalVariables.username;
        public BinanceScannerSetup()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.BinanceHost;
            usernameTextBox.Text = GlobalVariables.BinanceUser;
            passwordBox.Text = GlobalVariables.BinancePW;
            pathbox.Text = GlobalVariables.BinancePath;
            portbox1.Text = GlobalVariables.BinancePort1.ToString();
            portbox2.Text = GlobalVariables.BinancePort2.ToString();
            ApiKeyTextBox.Text = GlobalVariables.BinanceAPI;
            delayTextBox.Text = GlobalVariables.CryptoDelay.ToString();
            ApiSecretTextBox.Text = GlobalVariables.BinanceSecret;
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
                    string updateQuery = "UPDATE users SET BinanceHost = @BinanceHost, BinanceUser = @BinanceUser, BinancePW = @BinancePW, BinancePath = @Binancepath, BinancePort1 = @BinancePort1, BinancePort2 = @BinancePort2, BinanceAPI = @BinanceAPI, BinanceSecret = @BinanceSecret, CryptoDelay = @CryptoDelay, CryptoTimeframe = @CryptoTimeFrame, CryptoGranularity = @CryptoGranularity WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@BinanceHost", hostTextBox.Text);
                        GlobalVariables.BinanceHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@BinanceUser", usernameTextBox.Text);
                        GlobalVariables.BinanceUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@BinancePW", passwordBox.Text);
                        GlobalVariables.BinancePW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@BinancePath", pathbox.Text);
                        GlobalVariables.BinancePath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@BinancePort1", port1);
                        GlobalVariables.BinancePort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@BinancePort2", port2);
                        GlobalVariables.BinancePort2 = port2;
                        cmd.Parameters.AddWithValue("@BinanceAPI", ApiKeyTextBox.Text);
                        GlobalVariables.BinanceAPI = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@BinanceSecret", ApiSecretTextBox.Text);
                        GlobalVariables.BinanceSecret = ApiSecretTextBox.Text;
                        cmd.Parameters.AddWithValue("@CryptoDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        GlobalVariables.CryptoDelay = delay;
                        cmd.Parameters.AddWithValue("@CryptoTimeFrame", timespantextbox.Text);
                        int.TryParse(timespantextbox.Text, out TimeSpan);
                        GlobalVariables.CryptoTimeSpan = TimeSpan;
                        cmd.Parameters.AddWithValue("@cryptogranularity", granularitytextbox.Text);
                        int.TryParse(granularitytextbox.Text, out granularity);
                        GlobalVariables.CryptoGranularity = granularity;



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
                    string updateQuery = "UPDATE users SET BinanceHost = @BinanceHost, BinanceUser = @BinanceUser, BinancePW = @BinancePW, BinancePath = @BinancePath, BinancePort1 = @BinancePort1, BinancePort2 = @BinancePort2, BinanceAPI = @BinanceAPI, BinanceSecret = @BinanceSecret, CryptoDelay = @CryptoDelay, CryptoTimeframe = @CryptoTimeFrame, CryptoGranularity = @CryptoGranularity WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@BinanceHost", hostTextBox.Text);
                        GlobalVariables.BinanceHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@BinanceUser", usernameTextBox.Text);
                        GlobalVariables.BinanceUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@BinancePW", passwordBox.Text);
                        GlobalVariables.BinancePW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@BinancePath", pathbox.Text);
                        GlobalVariables.BinancePath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@BinancePort1", port1);
                        GlobalVariables.BinancePort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@BinancePort2", port2);
                        GlobalVariables.BinancePort2 = port2;
                        cmd.Parameters.AddWithValue("@BinanceAPI", ApiKeyTextBox.Text);
                        GlobalVariables.BinanceAPI = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@BinanceSecret", ApiSecretTextBox.Text);
                        GlobalVariables.BinanceSecret = ApiSecretTextBox.Text;
                        cmd.Parameters.AddWithValue("@CryptoDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        GlobalVariables.CryptoDelay = delay;
                        cmd.Parameters.AddWithValue("@CryptoTimeFrame", timespantextbox.Text);
                        int.TryParse(timespantextbox.Text, out TimeSpan);
                        GlobalVariables.CryptoTimeSpan = TimeSpan;
                        cmd.Parameters.AddWithValue("@cryptogranularity", granularitytextbox.Text);
                        int.TryParse(granularitytextbox.Text, out granularity);
                        GlobalVariables.CryptoGranularity = granularity;


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

        private void BinanceScannerSetup_Load(object sender, EventArgs e)
        {

        }
    }
}

    


