using System;
using System.Windows.Forms;
using MySqlConnector;

namespace DragnetControl
{
    public partial class DragnetDBEdit : Form
    {
        string username = GlobalVariables.username;
        public DragnetDBEdit()
        {
            InitializeComponent();
            hostTextBox.Text = GlobalVariables.DragnetDBIP;
            usernameTextBox.Text = GlobalVariables.DragnetDBUser;
            passwordBox.Text = GlobalVariables.DragnetDBPassword;
            databasebox.Text = GlobalVariables.DragnetDBName;
            portbox1.Text = GlobalVariables.DragnetPort1.ToString();
            portbox2.Text = GlobalVariables.DragnetPort2.ToString();
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
                    string updateQuery = "UPDATE users SET dragnetip = @dragnetip, dragnetuser = @dragnetuser, dragnetpassword = @dragnetpassword, dragnetdbname = @dragnetdbname, dragnetport1 = @dragnetport1, dragnetport2 = @dragnetport2 WHERE username = @username";
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
                    string updateQuery = "UPDATE users SET dragnetip = @dragnetip, dragnetuser = @dragnetuser, dragnetpassword = @dragnetpassword, dragnetdbname = @dragnetdbname, dragnetport1 = @dragnetport1, dragnetport2 = @dragnetport2 WHERE username = @username";
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

