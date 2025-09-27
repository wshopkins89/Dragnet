using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DragnetControl
{
    public partial class Mimir_Control : Form
    {

        string DragnetDBConnect;

        public Mimir_Control()
        {
            InitializeComponent();
            build_connections();
            LoadSchemaInformation();
            HideSchedulerPanels();
            slidingWindowPanel.Visible = false;
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


        private async void retrieveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(URLTextBox.Text))
                {
                    MessageBox.Show("Please select a valid URL");
                    return;
                }

                string url = URLTextBox.Text;
                string outputDir = Path.GetDirectoryName(newModelPathTextBox.Text);
                string title = GlobalVariables.ActiveTitle;
                string sanitizedTitle = SanitizeFilename(title);

                string csvFilePath = Path.Combine(outputDir, $"{sanitizedTitle}.csv");

                if (BySentenceRadioButton.Checked == true)
                {
                    await ExecuteBySentenceScriptAsync(url);

                    if (!File.Exists(csvFilePath))
                    {
                        MessageBox.Show($"CSV file not found: {csvFilePath}");
                        return;
                    }

                    DataTable dataTable = new DataTable();
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null,
                        HeaderValidated = null,
                        BadDataFound = context =>
                        {
                            MessageBox.Show($"Bad data found: {context.RawRecord}");
                        }
                    };

                    using (var reader = new StreamReader(csvFilePath))
                    using (var csv = new CsvReader(reader, config))
                    {
                        using (var dr = new CsvDataReader(csv))
                        {
                            dataTable.Load(dr);
                        }
                    }

                    if (dataTable.Columns.Contains("sentence"))
                    {
                        dataTable.Columns["sentence"].SetOrdinal(0);
                    }

                    DataTable editableDataTable = dataTable.Clone();
                    foreach (DataColumn col in editableDataTable.Columns)
                    {
                        col.ReadOnly = false;
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        editableDataTable.ImportRow(row);
                    }

                    TrainingDataGridView.DataSource = editableDataTable;
                    TrainingDataGridView.RowHeadersVisible = false;
                    TrainingDataGridView.AllowUserToAddRows = false;
                    TrainingDataGridView.AllowUserToDeleteRows = false;
                    TrainingDataGridView.ReadOnly = false;

                    foreach (DataGridViewColumn column in TrainingDataGridView.Columns)
                    {
                        column.ReadOnly = false;
                        column.HeaderText = column.Name;
                        column.Width = column.Name == "sentence" ? 1085 : 50;
                    }

                    // Set AutoSize mode for rows and wrap mode for the first column
                    TrainingDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    TrainingDataGridView.Columns["sentence"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    TrainingDataGridView.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async Task ExecuteBySentenceScriptAsync(string url)
        {
            string fullOutputPath = newModelPathTextBox.Text; // Full path including the filename
            string outputDir = Path.GetDirectoryName(fullOutputPath);
            int outputs = Convert.ToInt32(numberOfOutputsComboBox.Text);
            string filename = GlobalVariables.ActiveTitle;

            // Gather output names from dynamically generated text boxes
            var outputNames = new List<string>();
            var outputDefaultScores = new List<string>();
            for (int i = 0; i < outputs; i++)
            {
                var namesTextBox = this.Controls.Find($"textBoxName{i}", true).FirstOrDefault() as TextBox;
                if (namesTextBox != null)
                {
                    outputNames.Add(namesTextBox.Text);
                }
                var defaultsTextBox = this.Controls.Find($"textBoxDefaultScore{i}", true).FirstOrDefault() as TextBox;
                if (defaultsTextBox != null)
                {
                    outputDefaultScores.Add(defaultsTextBox.Text);
                }
            }

            string outputNamesArgs = string.Join(" ", outputNames.Select(name => $"\"{name}\""));
            string outputDefaults = string.Join(" ", outputDefaultScores.Select(defaultScore => $"\"{defaultScore}\""));
            string arguments = $"\"{url}\" \"{outputDir}\" \"{filename}\" {outputs} {outputNamesArgs} {outputDefaults}";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "RetrieveBySentence.exe",
                WorkingDirectory = GlobalVariables.CuratorPath,
                Arguments = arguments,
                UseShellExecute = true,
            };

            using (Process process = new Process { StartInfo = start })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        private string SanitizeFilename(string title, int maxLength = 50)
        {
            // Remove non-alphanumeric characters (except for underscores and hyphens)
            string sanitized = Regex.Replace(title, @"[^a-zA-Z0-9-_]", "");
            // Truncate the filename to a maximum length
            return sanitized.Length <= maxLength ? sanitized : sanitized.Substring(0, maxLength);
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

        private void addSchemaButton_Click(object sender, EventArgs e)
        {

        }

        private void selectDirectoryButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\"; // Adjust as needed
                openFileDialog.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Read the contents of the file into a string
                    var filePath = openFileDialog.FileName;
                    var jsonConfig = File.ReadAllText(filePath);

                    // Deserialize the JSON string into a dictionary
                    var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                    string BERTPath = filePath.Replace(".json", ".pb");
                    newModelPathTextBox.Text = BERTPath; // Display the selected H5 file path in the textbox
                    FileInfo fileInfo = new FileInfo(BERTPath);
                    DirectoryInfo directoryInfo = fileInfo.Directory;
                    string directoryPath = directoryInfo.FullName;
                    newModelPathTextBox.Enabled = false; // Optional: Disable the textbox if you don't want it edited after selection
                    LoadConfigurationIntoUI(config);
                    // Call the method to update the UI with the loaded configuration
                }
            }
        }

        private void saveConfigurationButton_Click(object sender, EventArgs e)
        {
            var configDict = ConstructConfigurationDictionary(); // Get the configuration dictionary

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Add the filename and path to the dictionary
                    configDict["model_name"] = Path.GetFileNameWithoutExtension(filePath);
                    configDict["Path"] = filePath;

                    // Serialize the dictionary to a JSON string
                    string configurationJson = JsonConvert.SerializeObject(configDict, Formatting.Indented);

                    // Write the JSON string to the file
                    File.WriteAllText(filePath, configurationJson);
                    newModelPathTextBox.Text = filePath;
                    configDict.Clear();
                    //reloadConfiguration(filePath);
                }
            }
        }
        private Dictionary<string, object> ConstructConfigurationDictionary()
        {
            var config = new Dictionary<string, object>();
            var layersConfig = new List<Dictionary<string, object>>();

            config["optimizer"] = optimizerComboBox.SelectedItem.ToString();
            config["model_type"] = modelTypeComboBox.SelectedItem.ToString();
            config["model_library"] = modelLibraryComboBox.SelectedItem.ToString();
            config["tokenizer"] = versionComboBox.SelectedItem.ToString();
            config["task_type"] = taskTypeComboBox.SelectedItem.ToString();
            config["base_model"] = baseModelComboBox.SelectedItem.ToString();
            config["max_sequence_length"] = int.Parse(maxSequenceTextBox.Text);
            config["batch_size"] = int.Parse(batchSizeTextBox.Text);
            config["training_epochs"] = trainingEpochsTextBox.Text;
            config["warmup_steps"] = warmupStepsTextBox.Text;
            config["convert_to_lowercase"] = convertToLowerCaseCheckBox.Checked;
            config["remove_punctuation"] = removePunctuationCheckBox.Checked;
            config["remove_stopwords"] = removeStopwordCheckBox.Checked;
            config["freeze_bert_layers"] = freezeBertLayersCheckBox.Checked;
            var trainableLayers = trainableLayersSelectedListBox.Items.Cast<string>().ToList();
            config["trainable_layers"] = trainableLayers;
            var evaluationMetrics = evaluationMetricsSelectedListBox.Items.Cast<string>().ToList();
            config["evaluation_metrics"] = evaluationMetrics;

            var outputsConfig = new List<Dictionary<string, object>>();
            foreach (System.Windows.Forms.Control control in panelLayers.Controls)
            {
                if (control is GroupBox groupBox && groupBox.Tag != null && groupBox.Tag.ToString().StartsWith("Output"))
                {
                    var outputConfig = new Dictionary<string, object>();
                    foreach (System.Windows.Forms.Control groupControl in groupBox.Controls)
                    {
                        if (groupControl is TextBox textBox)
                        {
                            if (textBox.Name.Contains("textBoxName"))
                            {
                                outputConfig["name"] = textBox.Text;
                            }
                            else if (textBox.Name.Contains("textBoxNeurons"))
                            {
                                outputConfig["neurons"] = textBox.Text;
                            }
                            else if (textBox.Name.Contains("textBoxDefaultScore"))
                            {
                                outputConfig["default_score"] = textBox.Text;
                            }
                        }
                        else if (groupControl is NumericUpDown numericUpDown)
                        {
                            if (numericUpDown.Name.Contains("numericUpDownWeight"))
                            {
                                outputConfig["weight"] = (decimal)numericUpDown.Value;
                            }
                        }
                        else if (groupControl is ComboBox comboBox)
                        {
                            if (comboBox.Name.Contains("comboBoxOutputType"))
                            {
                                outputConfig["type"] = comboBox.SelectedItem.ToString();
                            }
                            if (comboBox.Name.Contains("comboBoxActivation"))
                            {
                                outputConfig["activation"] = comboBox.SelectedItem.ToString();
                            }
                            if (comboBox.Name.Contains("comboBoxLoss"))
                            {
                                outputConfig["loss"] = comboBox.SelectedItem.ToString();
                            }
                        }
                        else if (groupControl is CheckBox checkBox)
                        {
                            if (checkBox.Name.Contains("biasCheckBox"))
                            {
                                outputConfig["use_bias"] = checkBox.Checked;
                            }
                        }
                    }
                    outputsConfig.Add(outputConfig);
                }
            }
            config["outputs"] = outputsConfig;
            config["custom_tokenizer"] = customRadioButton.Checked;
            config["custom_tokenizer_path"] = customTokenizerPathTextBox.Text;

            var schedulerType = schedulerTypeComboBox.SelectedItem.ToString();
            config["scheduler_type"] = schedulerType;
            if (schedulerType == "Constant")
            {
                config["constant_learning_rate"] = constantLearningRateTextBox.Text;
            }
            else if (schedulerType == "Step" || schedulerType == "Exponential" || schedulerType == "Inverse Time")
            {
                config["initial_learning_rate"] = initialLearningRateTextBox.Text;
                config["decay_steps"] = decayStepsTextBox.Text;
                config["decay_rate"] = decayRateTextBox.Text;
            }
            else if (schedulerType == "Polynomial")
            {
                config["initial_learning_rate"] = polyInitialLRTextBox.Text;
                config["decay_steps"] = polyDecayStepsTextBox.Text;
                config["power"] = polyPowerTextBox.Text;
                config["end_learning_rate"] = polyEndLearningRateTextBox.Text;
            }
            else if (schedulerType == "Cosine")
            {
                config["initial_learning_rate"] = cosineInitialLRTextBox.Text;
                config["decay_steps"] = cosineDecayStepsTextBox.Text;
                config["alpha"] = alphaTextBox.Text;
            }
            else if (schedulerType == "ReduceLROnPlateau")
            {
                config["monitor"] = monitorLRComboBox.SelectedItem.ToString();
                config["factor"] = LRfactorTextBox.Text;
                config["patience"] = patienceTextBox.Text;
                config["minimum_learning_rate"] = LRminLearningRateTextBox.Text;
            }
            else if (schedulerType == "Cyclic Learning Rate")
            {
                config["base_learning_rate"] = cyclicBaseLRTextBox.Text;
                config["step_size_up"] = cyclicStepUpTextBox.Text;
                config["step_size_down"] = cyclicTextBoxDown.Text;
                config["mode"] = cyclicModeComboBox.SelectedItem.ToString();
                config["max_learning_rate"] = cyclicMaxLearningRateTextBox.Text;
            }
            else if (schedulerType == "OneCycleLR")
            {
                config["max_learning_rate"] = oneCycleMaxLearningRateTextBox.Text;
                config["total_steps"] = oneCycleTotalStepsTextBox.Text;
                config["epochs"] = oneCycleEpochsTextBox.Text;
                config["steps_per_epoch"] = oneCycleStepsPerEpochTextBox.Text;
            }
            return config;
        }


        private void addTrainableLayersButton_Click(object sender, EventArgs e)
        {
            var selectedItems = trainableLayersPoolListBox.SelectedItems.OfType<string>().ToList();
            foreach (var item in selectedItems)
            {
                if (!item.StartsWith("(Selected)"))
                {
                    trainableLayersSelectedListBox.Items.Add(item);

                    int itemIndex = trainableLayersPoolListBox.Items.IndexOf(item);

                    trainableLayersPoolListBox.Items[itemIndex] = "(Selected) " + item;

                }
            }
        }

        private void removeTrainableLayerButton_Click(object sender, EventArgs e)
        {
            var selectedItems = trainableLayersSelectedListBox.SelectedItems.OfType<string>().ToList();
            foreach (var item in selectedItems)
            {
                trainableLayersSelectedListBox.Items.Remove(item);

                string originalItem = item.StartsWith("(Selected) ") ? item.Substring(11) : item;
                int itemIndex = trainableLayersPoolListBox.Items.IndexOf("(Selected) " + originalItem);
                if (itemIndex != -1)
                {
                    trainableLayersPoolListBox.Items[itemIndex] = originalItem;
                }
                else
                {
                    trainableLayersPoolListBox.Items.Add(originalItem);
                }
            }
        }

        private void addEvaluationMetricButton_Click(object sender, EventArgs e)
        {
            var selectedItems = evaluationMetricsPoolListBox.SelectedItems.OfType<string>().ToList();
            foreach (var item in selectedItems)
            {
                if (!item.StartsWith("(Selected)"))
                {
                    evaluationMetricsSelectedListBox.Items.Add(item);

                    int itemIndex = evaluationMetricsPoolListBox.Items.IndexOf(item);

                    evaluationMetricsPoolListBox.Items[itemIndex] = "(Selected) " + item;

                }
            }
        }

        private void removeEvaluationMetricButton_Click(object sender, EventArgs e)
        {
            var selectedItems = evaluationMetricsSelectedListBox.SelectedItems.OfType<string>().ToList();
            foreach (var item in selectedItems)
            {
                evaluationMetricsSelectedListBox.Items.Remove(item);

                string originalItem = item.StartsWith("(Selected) ") ? item.Substring(11) : item;
                int itemIndex = evaluationMetricsPoolListBox.Items.IndexOf("(Selected) " + originalItem);
                if (itemIndex != -1)
                {
                    evaluationMetricsPoolListBox.Items[itemIndex] = originalItem;
                }
                else
                {
                    evaluationMetricsPoolListBox.Items.Add(originalItem);
                }
            }
        }

        private void PrebuiltRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (PrebuiltRadioButton.Checked == true)
            {
                versionComboBox.Enabled = true;
                customTokenizerPathTextBox.Clear();
                customTokenizerPathTextBox.Enabled = false;
                selectTokenizerButton.Enabled = false;
            }
        }

        private void customRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked == true)
            {
                versionComboBox.Text = "";
                versionComboBox.Enabled = false;
                customTokenizerPathTextBox.Enabled = true;
                selectTokenizerButton.Enabled = true;
            }
        }
        private void numberOfOutputsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelLayers.Controls.Clear();
            int numLayers = int.Parse(numberOfOutputsComboBox.SelectedItem.ToString());
            int startY = 10; // Starting Y position
            int spacingY = 150; // Vertical spacing between GroupBoxes

            for (int i = 0; i < numLayers; i++)
            {
                GroupBox groupBoxLayer = CreateLayerGroupBox(i, startY, spacingY);
                ComboBox comboBoxOutputType = (ComboBox)groupBoxLayer.Controls["comboBoxOutputType" + i];
                panelLayers.Controls.Add(groupBoxLayer);
                comboBoxOutputType.SelectedIndexChanged += comboBoxLayerType_SelectedIndexChanged;
                panelLayers.AutoScroll = true;
                panelLayers.AutoScrollMinSize = new Size(0, startY + spacingY);
            }
        }
        private void comboBoxLayerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBoxLayerType = sender as ComboBox;
            if (comboBoxLayerType == null) return;

            string groupName = comboBoxLayerType.Parent.Name;
            int layerIndex = ExtractLayerIndex(groupName);

            SetupLayerControls((GroupBox)comboBoxLayerType.Parent, layerIndex);
        }
        private int ExtractLayerIndex(string groupName)
        {
            string indexPart = groupName.Replace("groupBoxLayer", "");
            if (int.TryParse(indexPart, out int index))
            {
                return index;
            }
            return -1;
        }
        private void SetupLayerControls(GroupBox groupBoxLayer, int layerIndex)
        {
            ComboBox comboBoxLayerType = groupBoxLayer.Controls.OfType<ComboBox>().FirstOrDefault(cb => cb.Name == $"comboBoxLayerType{layerIndex}");
            if (comboBoxLayerType != null)
            {
                if (comboBoxLayerType.SelectedItem?.ToString() == "LSTM")
                {
                    AddOutputControls(groupBoxLayer, layerIndex);
                }
                else
                {
                    RemoveOutputControls(groupBoxLayer, layerIndex);
                }
            }
        }
        private void AddOutputControls(GroupBox groupBox, int index)
        {
            // Checkbox for Bidirectional
            TextBox outputNameTextBox = new TextBox
            {
                Text = "",
                Location = new Point(10, 50), // Adjust position as needed
                Size = new Size(10, 50),
                Name = $"outputNameTextBox{index}"
            };
            groupBox.Controls.Add(outputNameTextBox);

            // Checkbox for Return Sequences
            CheckBox checkBoxReturnSequences = new CheckBox
            {
                Text = "Return Sequences",
                Location = new Point(10, 75), // Adjust position as needed
                AutoSize = true,
                Name = $"checkBoxReturnSequences{index}"
            };
            groupBox.Controls.Add(checkBoxReturnSequences);

            // Checkbox for Return State
            CheckBox checkBoxReturnState = new CheckBox
            {
                Text = "Return State",
                Location = new Point(10, 100), // Adjust position as needed
                AutoSize = true,
                Name = $"checkBoxReturnState{index}"
            };
            groupBox.Controls.Add(checkBoxReturnState);

            ComboBox comboBoxActivation = new ComboBox
            {
                Location = new Point(250, 16),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = $"comboBoxActivation{index}" // Naming the ComboBox
            };
            comboBoxActivation.Items.AddRange(new object[] { "Linear", "ReLU", "Sigmoid", "Tanh", "Softmax" });
            comboBoxActivation.SelectedIndex = 0; // Default to "ReLU"
            groupBox.Controls.Add(comboBoxActivation);

            // Update the GroupBox height to fit new controls
            groupBox.Height += 100; // Adjust the height increase as needed

            RepositionSubsequentGroupBoxes();
        }

        private void RemoveOutputControls(GroupBox groupBox, int index)
        {
            // Identifying LSTM-specific controls by their names
            var controlNamesToRemove = new List<string>
            {
            $"checkBoxBidirectional{index}",
            $"checkBoxReturnSequences{index}",
            $"checkBoxReturnState{index}",
            };

            // Remove the controls from the groupBox
            foreach (var name in controlNamesToRemove)
            {
                var controlToRemove = groupBox.Controls.OfType<System.Windows.Forms.Control>().FirstOrDefault(c => c.Name == name);
                if (controlToRemove != null)
                {
                    groupBox.Controls.Remove(controlToRemove);
                    controlToRemove.Dispose();
                }
            }
            groupBox.Height -= 100; // Adjust this value as necessary to match the original height before controls were added

            RepositionSubsequentGroupBoxes();
        }
        private void RepositionSubsequentGroupBoxes()
        {
            int currentY = 10;
            int spacingY = 20;

            foreach (System.Windows.Forms.Control control in panelLayers.Controls)
            {
                if (control is GroupBox groupBox)
                {
                    groupBox.Location = new Point(10, currentY);
                    currentY += groupBox.Height + spacingY;
                }
            }
        }
        private GroupBox CreateLayerGroupBox(int index, int startY, int spacingY)
        {
            GroupBox groupBoxLayer = new GroupBox
            {
                Text = $"Output {index + 1}",
                Location = new Point(10, startY + (index * spacingY)),
                Size = new Size(panelLayers.Width - 20, 150),
                //AutoSize = true,
                //AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Name = $"groupBoxLayer{index}",
                Tag = $"Output{index}"
            };

            Label labelOutputs = new Label
            {
                Text = "Name:",
                Location = new Point(6, 19),
                AutoSize = true,
                Name = $"labelName{index}"
            };
            groupBoxLayer.Controls.Add(labelOutputs);

            TextBox textBoxName = new TextBox
            {
                Location = new Point(55, 16),
                Width = 100,
                Name = $"textBoxName{index}"
            };
            groupBoxLayer.Controls.Add(textBoxName);

            Label labelType = new Label
            {
                Text = "Type:",
                Location = new Point(165, 19),
                AutoSize = true,
                Name = $"labelType{index}"
            };
            groupBoxLayer.Controls.Add(labelType);

            // ComboBox for Layer Type
            ComboBox comboBoxOutputType = new ComboBox
            {
                Location = new Point(210, 16),
                Width = 110,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = $"comboBoxOutputType{index}"
            };
            comboBoxOutputType.Items.AddRange(new object[] { "Classification", "Regression", "Token Classification", "Sequence Classification" });
            groupBoxLayer.Controls.Add(comboBoxOutputType);

            // CheckBox for Dropout
            Label weightLabel = new Label
            {
                Text = "Weight:",
                Location = new Point(323, 19),
                AutoSize = true,
                Name = $"weightLabel{index}"
            };
            groupBoxLayer.Controls.Add(weightLabel);

            // NumericUpDown for Dropout Rate
            NumericUpDown numericUpDownWeightRate = new NumericUpDown
            {
                Location = new Point(380, 16),
                Width = 50,
                Minimum = 0,
                Maximum = 1,
                DecimalPlaces = 2,
                Increment = 0.01M,
                Name = $"numericUpDownWeight{index}"
            };
            groupBoxLayer.Controls.Add(numericUpDownWeightRate);

            Label labelActivation = new Label
            {
                Text = "Activation:",
                Location = new Point(6, 55),
                AutoSize = true,
                Name = $"labelActivation{index}"
            };
            groupBoxLayer.Controls.Add(labelActivation);
            // ComboBox for Activation Function
            ComboBox comboBoxActivation = new ComboBox
            {
                Location = new Point(80, 50),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = $"comboBoxActivation{index}"
            };
            comboBoxActivation.Items.AddRange(new object[] { "Linear", "ReLU", "Sigmoid", "Tanh", "Softmax", "gelu" });
            groupBoxLayer.Controls.Add(comboBoxActivation);

            Label labelLoss = new Label
            {
                Text = "Loss:",
                Location = new Point(190, 55),
                AutoSize = true,
                Name = $"labelLoss{index}"
            };
            groupBoxLayer.Controls.Add(labelLoss);
            // ComboBox for Activation Function
            ComboBox comboBoxLoss = new ComboBox
            {
                Location = new Point(230, 50),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = $"comboBoxLoss{index}"
            };
            comboBoxLoss.Items.AddRange(new object[] { "categorical_crossentropy", "binary_crossentropy", "mean_squared_error", "sparse_categorical_crossentropy", "hinge" });
            groupBoxLayer.Controls.Add(comboBoxLoss);

            Label labelNeurons = new Label
            {
                Text = "Neurons:",
                Location = new Point(6, 85),
                AutoSize = true,
                Name = $"labelNeurons{index}"
            };
            groupBoxLayer.Controls.Add(labelNeurons);

            TextBox textBoxNeurons = new TextBox
            {
                Location = new Point(70, 80),
                Width = 50,
                Name = $"textBoxNeurons{index}"
            };
            groupBoxLayer.Controls.Add(textBoxNeurons);

            groupBoxLayer.Controls.Add(labelActivation);
            CheckBox biasCheckBox = new CheckBox
            {
                Text = "Use Bias",
                Location = new Point(140, 85),
                AutoSize = true,
                Name = $"biasCheckBox{index}"
            };
            groupBoxLayer.Controls.Add(biasCheckBox);
            Label defaultScoreLabel = new Label
            {
                Text = "Default Score:",
                Location = new Point(250, 85),
            };
            groupBoxLayer.Controls.Add(defaultScoreLabel);
            TextBox defaultScoreTextBox = new TextBox
            {
                Location = new Point(350, 80),
                Width = 50,
                Name = $"textBoxDefaultScore{index}",
            };
            groupBoxLayer.Controls.Add(defaultScoreTextBox);

            return groupBoxLayer;
        }
        private void HideSchedulerPanels()
        {
            constantDecayPanel.Visible = false;
            stepDecayPanel.Visible = false;
            polynomialDecayPanel.Visible = false;
            cosineDecayPanel.Visible = false;
            reduceLROnPlateauPanel.Visible = false;
            cyclicLearningRatePanel.Visible = false;
            OneCycleLRPanel.Visible = false;
        }
        private void schedulerTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideSchedulerPanels();

            string selectedText = schedulerTypeComboBox.Text.Trim();

            if (selectedText.Equals("Step", StringComparison.OrdinalIgnoreCase) ||
                selectedText.Equals("Exponential", StringComparison.OrdinalIgnoreCase) ||
                selectedText.Equals("Inverse Time", StringComparison.OrdinalIgnoreCase))
            {
                stepDecayPanel.Visible = true;
            }
            else if (selectedText.Equals("Polynomial", StringComparison.OrdinalIgnoreCase))
            {
                polynomialDecayPanel.Visible = true;
            }
            else if (selectedText.Equals("Cosine", StringComparison.OrdinalIgnoreCase))
            {
                cosineDecayPanel.Visible = true;

            }
            else if (selectedText.Equals("ReduceLROnPlateau", StringComparison.OrdinalIgnoreCase))
            {
                reduceLROnPlateauPanel.Visible = true;
            }
            else if (selectedText.Equals("Cyclic Learning Rate", StringComparison.OrdinalIgnoreCase))
            {
                cyclicLearningRatePanel.Visible = true;
            }
            else if (selectedText.Equals("OneCycleLR", StringComparison.OrdinalIgnoreCase))
            {
                OneCycleLRPanel.Visible = true;
            }
            else
            {
                constantDecayPanel.Visible = true;

            }
        }

        private void constructModelButton_Click(object sender, EventArgs e)
        {
            // Check if both text boxes are populated
            if (!string.IsNullOrEmpty(newModelPathTextBox.Text))
            {
                // Construct the full path to the JSON configuration file
                string configFilePath = newModelPathTextBox.Text;

                // Execute the Python script with the config file path as an argument
                ExecutePythonScript(configFilePath);
            }
            else
            {
                MessageBox.Show("Please save a configuration before building the model.", "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void ExecutePythonScript(string configFilePath)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "BuildBertModel.exe"; // or the path to your Python executable
            start.WorkingDirectory = GlobalVariables.CuratorPath;
            start.Arguments = $"\"{configFilePath}\""; // Adjust the path to your script
            start.UseShellExecute = true;

            Process.Start(start);
        }
        private void LoadConfigurationIntoUI(Dictionary<string, object> config)
        {
            string path = config["Path"].ToString();
            string directory = System.IO.Path.GetDirectoryName(path);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(path);
            string trimmedPath = System.IO.Path.Combine(directory, fileNameWithoutExtension);
            trainingPathTextBox.Text = trimmedPath;
            optimizerComboBox.SelectedItem = config["optimizer"].ToString();
            modelTypeComboBox.SelectedItem = config["model_type"].ToString();
            modelLibraryComboBox.SelectedItem = config["model_library"].ToString();
            versionComboBox.SelectedItem = config["tokenizer"].ToString();
            taskTypeComboBox.SelectedItem = config["task_type"].ToString();
            baseModelComboBox.SelectedItem = config["base_model"].ToString();
            maxSequenceTextBox.Text = config["max_sequence_length"].ToString();
            batchSizeTextBox.Text = config["batch_size"].ToString();
            trainingEpochsTextBox.Text = config["training_epochs"].ToString();
            warmupStepsTextBox.Text = config["warmup_steps"].ToString();
            convertToLowerCaseCheckBox.Checked = Convert.ToBoolean(config["convert_to_lowercase"]);
            removePunctuationCheckBox.Checked = Convert.ToBoolean(config["remove_punctuation"]);
            removeStopwordCheckBox.Checked = Convert.ToBoolean(config["remove_stopwords"]);
            freezeBertLayersCheckBox.Checked = Convert.ToBoolean(config["freeze_bert_layers"]);

            // Clear and repopulate trainable layers
            trainableLayersSelectedListBox.Items.Clear();
            trainableLayersPoolListBox.Items.Clear();
            if (config.TryGetValue("trainable_layers", out object trainableLayersObj))
            {
                if (trainableLayersObj is JArray trainableLayersArray)
                {
                    foreach (var layer in trainableLayersArray)
                    {
                        trainableLayersSelectedListBox.Items.Add(layer.ToString());
                    }
                }
            }

            // Clear and repopulate evaluation metrics
            evaluationMetricsSelectedListBox.Items.Clear();
            evaluationMetricsPoolListBox.Items.Clear();
            if (config.TryGetValue("evaluation_metrics", out object evaluationMetricsObj))
            {
                if (evaluationMetricsObj is JArray evaluationMetricsArray)
                {
                    foreach (var metric in evaluationMetricsArray)
                    {
                        evaluationMetricsSelectedListBox.Items.Add(metric.ToString());
                    }
                }
            }

            // Clear and repopulate outputs
            panelLayers.Controls.Clear();
            if (config.TryGetValue("outputs", out object outputsObj))
            {
                if (outputsObj is JArray outputsArray)
                {
                    // Set the number of outputs and trigger the creation of group boxes
                    numberOfOutputsComboBox.SelectedItem = outputsArray.Count.ToString();
                    numberOfOutputsComboBox_SelectedIndexChanged(null, null);

                    // Populate each group box with the relevant data
                    int index = 0;
                    foreach (var output in outputsArray)
                    {
                        var outputDict = output.ToObject<Dictionary<string, object>>();
                        GroupBox groupBox = panelLayers.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Tag.ToString() == $"Output{index}");
                        if (groupBox != null)
                        {
                            ((TextBox)groupBox.Controls[$"textBoxName{index}"]).Text = outputDict["name"].ToString();
                            ((TextBox)groupBox.Controls[$"textBoxNeurons{index}"]).Text = outputDict["neurons"].ToString();
                            ((ComboBox)groupBox.Controls[$"comboBoxOutputType{index}"]).SelectedItem = outputDict["type"].ToString();
                            ((ComboBox)groupBox.Controls[$"comboBoxActivation{index}"]).SelectedItem = outputDict["activation"].ToString();
                            ((ComboBox)groupBox.Controls[$"comboBoxLoss{index}"]).SelectedItem = outputDict["loss"].ToString();
                            ((CheckBox)groupBox.Controls[$"biasCheckBox{index}"]).Checked = outputDict.ContainsKey("use_bias") ? Convert.ToBoolean(outputDict["use_bias"]) : false;
                            ((TextBox)groupBox.Controls[$"textBoxDefaultScore{index}"]).Text = outputDict["default_score"].ToString();
                            if (outputDict.ContainsKey("weight"))
                            {
                                ((NumericUpDown)groupBox.Controls[$"numericUpDownWeight{index}"]).Value = Convert.ToDecimal(outputDict["weight"]);
                            }
                        }
                        index++;
                    }
                }
            }

            // Custom tokenizer settings
            customRadioButton.Checked = Convert.ToBoolean(config["custom_tokenizer"]);
            customTokenizerPathTextBox.Text = config["custom_tokenizer_path"].ToString();

            // Scheduler settings
            schedulerTypeComboBox.SelectedItem = config["scheduler_type"].ToString();
            switch (config["scheduler_type"].ToString())
            {
                case "Constant":
                    constantLearningRateTextBox.Text = config["constant_learning_rate"].ToString();
                    break;
                case "Step":
                case "Exponential":
                case "Inverse Time":
                    initialLearningRateTextBox.Text = config["initial_learning_rate"].ToString();
                    decayStepsTextBox.Text = config["decay_steps"].ToString();
                    decayRateTextBox.Text = config["decay_rate"].ToString();
                    break;
                case "Polynomial":
                    polyInitialLRTextBox.Text = config["initial_learning_rate"].ToString();
                    polyDecayStepsTextBox.Text = config["decay_steps"].ToString();
                    polyPowerTextBox.Text = config["power"].ToString();
                    polyEndLearningRateTextBox.Text = config["end_learning_rate"].ToString();
                    break;
                case "Cosine":
                    cosineInitialLRTextBox.Text = config["initial_learning_rate"].ToString();
                    cosineDecayStepsTextBox.Text = config["decay_steps"].ToString();
                    alphaTextBox.Text = config["alpha"].ToString();
                    break;
                case "ReduceLROnPlateau":
                    monitorLRComboBox.SelectedItem = config["monitor"].ToString();
                    LRfactorTextBox.Text = config["factor"].ToString();
                    patienceTextBox.Text = config["patience"].ToString();
                    LRminLearningRateTextBox.Text = config["minimum_learning_rate"].ToString();
                    break;
                case "Cyclic Learning Rate":
                    cyclicBaseLRTextBox.Text = config["base_learning_rate"].ToString();
                    cyclicStepUpTextBox.Text = config["step_size_up"].ToString();
                    cyclicTextBoxDown.Text = config["step_size_down"].ToString();
                    cyclicModeComboBox.SelectedItem = config["mode"].ToString();
                    cyclicMaxLearningRateTextBox.Text = config["max_learning_rate"].ToString();
                    break;
                case "OneCycleLR":
                    oneCycleMaxLearningRateTextBox.Text = config["max_learning_rate"].ToString();
                    oneCycleTotalStepsTextBox.Text = config["total_steps"].ToString();
                    oneCycleEpochsTextBox.Text = config["epochs"].ToString();
                    oneCycleStepsPerEpochTextBox.Text = config["steps_per_epoch"].ToString();
                    break;
            }
        }

        private void ByArticleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ByArticleRadioButton.Checked == true)
            {
                slidingWindowPanel.Visible = true;
            }
            if (ByArticleRadioButton.Checked == false)
            {
                slidingWindowPanel.Visible = false;
            }
        }

        private void saveTrainingSetButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Assuming the DataTable is bound to the DataGridView
                DataTable dataTable = (DataTable)TrainingDataGridView.DataSource;
                if (dataTable == null)
                {
                    MessageBox.Show("No data to save.");
                    return;
                }

                string outputDir = Path.GetDirectoryName(newModelPathTextBox.Text);
                string title = GlobalVariables.ActiveTitle;
                string sanitizedTitle = SanitizeFilename(title);
                string csvFilePath = Path.Combine(outputDir, $"{sanitizedTitle}_training.csv");

                // Save the DataTable to a CSV file using CsvHelper
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    ShouldQuote = args => true
                };

                using (var writer = new StreamWriter(csvFilePath))
                using (var csv = new CsvWriter(writer, config))
                {
                    // Write the header row
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();

                    // Write the data rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            csv.WriteField(row[column]);
                        }
                        csv.NextRecord();
                    }
                }

                MessageBox.Show($"Data saved to {csvFilePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving: {ex.Message}");
            }
        }

        private void Mimir_Control_Load(object sender, EventArgs e)
        {

        }

        private void trainingPathSelectButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;
                    trainingPathTextBox.Text = folderPath;
                }
            }
        }
        private void selectTrainingSetsButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Multiselect = true;
                ofd.Title = "Select CSV files";

                // Get the path from the textbox
                string initialDirectory = trainingPathTextBox.Text;

                // Set the initial directory to the parent directory
                if (!string.IsNullOrEmpty(initialDirectory) && System.IO.Directory.Exists(initialDirectory))
                {
                    var parentDirectory = System.IO.Directory.GetParent(initialDirectory);
                    if (parentDirectory != null)
                    {
                        ofd.InitialDirectory = parentDirectory.FullName;
                    }
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    trainingFileListBox.Items.AddRange(ofd.FileNames); // Add the file names to the ListBox
                }
            }
        }

        private void trainModelButton_Click(object sender, EventArgs e)
        {
            string configPath = newModelPathTextBox.Text;
            if (configPath.EndsWith(".pb", StringComparison.OrdinalIgnoreCase))
            {
                configPath = configPath.Substring(0, configPath.Length - 3) + ".json";
            }
            string ModelPath = trainingPathTextBox.Text;
            List<string> csvFiles = trainingFileListBox.Items.Cast<string>().ToList();
            string csvFilePathsArgument = string.Join(" ", csvFiles.Select(path => $"\"{path}\""));
            string arguments = $"\"{configPath}\" \"{ModelPath}\" {csvFilePathsArgument}";

            if (csvFiles.Count > 0)
            {
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = "TrainBERTModel.exe",
                    WorkingDirectory = GlobalVariables.CuratorPath,
                    Arguments = arguments,
                    UseShellExecute = true,
                };
                using (Process process = new Process { StartInfo = start })
                {
                    process.Start();
                }
            }
            else
            {
                MessageBox.Show("No CSV files selected for training.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

