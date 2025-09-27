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
    public partial class SMScannerEdit : Form
    {
        private Form _systemMap;

        string username = GlobalVariables.username;
        public SMScannerEdit(Form System_Map)
        {
            InitializeComponent();
            _systemMap = ParentForm;
            this.Owner = _systemMap;
            hostTextBox.Text = GlobalVariables.smIP;
            usernameTextBox.Text = GlobalVariables.smUser;
            passwordBox.Text = GlobalVariables.smPW;
            pathbox.Text = GlobalVariables.smpath;
            portbox1.Text = GlobalVariables.smPort1.ToString();
            portbox2.Text = GlobalVariables.smPort2.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(GlobalVariables.UsersDBConnect))
            {
                int port1;
                int port2;
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET smip = @smip, smuser = @smuser, smpw = @smpw, smpath = @smpath, smport1 = @smport1, smport2 = @smport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@smip", hostTextBox.Text);
                        GlobalVariables.smIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@smuser", usernameTextBox.Text);
                        GlobalVariables.smUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@smpw", passwordBox.Text);
                        GlobalVariables.smPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@smpath", pathbox.Text);
                        GlobalVariables.smpath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@smport1", port1);
                        GlobalVariables.smPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@smport2", port2);
                        GlobalVariables.smPort2 = port2;


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
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET smip = @smip, smuser = @smuser, smpw = @smpw, smpath = @smpath, smport1 = @smport1, smport2 = @smport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@smip", hostTextBox.Text);
                        GlobalVariables.smIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@smuser", usernameTextBox.Text);
                        GlobalVariables.smUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@smpw", passwordBox.Text);
                        GlobalVariables.smPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@smpath", pathbox.Text);
                        GlobalVariables.smpath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@smport1", port1);
                        GlobalVariables.smPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@smport2", port2);
                        GlobalVariables.smPort2 = port2;
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

    


