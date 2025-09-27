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
    public partial class TrendsScannerEdit : Form
    {
        private Form _systemMap;

        string username = GlobalVariables.username;
        public TrendsScannerEdit(Form System_Map)
        {
            InitializeComponent();
            _systemMap = ParentForm;
            this.Owner = _systemMap;
            hostTextBox.Text = GlobalVariables.trendsIP;
            usernameTextBox.Text = GlobalVariables.trendsUser;
            passwordBox.Text = GlobalVariables.trendsPW;
            pathbox.Text = GlobalVariables.trendspath;
            portbox1.Text = GlobalVariables.trendsPort1.ToString();
            portbox2.Text = GlobalVariables.trendsPort2.ToString();
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
                    string updateQuery = "UPDATE users SET trendsip = @trendsip, trendsuser = @trendsuser, trendspw = @trendspw, trendspath = @trendspath, trendsport1 = @trendsport1, trendsport2 = @trendsport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@trendsip", hostTextBox.Text);
                        GlobalVariables.trendsIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@trendsuser", usernameTextBox.Text);
                        GlobalVariables.trendsUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@trendspw", passwordBox.Text);
                        GlobalVariables.trendsPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@trendspath", pathbox.Text);
                        GlobalVariables.trendspath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@trendsport1", port1);
                        GlobalVariables.trendsPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@trendsport2", port2);
                        GlobalVariables.trendsPort2 = port2;


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
                    string updateQuery = "UPDATE users SET trendsip = @trendsip, trendsuser = @trendsuser, trendspw = @trendspw, trendspath = @trendspath, trendsport1 = @trendsport1, trendsport2 = @trendsport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@trendsip", hostTextBox.Text);
                        GlobalVariables.trendsIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@trendsuser", usernameTextBox.Text);
                        GlobalVariables.trendsUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@trendspw", passwordBox.Text);
                        GlobalVariables.trendsPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@trendspath", pathbox.Text);
                        GlobalVariables.trendspath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@trendsport1", port1);
                        GlobalVariables.trendsPort1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@trendsport2", port2);
                        GlobalVariables.trendsPort2 = port2;
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

    


