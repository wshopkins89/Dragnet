using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using DragnetControl.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;

namespace DragnetControl
{
    public partial class AssetDBEdit : Form
    {
        string username = GlobalVariables.username;
        public AssetDBEdit()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.assetIP;
            usernameTextBox.Text = GlobalVariables.assetUser;
            passwordBox.Text = GlobalVariables.assetPW;
            databasebox.Text = GlobalVariables.assetDBName;
            portbox1.Text = GlobalVariables.assetport1.ToString();
            portbox2.Text = GlobalVariables.assetport2.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseSettings.Current.UsersConnectionString))
            {
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET assetIP = @assetIP, assetuser = @assetuser, assetpw = @assetpw assetdbname = @assetdbname, assetport1 = @assetport1, assetport2 = @assetport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@assetIP", hostTextBox.Text);
                        cmd.Parameters.AddWithValue("@assetuser", usernameTextBox.Text);
                        cmd.Parameters.AddWithValue("@assetpw", passwordBox.Text);
                        cmd.Parameters.AddWithValue("@assetdbname", databasebox.Text);
                        int port1;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@AssetPort1", port1);
                        int port2;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@AssetPort2", port2);
                        GlobalVariables.UpdateSessionState(state =>
                        {
                            state.AssetDatabase = new Configuration.DatabaseCredentials(
                                hostTextBox.Text,
                                usernameTextBox.Text,
                                passwordBox.Text,
                                databasebox.Text,
                                port1,
                                port2);
                            state.AssetDbConnectionString = state.AssetDatabase.BuildConnectionString();
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
            using (MySqlConnection conn = new MySqlConnection(DatabaseSettings.Current.UsersConnectionString))
            {
                int port1;
                int port2;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET assetIP = @assetIP, assetuser = @assetuser, assetpw = @assetpw, assetdbname = @assetdbname, assetport1 = @assetport1, assetport2 = @assetport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@assetIP", hostTextBox.Text);
                        cmd.Parameters.AddWithValue("@assetuser", usernameTextBox.Text);
                        cmd.Parameters.AddWithValue("@assetpw", passwordBox.Text);
                        cmd.Parameters.AddWithValue("@assetdbname", databasebox.Text);
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@assetport1", port1);
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@assetport2", port2);
                        GlobalVariables.UpdateSessionState(state =>
                        {
                            state.AssetDatabase = new Configuration.DatabaseCredentials(
                                hostTextBox.Text,
                                usernameTextBox.Text,
                                passwordBox.Text,
                                databasebox.Text,
                                port1,
                                port2);
                            state.AssetDbConnectionString = state.AssetDatabase.BuildConnectionString();
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

        private void AssetDBEdit_Load(object sender, EventArgs e)
        {

        }
    }
}

