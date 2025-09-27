using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;

namespace DragnetControl
{
    public partial class AddNode : Form
    {
        // TODO: Replace with your real connection string
        private string connectionString = $"Server={GlobalVariables.DragnetControlIP};Database=dragnetcontrol;Uid={GlobalVariables.DragnetControlUser};Pwd={GlobalVariables.DragnetControlPassword};";

        private void AddNode_Load(object sender, EventArgs e)
        {

        }
        public AddNode()
        {
            InitializeComponent();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            string ip = ipTextBox.Text.Trim();
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            string portStr = portTextBox.Text.Trim();
            string note = noteTextBox.Text.Trim();

            int port = 0;
            int cpuScore = 0;
            int ramScore = 0;
            if (cpuScoreComboBox.SelectedItem != null)
                int.TryParse(cpuScoreComboBox.SelectedItem.ToString(), out cpuScore);
            if (RAMScoreComboBox.SelectedItem != null)
                int.TryParse(RAMScoreComboBox.SelectedItem.ToString(), out ramScore);
            bool enabled = enabledCheckBox.Checked;

            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("IP Address is required.", "Error");
                return;
            }
            if (!int.TryParse(portStr, out port))
            {
                MessageBox.Show("Invalid port number.", "Error");
                return;
            }

            // Check for existing IP
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string checkSql = "SELECT COUNT(*) FROM dragnet_nodes WHERE ip_address = @ip";
                using (var cmd = new MySqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    var exists = Convert.ToInt32(cmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        MessageBox.Show("Node with this IP already exists!", "Duplicate");
                        return;
                    }
                }

                // Insert new node
                string insertSql = @"
                    INSERT INTO dragnet_nodes
                    (ip_address, username, password, port, enabled, cpu_score, ram_score, note)
                    VALUES
                    (@ip, @username, @password, @port, @enabled, @cpu_score, @ram_score, @note)
                ";
                using (var cmd = new MySqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@port", port);
                    cmd.Parameters.AddWithValue("@enabled", enabled ? 1 : 0);
                    cmd.Parameters.AddWithValue("@cpu_score", cpuScore);
                    cmd.Parameters.AddWithValue("@ram_score", ramScore);
                    cmd.Parameters.AddWithValue("@note", note);
                    cmd.ExecuteNonQuery();                }
                MessageBox.Show("Node added successfully!", "Success");
                NodeAdded?.Invoke(this, EventArgs.Empty); // Raise event for node added
                this.Close();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ipTextBox.Clear();
            usernameTextBox.Clear();
            passwordTextBox.Clear();
            portTextBox.Clear();
            noteTextBox.Clear();
            cpuScoreComboBox.SelectedIndex = -1;
            RAMScoreComboBox.SelectedIndex = -1;
            enabledCheckBox.Checked = false;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        public event EventHandler NodeAdded;

    } 

}


