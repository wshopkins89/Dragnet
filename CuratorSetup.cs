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
using DragnetControl.Infrastructure.Configuration;

namespace DragnetControl
{
    public partial class CuratorSetup : Form
    {


        string username = GlobalVariables.username;
        public CuratorSetup(Form System_Map)
        {
            InitializeComponent();
    
            hostTextBox.Text = GlobalVariables.DragnetDBIP;
            usernameTextBox.Text = GlobalVariables.DragnetDBUser;
            passwordBox.Text = GlobalVariables.DragnetDBPassword;
            databasebox.Text = GlobalVariables.DragnetDBName;
            portbox1.Text = GlobalVariables.DragnetPort1.ToString();
            portbox2.Text = GlobalVariables.DragnetPort2.ToString();
            pathTextBox.Text = GlobalVariables.CuratorPath;
            delaytextbox.Text = GlobalVariables.CurationDelayTime.ToString();
            archivaltimetextbox.Text = GlobalVariables.CurationHistoryTime.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseSettings.Current.UsersConnectionString))
            {
                int port1;
                int port2;
                int delay;
                int archivetime;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET dragnetip = @dragnetip, dragnetuser = @dragnetuser, dragnetpassword = @dragnetpassword, dragnetdbname = @dragnetdbname, dragnetport1 = @dragnetport1, dragnetport2 = @dragnetport2, curationdelaytime = @curationdelaytime, curationhistorytime = @curationhistorytime, curatorpath = @curatorpath WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@dragnetip", hostTextBox.Text);
                        GlobalVariables.DragnetDBIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@dragnetuser", usernameTextBox.Text);
                        GlobalVariables.DragnetDBUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@dragnetpassword", passwordBox.Text);
                        GlobalVariables.DragnetDBPassword = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@dragnetdbname", databasebox.Text);
                        GlobalVariables.DragnetDBName = databasebox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@dragnetport1", port1);
                        GlobalVariables.DragnetPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@dragnetport2", port2);
                        GlobalVariables.DragnetPort2 = port2;
                        cmd.Parameters.AddWithValue("@curatorpath", pathTextBox.Text);
                        GlobalVariables.CuratorPath = pathTextBox.Text;
                        cmd.Parameters.AddWithValue("curationdelaytime", delaytextbox.Text);
                        int.TryParse(delaytextbox.Text, out delay);
                        GlobalVariables.CurationDelayTime = delay;
                        cmd.Parameters.AddWithValue("@curationhistorytime", archivaltimetextbox.Text);
                        int.TryParse(archivaltimetextbox.Text, out archivetime);
                        GlobalVariables.CurationHistoryTime = archivetime;

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
                int delay;
                int archivetime;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET dragnetip = @dragnetip, dragnetuser = @dragnetuser, dragnetpassword = @dragnetpassword, dragnetdbname = @dragnetdbname, dragnetport1 = @dragnetport1, dragnetport2 = @dragnetport2, curationdelaytime = @curationdelaytime, curationhistorytime = @curationhistorytime, curatorpath = @curatorpath WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@dragnetip", hostTextBox.Text);
                        GlobalVariables.DragnetDBIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@dragnetuser", usernameTextBox.Text);
                        GlobalVariables.DragnetDBUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@dragnetpassword", passwordBox.Text);
                        GlobalVariables.DragnetDBPassword = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@dragnetdbname", databasebox.Text);
                        GlobalVariables.DragnetDBName = databasebox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@dragnetport1", port1);
                        GlobalVariables.DragnetPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@dragnetport2", port2);
                        GlobalVariables.DragnetPort2 = port2;
                        cmd.Parameters.AddWithValue("@curatorpath", pathTextBox.Text);
                        GlobalVariables.CuratorPath = pathTextBox.Text;
                        cmd.Parameters.AddWithValue("curationdelaytime", delaytextbox.Text);
                        int.TryParse(delaytextbox.Text, out delay);
                        GlobalVariables.CurationDelayTime = delay;
                        cmd.Parameters.AddWithValue("@curationhistorytime", archivaltimetextbox.Text);
                        int.TryParse(archivaltimetextbox.Text, out archivetime);
                        GlobalVariables.CurationHistoryTime = archivetime;
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

