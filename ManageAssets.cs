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
    public partial class ManageAssets : Form
    {
        string AssetDBConnect = $"server={GlobalVariables.assetIP};uid={GlobalVariables.assetUser};pwd={GlobalVariables.assetPW};database={GlobalVariables.assetDBName}";

        public ManageAssets()
        {
            InitializeComponent();
            using (MySqlConnection connection = new MySqlConnection(AssetDBConnect))
            {
                MySqlCommand command = new MySqlCommand("SELECT asset, commonname, market, active FROM stocks ORDER BY asset ASC", connection);
                DataTable dataTable = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
                

                // Add a new column to the DataTable to display the checkmark
                DataColumn checkmarkColumn = new DataColumn("Activated", typeof(string));
                checkmarkColumn.Expression = "IIF(active = 1, '\u2713', '')";
                dataTable.Columns.Add(checkmarkColumn);

                // Set the DataSource of the DataGridView to the modified DataTable
                dataGridView1.DataSource = dataTable;

                // Hide the "active" column, since the checkmark column now displays the value
                dataGridView1.Columns["active"].Visible = false;
            }
        }

        private void countrylabel_Click(object sender, EventArgs e)
        {

        }
    }

       
    
}
