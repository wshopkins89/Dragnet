using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Security.Policy;

namespace DragnetControl
{
    public partial class Ratatoskr_Control : Form
    {

        string DragnetDBConnect;

        public Ratatoskr_Control()
        {
            InitializeComponent();
            build_connections();
            LoadSchemaInformation();
        }
        private void build_connections()
        {
            DragnetDBConnect = $"server={GlobalVariables.DragnetDBIP};uid={GlobalVariables.DragnetDBUser};pwd={GlobalVariables.DragnetDBPassword};database={GlobalVariables.DragnetDBName}";
        }
        private void LoadSchemaInformation()
        {
            using (MySqlConnection conn3 = new MySqlConnection(DragnetDBConnect))
            {
                try
                {
                    // Retrieve all schema names
                    MySqlCommand command = new MySqlCommand($"SELECT schema_name FROM information_schema.schemata ORDER BY schema_name ASC", conn3);
                    DataTable dataTable = new DataTable();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    // Assuming your ListBox is named listBox1 (please replace with the actual name if different)
                    AssetListBox.Items.Clear(); // Clear existing items

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string schemaName = row["schema_name"].ToString();

                        // Add schema name to the ListBox
                        SchemaComboBox.Items.Add(schemaName);
                        SchemaComboBox2.Items.Add(schemaName);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (added this for clarity)
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private string GetSelectedSchema()
        {
            // Assuming you have a ListBox named listBoxSchemas for schemas (please replace if different)
            if (SchemaComboBox.SelectedItem != null)
            {
                return SchemaComboBox.SelectedItem.ToString();
            }
            return null;
        }
        private string GetSelectedTable()
        {
            // Assuming you have a ListBox named listBoxTables for table names (please replace if different)
            if (AssetListBox.SelectedItem != null)
            {
                return AssetListBox.SelectedItem.ToString();
            }
            return null;
        }

        private void SchemaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSchema = GetSelectedSchema(); // Retrieve the selected schema name

            if (string.IsNullOrEmpty(selectedSchema))
            {
                MessageBox.Show("Please select a schema first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection conn3 = new MySqlConnection(DragnetDBConnect))
            {
                try
                {
                    // Query to get all table names for the selected schema
                    MySqlCommand command = new MySqlCommand($"SELECT table_name FROM information_schema.tables WHERE table_schema = '{selectedSchema}' ORDER BY table_name ASC", conn3);
                    DataTable dataTable = new DataTable();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);


                    AssetListBox.Items.Clear(); // Clear existing items

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string tableName = row["table_name"].ToString();
                        AssetListBox.Items.Add(tableName); // Add table name to the ListBox
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void AssetListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = GetSelectedTable(); // Retrieve the selected table name

            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Please select a table first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection conn3 = new MySqlConnection(DragnetDBConnect))
            {
                try
                {
                    // Query to get all rows from the selected table
                    MySqlCommand command = new MySqlCommand($"SELECT timestamp, source, title, url, positive_score, negative_score, asset_relevance_score, temporal_relevance_score FROM `.`{selectedTable}`", conn3);
                    DataTable dataTable = new DataTable();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    ArticlesDataGridView.DataSource = dataTable; // Set DataGridView's data source to the DataTable
                    ArticlesDataGridView.Columns["timestamp"].HeaderText = "Published Time";
                    ArticlesDataGridView.Columns["source"].HeaderText = "Publication";
                    ArticlesDataGridView.Columns["title"].HeaderText = "Title";
                    ArticlesDataGridView.Columns["url"].Visible = false;
                    ArticlesDataGridView.Columns["positive_score"].HeaderText = "P Score";
                    ArticlesDataGridView.Columns["negative_score"].HeaderText = "N Score";
                    ArticlesDataGridView.Columns["asset_relevance_score"].HeaderText = "R Score";
                    ArticlesDataGridView.Columns["temporal_relevance_score"].HeaderText = "T Score";

                    ArticlesDataGridView.RowHeadersVisible = false;
                    ArticlesDataGridView.Columns["timestamp"].Width = 140;
                    ArticlesDataGridView.Columns["source"].Width = 166;
                    ArticlesDataGridView.Columns["title"].Width = 485;
                    //ArticlesDataGridView.Columns["url"].Width = 150;
                    ArticlesDataGridView.Columns["positive_score"].Width = 50;
                    ArticlesDataGridView.Columns["negative_score"].Width = 50;
                    ArticlesDataGridView.Columns["asset_relevance_score"].Width = 50;
                    ArticlesDataGridView.Columns["temporal_relevance_score"].Width = 50;
                    ArticlesDataGridView.Refresh();
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ArticlesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < ArticlesDataGridView.Rows.Count)
            {
                string url = ArticlesDataGridView.Rows[e.RowIndex].Cells["url"].Value.ToString();
                string title = ArticlesDataGridView.Rows[e.RowIndex].Cells["title"].Value.ToString();
                GlobalVariables.TargetURL = url;
                GlobalVariables.ActiveTitle = title;
                URLTextBox.Text = url;
            }

        }

        private string ExecuteBySentanceScript(string url)
        {
            string pythonScriptPath = @"C:\Users\wshop\Desktop\Mimir Control System\MimirControl\Control\Control\Scanners\PreprocessURL.exe";
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonScriptPath, // or python3 on some systems
                Arguments = $"\"{url}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
        private string ExecuteByArticleScript(string url)
        {
            string pythonScriptPath = @"C:\Users\wshop\Desktop\Mimir Control System\MimirControl\Control\Control\Scanners\PreprocessURL.exe";
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = pythonScriptPath, // or python3 on some systems
                Arguments = $"\"{url}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
        private async void retrieveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(URLTextBox.Text))
                {
                    MessageBox.Show("Please select a valid URL");
                    return;
                }
                // 1. Clear out the "mimir_scoring" table in "temp" schema
                using (MySqlConnection conn = new MySqlConnection($"server={GlobalVariables.DragnetDBIP};uid={GlobalVariables.DragnetDBUser};pwd={GlobalVariables.DragnetDBPassword};database=temp"))
                {
                    conn.Open();

                    // Clear the table
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM temp.mimir_scoring", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string url = URLTextBox.Text;

                    // 2. Add the article's title to the first row of the table
                    string title = GlobalVariables.ActiveTitle; // Assuming you have a method to get the title. If it's part of the JSON response, adjust accordingly.
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO temp.mimir_scoring (Data) VALUES (@Title)", conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.ExecuteNonQuery();
                    }
                    if (BySentenceRadioButton.Checked == true)
                    {
                        ExecuteBySentanceScript(url);
                        MySqlCommand command = new MySqlCommand($"SELECT Data, positive_score, negative_score, relevance_score, temporal_relevance_future_score, temporal_relevance_past_score, noise_filter FROM temp.Mimir_Scoring", conn);
                        {
                            DataTable dataTable = new DataTable();
                            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                            dataAdapter.Fill(dataTable);

                            TrainingDataGridView.DataSource = dataTable; // Set DataGridView's data source to the DataTable
                            TrainingDataGridView.Columns["Data"].HeaderText = "Data";
                            TrainingDataGridView.Columns["positive_score"].HeaderText = "P Score";
                            TrainingDataGridView.Columns["negative_score"].HeaderText = "N Score";
                            TrainingDataGridView.Columns["relevance_score"].HeaderText = "R Score";
                            TrainingDataGridView.Columns["temporal_relevance_future_score"].HeaderText = "TF Score";
                            TrainingDataGridView.Columns["temporal_relevance_past_score"].HeaderText = "TP Score";
                            TrainingDataGridView.Columns["noise_filter"].HeaderText = "Noise";


                            TrainingDataGridView.RowHeadersVisible = false;
                            TrainingDataGridView.Columns["Data"].Width = 835;
                            TrainingDataGridView.Columns["positive_score"].Width = 50;
                            TrainingDataGridView.Columns["negative_score"].Width = 50;
                            TrainingDataGridView.Columns["relevance_score"].Width = 50;
                            TrainingDataGridView.Columns["temporal_relevance_future_score"].Width = 50;
                            TrainingDataGridView.Columns["temporal_relevance_past_score"].Width = 50;
                            TrainingDataGridView.Columns["noise_filter"].Width = 50;
                            TrainingDataGridView.Refresh();
                        }
                    }
                    if (ByArticleRadioButton.Checked == true)
                    {
                        ExecuteByArticleScript(url);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void addSchemaButton_Click(object sender, EventArgs e)
        {
            {
                string newSchemaName = ShowSchemaInputDialog();
                if (!string.IsNullOrWhiteSpace(newSchemaName))
                {
                    try
                    {
                        CreateNewSchema(newSchemaName);
                        SchemaComboBox2.Items.Add(newSchemaName);
                        SchemaComboBox2.SelectedItem = newSchemaName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private string ShowSchemaInputDialog()
        {
            using (var inputBox = new InputBox())
            {
                if (inputBox.ShowDialog(this) == DialogResult.OK)
                {
                    return inputBox.InputText;
                }
            }
            return null;
        }
        private void CreateNewSchema(string schemaName)
        {
            string connectionString = $"server={GlobalVariables.DragnetDBIP};user id={GlobalVariables.DragnetDBUser};password={GlobalVariables.DragnetDBPassword};";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"CREATE SCHEMA {schemaName};";
                    command.ExecuteNonQuery();
                }
            }
        }
        private void FillArticleData()
        {
            using (MySqlConnection conn = new MySqlConnection($"server={GlobalVariables.DragnetDBIP};uid={GlobalVariables.DragnetDBUser};pwd={GlobalVariables.DragnetDBPassword};database=temp"))
            {
                MySqlCommand command = new MySqlCommand($"SELECT Data, positive_score, negative_score, relevance_score, temporal_relevance_future_score, temporal_relevance_past_score, noise_filter FROM temp.Mimir_Scoring", conn);
                DataTable dataTable = new DataTable();

                using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command))
                {
                    dataAdapter.Fill(dataTable);
                }

                TrainingDataGridView.DataSource = dataTable;
                TrainingDataGridView.Columns["Data"].HeaderText = "Data";
                TrainingDataGridView.Columns["positive_score"].HeaderText = "P Score";
                TrainingDataGridView.Columns["negative_score"].HeaderText = "N Score";
                TrainingDataGridView.Columns["relevance_score"].HeaderText = "R Score";
                TrainingDataGridView.Columns["temporal_relevance_future_score"].HeaderText = "TF Score";
                TrainingDataGridView.Columns["temporal_relevance_past_score"].HeaderText = "TP Score";
                TrainingDataGridView.Columns["noise_filter"].HeaderText = "Noise";

                TrainingDataGridView.RowHeadersVisible = false;
                TrainingDataGridView.Columns["Data"].Width = 835;
                TrainingDataGridView.Columns["positive_score"].Width = 50;
                TrainingDataGridView.Columns["negative_score"].Width = 50;
                TrainingDataGridView.Columns["relevance_score"].Width = 50;
                TrainingDataGridView.Columns["temporal_relevance_future_score"].Width = 50;
                TrainingDataGridView.Columns["temporal_relevance_past_score"].Width = 50;
                TrainingDataGridView.Columns["noise_filter"].Width = 50;

                TrainingDataGridView.Refresh();
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            string AutoScoreScriptPath = @"C:\Users\wshop\Desktop\Vanir System Control\Control\Control\Control\Scanners\TrainerAutoScore.exe";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = AutoScoreScriptPath,
                UseShellExecute = false,
                //RedirectStandardOutput = true,
                //CreateNoWindow = true
            };

            // Start the process
            using (Process process = Process.Start(start))
            {
                // Wait for the process to complete
                process.WaitForExit();

                // Once the script is done, reload the table
                FillArticleData();
            }
        }
    }
}

