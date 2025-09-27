using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace DragnetControl
{
    public partial class Gullveig : Form
    {
        public Gullveig()
        {
            InitializeComponent();
            LoadBalanceOfPowerImages(pictureBoxHouse, pictureBoxSenate);
            SearchPoliticians(GlobalVariables.AssetDBConnect, searchTextBox, politiciansListBox, representativesCheckBox, senatorsCheckBox, republicansCheckBox, democratCheckBox, otherCheckBox);
            
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            SearchPoliticians(GlobalVariables.AssetDBConnect, searchTextBox, politiciansListBox, representativesCheckBox, senatorsCheckBox, republicansCheckBox, democratCheckBox, otherCheckBox);
        }
        private void SearchPoliticians(string connectionString, TextBox searchTextBox, ListBox listBox, CheckBox representativesCheckBox, CheckBox senatorsCheckBox, CheckBox republicanCheckBox, CheckBox democratCheckBox, CheckBox otherCheckBox)
        {
            // Clear the listbox
            listBox.Items.Clear();

            // Get the search text
            string searchText = searchTextBox.Text.Trim();

            // Determine the query based on the checkbox selections
            string query = "SELECT name FROM politicians WHERE 1=1";

            // Add conditions for the chambers checkboxes
            if (representativesCheckBox.Checked && !senatorsCheckBox.Checked)
            {
                query += " AND chamber = 'House'";
            }
            else if (!representativesCheckBox.Checked && senatorsCheckBox.Checked)
            {
                query += " AND chamber = 'Senate'";
            }
            else if (representativesCheckBox.Checked && senatorsCheckBox.Checked)
            {
                query += " AND (chamber = 'House' OR chamber = 'Senate')";
            }
            else
            {
                // If neither chambers checkbox is checked, no results should be loaded
                return;
            }

            // Add conditions for the party checkboxes
            if (republicanCheckBox.Checked && !democratCheckBox.Checked && !otherCheckBox.Checked)
            {
                query += " AND party = 'Republican'";
            }
            else if (!republicanCheckBox.Checked && democratCheckBox.Checked && !otherCheckBox.Checked)
            {
                query += " AND party = 'Democrat'";
            }
            else if (!republicanCheckBox.Checked && !democratCheckBox.Checked && otherCheckBox.Checked)
            {
                query += " AND party = 'Other'";
            }
            else if (republicanCheckBox.Checked || democratCheckBox.Checked || otherCheckBox.Checked)
            {
                query += " AND (";
                bool isFirst = true;

                if (republicanCheckBox.Checked)
                {
                    query += "party = 'Republican'";
                    isFirst = false;
                }

                if (democratCheckBox.Checked)
                {
                    if (!isFirst) query += " OR ";
                    query += "party = 'Democrat'";
                    isFirst = false;
                }

                if (otherCheckBox.Checked)
                {
                    if (!isFirst) query += " OR ";
                    query += "party = 'Other'";
                }

                query += ")";
            }

            // Add condition for search text if it is not empty
            if (!string.IsNullOrEmpty(searchText))
            {
                query += " AND name LIKE @SearchText";
            }

            try
            {
                // Create a connection to the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Create a command
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            // Add the parameter for the search text
                            command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                        }

                        // Open the connection
                        connection.Open();

                        // Execute the command and read the data
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Add each name to the listbox
                                listBox.Items.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle MySQL errors
                MessageBox.Show("A MySQL error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other errors
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        public void LoadBalanceOfPowerImages(PictureBox pictureBoxHouse, PictureBox pictureBoxSenate)
        {
            try
            {
                // Directory containing images
                string imagesDirectory = GlobalVariables.CongressImagesFilepath;

                // Define the specific image file paths
                string houseImagePath = Path.Combine(imagesDirectory, "BalanceOfPowerHouse.png");
                string senateImagePath = Path.Combine(imagesDirectory, "BalanceOfPowerSenate.png");

                // Check if the images exist and load them into the PictureBoxes
                if (File.Exists(houseImagePath) && File.Exists(senateImagePath))
                {
                    pictureBoxHouse.Image = Image.FromFile(houseImagePath);
                    pictureBoxSenate.Image = Image.FromFile(senateImagePath);
                }
                else
                {
                    MessageBox.Show("One or both of the balance of power images are missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The specified directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Image files not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void politiciansListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (politiciansListBox.SelectedIndex != -1)
            {
                string selectedPolitician = politiciansListBox.SelectedItem.ToString();
                using (MySqlConnection connection = new MySqlConnection(GlobalVariables.AssetDBConnect))
                {
                    string query = "SELECT * FROM politicians WHERE Name = @Name";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", selectedPolitician);

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            politicianNameLabel.Text = reader["Name"].ToString();
                            partyLabel.Text = "Party: " + reader["Party"].ToString();
                            chamberLabel.Text = "Chamber: " + reader["Chamber"].ToString();
                            stateLabel.Text = "State: " + reader["State"].ToString();
                            totalTradesLabel.Text = "Total Trades: " + reader["Trades"].ToString();
                            volumeLabel.Text = "Traded Volume: " + reader["Volume"].ToString();
                            lastTradeLabel.Text = "Date of Last Trade: " + reader.GetDateTime("LastTrade").ToString("MM/dd/yyyy");

                            string imagePath = Path.Combine(GlobalVariables.CongressImagesFilepath, reader["Image"].ToString());
                            if (File.Exists(imagePath))
                            {
                                politicianPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                politicianPictureBox.Image = Image.FromFile(imagePath);
                            }
                            else
                            {
                                politicianPictureBox.Image = null;
                                MessageBox.Show("Image file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            // Load the politician's trade table into the DataGridView
                            LoadPoliticianTradeTable(selectedPolitician);
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadPoliticianTradeTable(string politicianName)
        {
            // Format the table name to match the schema (e.g., "angus_king")
            string tableName = politicianName.ToLower().Replace(' ', '_');
            string query = $"SELECT issuer,publisheddate, tradeddate, daystofile, owner, type, size, price FROM Politicians.`{tableName}`";

            using (MySqlConnection connection = new MySqlConnection(GlobalVariables.AssetDBConnect))
            {
                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    politicianTradeDataGridView.DataSource = dataTable;
                    politicianTradeDataGridView.RowHeadersVisible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the trade table: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void refreshGovernmentButton_Click(object sender, EventArgs e)
        {
            RefreshImages(pictureBoxHouse, pictureBoxSenate);
        }
        private void RunRebuildGovernmentScript()
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "RebuildGovernment.exe",
                WorkingDirectory = GlobalVariables.CuratorPath,
                UseShellExecute = true, // Use ShellExecute to avoid issues
            };
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
            }
        
        }

        public void RefreshImages(PictureBox pictureBoxHouse, PictureBox pictureBoxSenate)
        {
            try
            {
                // Directory containing images
                string imagesDirectory = GlobalVariables.CongressImagesFilepath;

                // Define the specific image file paths
                string houseImagePath = Path.Combine(imagesDirectory, "BalanceOfPowerHouse.png");
                string senateImagePath = Path.Combine(imagesDirectory, "BalanceOfPowerSenate.png");

                // Unload and dispose of the images to release file locks
                if (pictureBoxHouse.Image != null)
                {
                    pictureBoxHouse.Image.Dispose();
                    pictureBoxHouse.Image = null;
                }

                if (pictureBoxSenate.Image != null)
                {
                    pictureBoxSenate.Image.Dispose();
                    pictureBoxSenate.Image = null;
                }

                // Run the Python script

                RunRebuildGovernmentScript();

                // Wait for a short time to ensure the script has finished and files are updated
                System.Threading.Thread.Sleep(2000); // Adjust the time if necessary

                // Check if the images exist and load them into the PictureBoxes
                if (File.Exists(houseImagePath) && File.Exists(senateImagePath))
                {
                    pictureBoxHouse.Image = Image.FromFile(houseImagePath);
                    pictureBoxHouse.SizeMode = PictureBoxSizeMode.StretchImage;

                    pictureBoxSenate.Image = Image.FromFile(senateImagePath);
                    pictureBoxSenate.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    MessageBox.Show("One or both of the balance of power images are missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The specified directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Image files not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buildCongressionalDatabaseButton_Click(object sender, EventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "ConstructCongressionalDatabase.exe",
                WorkingDirectory = GlobalVariables.CuratorPath,
                UseShellExecute = true,
            };

            using (Process process = new Process { StartInfo = start })
            {
                process.Start();
            }
        }

        private void checkRecentActivityButton_Click(object sender, EventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "CapitolTradesScraper.exe",
                WorkingDirectory = GlobalVariables.CuratorPath,
                UseShellExecute = true,
            };

            using (Process process = new Process { StartInfo = start })
            {
                process.Start();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
