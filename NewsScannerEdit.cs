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
    public partial class NewsScannerEdit : Form
    {
        private Form _systemMap;

        string username = GlobalVariables.username;
        public NewsScannerEdit(Form System_Map)
        {
            InitializeComponent();
            _systemMap = ParentForm;
            this.Owner = _systemMap;
            hostTextBox.Text = GlobalVariables.newsIP;
            usernameTextBox.Text = GlobalVariables.newsUser;
            passwordBox.Text = GlobalVariables.newsPW;
            pathbox.Text = GlobalVariables.newspath;
            portbox1.Text = GlobalVariables.newsport1.ToString();
            portbox2.Text = GlobalVariables.newsport2.ToString();
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
                    string updateQuery = "UPDATE users SET newsip = @newsip, newsuser = @newsuser, newspw = @newspw, newspath = @newspath, newsport1 = @newsport1, newsport2 = @newsport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@newsip", hostTextBox.Text);
                        GlobalVariables.newsIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@newsuser", usernameTextBox.Text);
                        GlobalVariables.newsUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@newspw", passwordBox.Text);
                        GlobalVariables.newsPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@newspath", pathbox.Text);
                        GlobalVariables.newspath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@newsport1", port1);
                        GlobalVariables.newsport1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@newsport2", port2);
                        GlobalVariables.newsport2 = port2;


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
                    string updateQuery = "UPDATE users SET newsip = @newsip, newsuser = @newsuser, newspw = @newspw, newspath = @newspath, newsport1 = @newsport1, newsport2 = @newsport2 WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@newsip", hostTextBox.Text);
                        GlobalVariables.newsIP = hostTextBox.Text;
                        cmd.Parameters.AddWithValue("@newsuser", usernameTextBox.Text);
                        GlobalVariables.newsUser = usernameTextBox.Text;
                        cmd.Parameters.AddWithValue("@newspw", passwordBox.Text);
                        GlobalVariables.newsPW = passwordBox.Text;
                        cmd.Parameters.AddWithValue("@newspath", pathbox.Text);
                        GlobalVariables.newspath = pathbox.Text;
                        int.TryParse(portbox1.Text, out port1);
                        cmd.Parameters.AddWithValue("@newsport1", port1);
                        GlobalVariables.newsport1 = port1;
                        int.TryParse(portbox2.Text, out port2);
                        cmd.Parameters.AddWithValue("@newsport2", port2);
                        GlobalVariables.newsport2 = port2;
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

    


