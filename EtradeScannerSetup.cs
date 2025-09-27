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
    public partial class EtradeScannerSetup : Form
    {
        string username = GlobalVariables.username;
        public EtradeScannerSetup()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.EtradeHost;
            usernameTextBox.Text = GlobalVariables.EtradeUser;
            passwordBox.Text = GlobalVariables.EtradePW;
            pathbox.Text = GlobalVariables.ScriptsPath;
            portbox1.Text = GlobalVariables.EtradePort1.ToString();
            portbox2.Text = GlobalVariables.EtradePort2.ToString();
            ApiKeyTextBox.Text = GlobalVariables.EtradeAPIKey;
            delayTextBox.Text = GlobalVariables.EtradeDelay.ToString();
            ApiSecretTextBox.Text = GlobalVariables.EtradeAPISecret;
            timespantextbox.Text = GlobalVariables.EtradeTimespan.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;uid=root;password=password" + ";database=userdata"))
            {
                int port1;
                int port2;
                int delay;
                decimal TimeSpan;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET EtradeHost = @EtradeHost, EtradeUser = @EtradeUser, EtradePW = @EtradePW, EtradePort1 = @EtradePort1, EtradePort2 = @EtradePort2, EtradePath = @EtradePath, EtradeAPIKey = @EtradeAPIKey, EtradeAPIsecret = @EtradeAPISecret, Etradedelay = @EtradeDelay, Etradetimespan = @EtradeTimespan WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@EtradeHost", hostTextBox.Text);
                        GlobalVariables.EtradeHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradeUser", usernameTextBox.Text);
                        GlobalVariables.EtradeUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradePW", passwordBox.Text);
                        GlobalVariables.EtradePW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@EtradePath", pathbox.Text);
                        GlobalVariables.ScriptsPath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@EtradePort1", port1);
                        GlobalVariables.EtradePort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@EtradePort2", port2);
                        GlobalVariables.EtradePort2 = port2;
                        cmd.Parameters.AddWithValue("@EtradeAPI", ApiKeyTextBox.Text);
                        GlobalVariables.EtradeAPIKey = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradeAPISecret", ApiSecretTextBox.Text);
                        GlobalVariables.EtradeAPISecret = ApiSecretTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradeDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        GlobalVariables.EtradeDelay = delay;
                        cmd.Parameters.AddWithValue("@EtradeTimespan", timespantextbox.Text);
                        decimal.TryParse(timespantextbox.Text, out TimeSpan);
                        GlobalVariables.EtradeTimespan = TimeSpan;



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
            using (MySqlConnection conn = new MySqlConnection("server=localhost;uid=root;password=password" + ";database=userdata"))
            {
                int port1;
                int port2;
                int delay;
                decimal TimeSpan;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET EtradeHost = @EtradeHost, EtradeUser = @EtradeUser, EtradePW = @EtradePW, EtradePort1 = @EtradePort1, EtradePort2 = @EtradePort2, EtradePath = @EtradePath, EtradeAPIKey = @EtradeAPIKey, EtradeAPIsecret = @EtradeAPISecret, Etradedelay = @EtradeDelay, Etradetimespan = @EtradeTimespan WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@EtradeHost", hostTextBox.Text);
                        GlobalVariables.EtradeHost = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradeUser", usernameTextBox.Text);
                        GlobalVariables.EtradeUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradePW", passwordBox.Text);
                        GlobalVariables.EtradePW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@EtradePath", pathbox.Text);
                        GlobalVariables.ScriptsPath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@EtradePort1", port1);
                        GlobalVariables.EtradePort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@EtradePort2", port2);
                        GlobalVariables.EtradePort2 = port2;
                        cmd.Parameters.AddWithValue("@EtradeAPIKey", ApiKeyTextBox.Text);
                        GlobalVariables.EtradeAPIKey = ApiKeyTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradeAPISecret", ApiSecretTextBox.Text);
                        GlobalVariables.EtradeAPISecret = ApiSecretTextBox.Text;
                        cmd.Parameters.AddWithValue("@EtradeDelay", delayTextBox.Text);
                        int.TryParse(delayTextBox.Text, out delay);
                        GlobalVariables.EtradeDelay = delay;
                        cmd.Parameters.AddWithValue("@EtradeTimespan", timespantextbox.Text);
                        decimal.TryParse(timespantextbox.Text, out TimeSpan);
                        GlobalVariables.EtradeTimespan = TimeSpan;

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

        private void M1ScannerEdit_Load(object sender, EventArgs e)
        {

        }
    }
}

    


