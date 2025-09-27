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
    public partial class CoinbaseScannerSetup : Form
    {

        string username = GlobalVariables.username;
        public CoinbaseScannerSetup()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.CoinbaseScannerHost;
            usernameTextBox.Text = GlobalVariables.CoinbaseScannerUser;
            passwordBox.Text = GlobalVariables.CoinbaseScannerPW;
            pathbox.Text = GlobalVariables.CoinbaseScannerPath;
            portbox1.Text = GlobalVariables.CoinbaseScannerPort1.ToString();
            portbox2.Text = GlobalVariables.CoinbaseScannerPort2.ToString();
            ApiKeyTextBox.Text = GlobalVariables.coinbaseAPIKey;
            delayTextBox.Text = GlobalVariables.CryptoDelay.ToString();
            ApiSecretTextBox.Text = GlobalVariables.CoinbaseSecret;
            passphrasetextbox.Text = GlobalVariables.CoinbasePassphrase;
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
                    string updateQuery = "UPDATE users SET CBScannerHost = @CBScannerHost, CBScannerUser = @CBScannerUser, CBScannerPW = @CBScannerPW, CBScannerPath = @CBScannerPath, CBScannerPort1 = @CBScannerPort1, CBScannerPort2 = @CBScannerPort2, CoinbaseAPIKey = @CoinbaseAPIKey, CoinbaseSecret = @CoinbaseSecret, CoinbasePassphrase = @CoinbasePassphrase, CryptoDelay = @CryptoDelay, CryptoTimeframe = @CryptoTimeFrame, CryptoGranularity = @CryptoGranularity WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@CBScannerHost", hostTextBox.Text);
                        GlobalVariables.CoinbaseScannerHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@CBScannerUser", usernameTextBox.Text);
                        GlobalVariables.CoinbaseScannerUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@CBScannerPW", passwordBox.Text);
                        GlobalVariables.CoinbaseScannerPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@CBScannerPath", pathbox.Text);
                        GlobalVariables.CoinbaseScannerPath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@CBScannerPort1", port1);
                        GlobalVariables.CoinbaseScannerPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@CBScannerPort2", port2);
                        GlobalVariables.CoinbaseScannerPort2 = port2;
                        cmd.Parameters.AddWithValue("@CoinbaseAPIKey", ApiKeyTextBox.Text);
                        GlobalVariables.coinbaseAPIKey = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@CoinbaseSecret", ApiSecretTextBox.Text);
                        GlobalVariables.CoinbaseSecret = ApiSecretTextBox.Text;
                        cmd.Parameters.AddWithValue("CoinbasePassphrase", passphrasetextbox.Text);
                        GlobalVariables.CoinbasePassphrase = passphrasetextbox.Text;
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
                    string updateQuery = "UPDATE users SET CBScannerHost = @CBScannerHost, CBScannerUser = @CBScannerUser, CBScannerPW = @CBScannerPW, CBScannerPath = @CBScannerPath, CBScannerPort1 = @CBScannerPort1, CBScannerPort2 = @CBScannerPort2, CoinbaseAPIKey = @CoinbaseAPIKey, CoinbaseSecret = @CoinbaseSecret, CoinbasePassphrase = @CoinbasePassphrase, CryptoDelay = @CryptoDelay, CryptoTimeframe = @CryptoTimeFrame, CryptoGranularity = @CryptoGranularity WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@CBScannerHost", hostTextBox.Text);
                        GlobalVariables.CoinbaseScannerHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@CBScannerUser", usernameTextBox.Text);
                        GlobalVariables.CoinbaseScannerUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@CBScannerPW", passwordBox.Text);
                        GlobalVariables.CoinbaseScannerPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@CBScannerPath", pathbox.Text);
                        GlobalVariables.CoinbaseScannerPath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@CBScannerPort1", port1);
                        GlobalVariables.CoinbaseScannerPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@CBScannerPort2", port2);
                        GlobalVariables.CoinbaseScannerPort2 = port2;
                        cmd.Parameters.AddWithValue("@CoinbaseAPIKey", ApiKeyTextBox.Text);
                        GlobalVariables.coinbaseAPIKey = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@CoinbaseSecret", ApiSecretTextBox.Text);
                        GlobalVariables.CoinbaseSecret = ApiSecretTextBox.Text;
                        cmd.Parameters.AddWithValue("CoinbasePassphrase", passphrasetextbox.Text);
                        GlobalVariables.CoinbasePassphrase = passphrasetextbox.Text;
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


      
    }
}

    


