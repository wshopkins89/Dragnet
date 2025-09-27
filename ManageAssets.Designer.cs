
namespace DragnetControl
{
    partial class ManageAssets
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageAssets));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.StocksTab = new System.Windows.Forms.TabPage();
            this.CryptoTab = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.ForexTab = new System.Windows.Forms.TabPage();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.AssetLabel = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.commonnamelabel = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.countrylabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.StocksTab.SuspendLayout();
            this.CryptoTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.ForexTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(53, 31);
            this.textBox1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(515, 33);
            this.textBox1.TabIndex = 0;
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(229, 76);
            this.SearchButton.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(175, 46);
            this.SearchButton.TabIndex = 1;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(728, 813);
            this.dataGridView1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.StocksTab);
            this.tabControl1.Controls.Add(this.CryptoTab);
            this.tabControl1.Controls.Add(this.ForexTab);
            this.tabControl1.Location = new System.Drawing.Point(618, 24);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(737, 852);
            this.tabControl1.TabIndex = 3;
            // 
            // StocksTab
            // 
            this.StocksTab.Controls.Add(this.dataGridView1);
            this.StocksTab.Location = new System.Drawing.Point(4, 35);
            this.StocksTab.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.StocksTab.Name = "StocksTab";
            this.StocksTab.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.StocksTab.Size = new System.Drawing.Size(729, 813);
            this.StocksTab.TabIndex = 0;
            this.StocksTab.Text = "Stocks";
            this.StocksTab.UseVisualStyleBackColor = true;
            // 
            // CryptoTab
            // 
            this.CryptoTab.Controls.Add(this.dataGridView2);
            this.CryptoTab.Location = new System.Drawing.Point(4, 35);
            this.CryptoTab.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.CryptoTab.Name = "CryptoTab";
            this.CryptoTab.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.CryptoTab.Size = new System.Drawing.Size(729, 813);
            this.CryptoTab.TabIndex = 1;
            this.CryptoTab.Text = "Crypto";
            this.CryptoTab.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 51;
            this.dataGridView2.Size = new System.Drawing.Size(729, 813);
            this.dataGridView2.TabIndex = 0;
            // 
            // ForexTab
            // 
            this.ForexTab.Controls.Add(this.dataGridView3);
            this.ForexTab.Location = new System.Drawing.Point(4, 35);
            this.ForexTab.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ForexTab.Name = "ForexTab";
            this.ForexTab.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ForexTab.Size = new System.Drawing.Size(729, 813);
            this.ForexTab.TabIndex = 2;
            this.ForexTab.Text = "Forex";
            this.ForexTab.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Location = new System.Drawing.Point(0, 0);
            this.dataGridView3.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowHeadersWidth = 51;
            this.dataGridView3.Size = new System.Drawing.Size(729, 817);
            this.dataGridView3.TabIndex = 0;
            // 
            // AssetLabel
            // 
            this.AssetLabel.AutoSize = true;
            this.AssetLabel.Location = new System.Drawing.Point(121, 151);
            this.AssetLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.AssetLabel.Name = "AssetLabel";
            this.AssetLabel.Size = new System.Drawing.Size(82, 26);
            this.AssetLabel.TabIndex = 4;
            this.AssetLabel.Text = "Asset:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(229, 148);
            this.textBox2.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(339, 33);
            this.textBox2.TabIndex = 5;
            // 
            // commonnamelabel
            // 
            this.commonnamelabel.AutoSize = true;
            this.commonnamelabel.Location = new System.Drawing.Point(28, 210);
            this.commonnamelabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.commonnamelabel.Name = "commonnamelabel";
            this.commonnamelabel.Size = new System.Drawing.Size(175, 26);
            this.commonnamelabel.TabIndex = 6;
            this.commonnamelabel.Text = "Common Name:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(229, 207);
            this.textBox3.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(339, 33);
            this.textBox3.TabIndex = 7;
            // 
            // countrylabel
            // 
            this.countrylabel.AutoSize = true;
            this.countrylabel.Location = new System.Drawing.Point(93, 279);
            this.countrylabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.countrylabel.Name = "countrylabel";
            this.countrylabel.Size = new System.Drawing.Size(110, 26);
            this.countrylabel.TabIndex = 8;
            this.countrylabel.Text = "Country:";
            this.countrylabel.Click += new System.EventHandler(this.countrylabel_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "United States",
            "Afghanistan",
            "Albania",
            "Algeria",
            "Andorra",
            "Angola",
            "Antigua and Barbuda",
            "Argentina",
            "Armenia",
            "Australia",
            "Austria",
            "Azerbaijan",
            "Bahamas",
            "Bahrain",
            "Bangladesh",
            "Barbados",
            "Belarus",
            "Belgium",
            "Belize",
            "Benin",
            "Bhutan",
            "Bolivia",
            "Bosnia and Herzegovina",
            "Botswana",
            "Brazil",
            "Brunei",
            "Bulgaria",
            "Burkina Faso",
            "Burundi",
            "Cote d\'Ivoire",
            "Cabo Verde",
            "Cambodia",
            "Cameroon",
            "Canada",
            "Central African Republic",
            "Chad",
            "Chile",
            "China",
            "Colombia",
            "Comoros",
            "Congo",
            "Costa Rica",
            "Croatia",
            "Cuba",
            "Cyprus",
            "Czechia (Czech Republic)",
            "Democratic Republic of the Congo",
            "Denmark",
            "Djibouti",
            "Dominica",
            "Domincian Republic",
            "Ecuador",
            "Egypt",
            "El Salvador",
            "Equatorial Guinea",
            "Eritrea",
            "Estonia",
            "Eswatini",
            "Ethiopia",
            "Fiji",
            "Finland",
            "France",
            "Gabon",
            "Gambia",
            "Georgia",
            "Germany",
            "Ghana",
            "Greece",
            "Grenada",
            "Guatemala",
            "Guinea",
            "Guinea-Bissau",
            "Guyana",
            "Hati",
            "Holy See",
            "Honduras",
            "Hungary",
            "Iceland",
            "India",
            "Indonesia",
            "Iran",
            "Iraq",
            "Ireland",
            "Israel",
            "Italy",
            "Jamaica",
            "Japan",
            "Jordan",
            "Kazakhstan",
            "Kenya",
            "Kiribati",
            "Kuwait",
            "Kyrgyzstan",
            "Laos",
            "Latvia",
            "Lebanon",
            "Lesotho",
            "Liberia",
            "Libya",
            "Liechtenstein",
            "Lithuania",
            "Luxembourg",
            "Madagascar",
            "Malawi",
            "Malaysia",
            "Maldives",
            "Mali",
            "Malta",
            "Marshall Islands",
            "Mauritania",
            "Mauritius",
            "Mexico",
            "Micronesia",
            "Moldova",
            "Monaco",
            "Mongolia",
            "Montenegro",
            "Morocco",
            "Mozambique",
            "Myanmar",
            "Namibia",
            "Nauru",
            "Nepal",
            "Netherlands",
            "New Zealand",
            "Nicaragua",
            "Niger",
            "Nigeria",
            "North Korea (lol)",
            "North Macedonia",
            "Norway",
            "Oman",
            "Pakistan",
            "Palau",
            "Panama",
            "Papua New Guinea",
            "Paraguay",
            "Peru",
            "Phillipines",
            "Poland",
            "Portugal",
            "Qatar",
            "Romania",
            "Russia",
            "Rwanda",
            "Saint Kitts and Nevis",
            "Saint Lucia",
            "Saint Vincent and the Grenadines",
            "Samoa",
            "San Marino",
            "Sao Tome and Principe",
            "Saudia Arabia",
            "Senegal",
            "Serbia",
            "Seycheles",
            "Sierra Leone",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "Solomon Islands",
            "Somalia",
            "South Africa",
            "South Korea",
            "South Sudan",
            "Spain",
            "Sri Lanka",
            "Sudan",
            "Suriname",
            "Sweden",
            "Switzerland",
            "Syria",
            "Tajikistan",
            "Tanzania",
            "Thailand",
            "Timor-Leste",
            "Togo",
            "Tonga",
            "Trinidad and Tobago",
            "Tunisia",
            "Turkey",
            "Turkmenistan",
            "Tuvalu",
            "Uganda",
            "Ukraine",
            "United Arab Emirates",
            "United Kingdom",
            "Uruguay",
            "Uzbekistan",
            "Vanuatu",
            "Venezuela",
            "Vietnam",
            "Yemen",
            "Zambia",
            "Zimbabwe"});
            this.comboBox1.Location = new System.Drawing.Point(229, 276);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(339, 34);
            this.comboBox1.TabIndex = 9;
            // 
            // ManageAssets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 900);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.countrylabel);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.commonnamelabel);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.AssetLabel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "ManageAssets";
            this.Text = "Manage Assets";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.StocksTab.ResumeLayout(false);
            this.CryptoTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ForexTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage StocksTab;
        private System.Windows.Forms.TabPage CryptoTab;
        private System.Windows.Forms.TabPage ForexTab;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Label AssetLabel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label commonnamelabel;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label countrylabel;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}