using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragnetControl
{
    public partial class ManuallyBuildDatabase : Form
    {
        string exchange;
        string asset;
        string interval;
        string granularity;
        String StartDate;
        String EndDate;
        

        public ManuallyBuildDatabase()
        {
            InitializeComponent();
            StartDateTimePicker.Value = DateTime.Now.AddHours(-1);
            endTimeDatePicker.Value = DateTime.Now;
            EndTimeTextBox.Text = DateTime.Now.ToString("HH-mm");
            StartTimeTextBox.Text= DateTime.Now.AddHours(-1).ToString("HH-mm");
            exchangeComboBox.Text = "Coinbase";
            assetComboBox.Text = "All Assets";
            intervalComboBox.Text = "One Hour";
        }
        private void StartProcess(string fileName, string workingDirectory, string arguments, bool createNoWindow = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                Arguments = $"\"{arguments}\"",
                CreateNoWindow = createNoWindow,
                UseShellExecute = true,
            };
            Process.Start(startInfo);
        }
        private void BuildQuery()
        {
            exchange = exchangeComboBox.Text;
            asset = assetComboBox.Text;
            StartDate = StartDateTimePicker.Value.ToString("yyyy-MM-dd") + "-" + StartTimeTextBox.Text.Replace(":", "-");
            EndDate = endTimeDatePicker.Value.ToString("yyyy-MM-dd") + "-" + EndTimeTextBox.Text.Replace(":", "-");
         /*
            if (intervalComboBox.Text == "Working Database")
            {
                interval = "dragnet";
                granularity = "60";
            }
          */
            if (intervalComboBox.Text == "One Minute")
            {
                interval = "dragnet";
                granularity = "60";
            }
           /* if (intervalComboBox.Text == "Five Minutes")
            {
                interval = "minute5";
                granularity = "300";
            }
            if (intervalComboBox.Text == "Fifteen Minutes")
            {
                interval = "minute15";
                granularity = "900";
            }
            if (intervalComboBox.Text == "One Hour")
            {
                interval = "minute60";
                granularity = "3600";
            }
            if (intervalComboBox.Text == "Six Hours")
            {
                interval = "minute360";
                granularity = "21600";
            }
            if (intervalComboBox.Text == "One Day")
            {
                interval = "minute1440";
                granularity = "86400";
            }
          */
            if(intervalComboBox.Text == "")
            {
                //Create a message box to tell the user to select an interval.
                MessageBox.Show("Please select an interval.");
            }
        }
        private void BuildButton_Click(object sender, EventArgs e)
        {
            BuildQuery();
            Process[] processes = Process.GetProcesses();
            {
                if (exchange == "Coinbase")
                {
                    StartProcess("BuildCoinbaseDatabase.exe", GlobalVariables.CoinbaseScannerPath, $"{GlobalVariables.username},{interval},{granularity},{StartDate},{EndDate}");
                }
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StartDateTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void RecalculateButton_Click(object sender, EventArgs e)
        {
            BuildQuery();
            Process[] processes = Process.GetProcesses();
            {
                if (exchange == "Coinbase")
                {
                    StartProcess("CleanupCurator.exe", GlobalVariables.CoinbaseScannerPath, $"{interval},{StartDate},{EndDate}");
                }
            }
        }

        private void ManuallyBuildDatabase_Load(object sender, EventArgs e)
        {

        }
    }
}
