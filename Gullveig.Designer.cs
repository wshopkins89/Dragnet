namespace DragnetControl
{
    partial class Gullveig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gullveig));
            this.politiciansListBox = new System.Windows.Forms.ListBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.PoliticiansTab = new System.Windows.Forms.TabPage();
            this.politicianTradeDataGridView = new System.Windows.Forms.DataGridView();
            this.lastTradeLabel = new System.Windows.Forms.Label();
            this.volumeLabel = new System.Windows.Forms.Label();
            this.totalTradesLabel = new System.Windows.Forms.Label();
            this.stateLabel = new System.Windows.Forms.Label();
            this.chamberLabel = new System.Windows.Forms.Label();
            this.partyLabel = new System.Windows.Forms.Label();
            this.politicianNameLabel = new System.Windows.Forms.Label();
            this.politicianPictureBox = new System.Windows.Forms.PictureBox();
            this.BillsTab = new System.Windows.Forms.TabPage();
            this.senatorsCheckBox = new System.Windows.Forms.CheckBox();
            this.representativesCheckBox = new System.Windows.Forms.CheckBox();
            this.democratCheckBox = new System.Windows.Forms.CheckBox();
            this.republicansCheckBox = new System.Windows.Forms.CheckBox();
            this.otherCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxHouse = new System.Windows.Forms.PictureBox();
            this.pictureBoxSenate = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.refreshGovernmentButton = new System.Windows.Forms.Button();
            this.buildCongressionalDatabaseButton = new System.Windows.Forms.Button();
            this.checkRecentActivityButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.PoliticiansTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.politicianTradeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.politicianPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHouse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSenate)).BeginInit();
            this.SuspendLayout();
            // 
            // politiciansListBox
            // 
            this.politiciansListBox.FormattingEnabled = true;
            this.politiciansListBox.ItemHeight = 14;
            this.politiciansListBox.Location = new System.Drawing.Point(6, 90);
            this.politiciansListBox.Name = "politiciansListBox";
            this.politiciansListBox.ScrollAlwaysVisible = true;
            this.politiciansListBox.Size = new System.Drawing.Size(254, 522);
            this.politiciansListBox.TabIndex = 0;
            this.politiciansListBox.SelectedIndexChanged += new System.EventHandler(this.politiciansListBox_SelectedIndexChanged);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(4, 3);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(175, 22);
            this.searchTextBox.TabIndex = 1;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(185, 3);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 2;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.PoliticiansTab);
            this.tabControl1.Controls.Add(this.BillsTab);
            this.tabControl1.Location = new System.Drawing.Point(90, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1381, 657);
            this.tabControl1.TabIndex = 3;
            // 
            // PoliticiansTab
            // 
            this.PoliticiansTab.Controls.Add(this.politicianTradeDataGridView);
            this.PoliticiansTab.Controls.Add(this.checkRecentActivityButton);
            this.PoliticiansTab.Controls.Add(this.lastTradeLabel);
            this.PoliticiansTab.Controls.Add(this.buildCongressionalDatabaseButton);
            this.PoliticiansTab.Controls.Add(this.volumeLabel);
            this.PoliticiansTab.Controls.Add(this.refreshGovernmentButton);
            this.PoliticiansTab.Controls.Add(this.totalTradesLabel);
            this.PoliticiansTab.Controls.Add(this.label3);
            this.PoliticiansTab.Controls.Add(this.stateLabel);
            this.PoliticiansTab.Controls.Add(this.label2);
            this.PoliticiansTab.Controls.Add(this.chamberLabel);
            this.PoliticiansTab.Controls.Add(this.pictureBoxSenate);
            this.PoliticiansTab.Controls.Add(this.pictureBoxHouse);
            this.PoliticiansTab.Controls.Add(this.partyLabel);
            this.PoliticiansTab.Controls.Add(this.politicianNameLabel);
            this.PoliticiansTab.Controls.Add(this.label1);
            this.PoliticiansTab.Controls.Add(this.politicianPictureBox);
            this.PoliticiansTab.Controls.Add(this.otherCheckBox);
            this.PoliticiansTab.Controls.Add(this.democratCheckBox);
            this.PoliticiansTab.Controls.Add(this.republicansCheckBox);
            this.PoliticiansTab.Controls.Add(this.politiciansListBox);
            this.PoliticiansTab.Controls.Add(this.searchTextBox);
            this.PoliticiansTab.Controls.Add(this.representativesCheckBox);
            this.PoliticiansTab.Controls.Add(this.searchButton);
            this.PoliticiansTab.Controls.Add(this.senatorsCheckBox);
            this.PoliticiansTab.Location = new System.Drawing.Point(4, 4);
            this.PoliticiansTab.Name = "PoliticiansTab";
            this.PoliticiansTab.Padding = new System.Windows.Forms.Padding(3);
            this.PoliticiansTab.Size = new System.Drawing.Size(1373, 630);
            this.PoliticiansTab.TabIndex = 0;
            this.PoliticiansTab.Text = "Politicians";
            this.PoliticiansTab.UseVisualStyleBackColor = true;
            // 
            // politicianTradeDataGridView
            // 
            this.politicianTradeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.politicianTradeDataGridView.Location = new System.Drawing.Point(288, 218);
            this.politicianTradeDataGridView.Name = "politicianTradeDataGridView";
            this.politicianTradeDataGridView.Size = new System.Drawing.Size(660, 406);
            this.politicianTradeDataGridView.TabIndex = 8;
            // 
            // lastTradeLabel
            // 
            this.lastTradeLabel.AutoSize = true;
            this.lastTradeLabel.Location = new System.Drawing.Point(437, 157);
            this.lastTradeLabel.Name = "lastTradeLabel";
            this.lastTradeLabel.Size = new System.Drawing.Size(132, 14);
            this.lastTradeLabel.TabIndex = 7;
            this.lastTradeLabel.Text = "Date of Last Trade: ";
            // 
            // volumeLabel
            // 
            this.volumeLabel.AutoSize = true;
            this.volumeLabel.Location = new System.Drawing.Point(437, 143);
            this.volumeLabel.Name = "volumeLabel";
            this.volumeLabel.Size = new System.Drawing.Size(54, 14);
            this.volumeLabel.TabIndex = 6;
            this.volumeLabel.Text = "Volume:";
            // 
            // totalTradesLabel
            // 
            this.totalTradesLabel.AutoSize = true;
            this.totalTradesLabel.Location = new System.Drawing.Point(437, 129);
            this.totalTradesLabel.Name = "totalTradesLabel";
            this.totalTradesLabel.Size = new System.Drawing.Size(91, 14);
            this.totalTradesLabel.TabIndex = 5;
            this.totalTradesLabel.Text = "Total Trades: ";
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Font = new System.Drawing.Font("Audiowide", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateLabel.Location = new System.Drawing.Point(519, 78);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(51, 17);
            this.stateLabel.TabIndex = 4;
            this.stateLabel.Text = "label2";
            // 
            // chamberLabel
            // 
            this.chamberLabel.AutoSize = true;
            this.chamberLabel.Font = new System.Drawing.Font("Audiowide", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chamberLabel.Location = new System.Drawing.Point(518, 42);
            this.chamberLabel.Name = "chamberLabel";
            this.chamberLabel.Size = new System.Drawing.Size(60, 19);
            this.chamberLabel.TabIndex = 3;
            this.chamberLabel.Text = "label2";
            // 
            // partyLabel
            // 
            this.partyLabel.AutoSize = true;
            this.partyLabel.Font = new System.Drawing.Font("Audiowide", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.partyLabel.Location = new System.Drawing.Point(519, 61);
            this.partyLabel.Name = "partyLabel";
            this.partyLabel.Size = new System.Drawing.Size(51, 17);
            this.partyLabel.TabIndex = 2;
            this.partyLabel.Text = "label2";
            // 
            // politicianNameLabel
            // 
            this.politicianNameLabel.AutoSize = true;
            this.politicianNameLabel.Font = new System.Drawing.Font("Audiowide", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.politicianNameLabel.Location = new System.Drawing.Point(493, 11);
            this.politicianNameLabel.Name = "politicianNameLabel";
            this.politicianNameLabel.Size = new System.Drawing.Size(97, 31);
            this.politicianNameLabel.TabIndex = 1;
            this.politicianNameLabel.Text = "label2";
            // 
            // politicianPictureBox
            // 
            this.politicianPictureBox.Location = new System.Drawing.Point(289, 11);
            this.politicianPictureBox.Name = "politicianPictureBox";
            this.politicianPictureBox.Size = new System.Drawing.Size(142, 201);
            this.politicianPictureBox.TabIndex = 0;
            this.politicianPictureBox.TabStop = false;
            // 
            // BillsTab
            // 
            this.BillsTab.Location = new System.Drawing.Point(4, 4);
            this.BillsTab.Name = "BillsTab";
            this.BillsTab.Padding = new System.Windows.Forms.Padding(3);
            this.BillsTab.Size = new System.Drawing.Size(1373, 630);
            this.BillsTab.TabIndex = 1;
            this.BillsTab.Text = "Bills";
            this.BillsTab.UseVisualStyleBackColor = true;
            // 
            // senatorsCheckBox
            // 
            this.senatorsCheckBox.AutoSize = true;
            this.senatorsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.senatorsCheckBox.Checked = true;
            this.senatorsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.senatorsCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.senatorsCheckBox.Location = new System.Drawing.Point(6, 69);
            this.senatorsCheckBox.Name = "senatorsCheckBox";
            this.senatorsCheckBox.Size = new System.Drawing.Size(84, 18);
            this.senatorsCheckBox.TabIndex = 4;
            this.senatorsCheckBox.Text = "Senators";
            this.senatorsCheckBox.UseVisualStyleBackColor = false;
            // 
            // representativesCheckBox
            // 
            this.representativesCheckBox.AutoSize = true;
            this.representativesCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.representativesCheckBox.Checked = true;
            this.representativesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.representativesCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.representativesCheckBox.Location = new System.Drawing.Point(87, 69);
            this.representativesCheckBox.Name = "representativesCheckBox";
            this.representativesCheckBox.Size = new System.Drawing.Size(128, 18);
            this.representativesCheckBox.TabIndex = 5;
            this.representativesCheckBox.Text = "Representatives";
            this.representativesCheckBox.UseVisualStyleBackColor = false;
            // 
            // democratCheckBox
            // 
            this.democratCheckBox.AutoSize = true;
            this.democratCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.democratCheckBox.Checked = true;
            this.democratCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.democratCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.democratCheckBox.Location = new System.Drawing.Point(6, 45);
            this.democratCheckBox.Name = "democratCheckBox";
            this.democratCheckBox.Size = new System.Drawing.Size(93, 18);
            this.democratCheckBox.TabIndex = 6;
            this.democratCheckBox.Text = "Democrats";
            this.democratCheckBox.UseVisualStyleBackColor = false;
            // 
            // republicansCheckBox
            // 
            this.republicansCheckBox.AutoSize = true;
            this.republicansCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.republicansCheckBox.Checked = true;
            this.republicansCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.republicansCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.republicansCheckBox.Location = new System.Drawing.Point(105, 45);
            this.republicansCheckBox.Name = "republicansCheckBox";
            this.republicansCheckBox.Size = new System.Drawing.Size(101, 18);
            this.republicansCheckBox.TabIndex = 7;
            this.republicansCheckBox.Text = "Republicans";
            this.republicansCheckBox.UseVisualStyleBackColor = false;
            // 
            // otherCheckBox
            // 
            this.otherCheckBox.AutoSize = true;
            this.otherCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.otherCheckBox.Checked = true;
            this.otherCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.otherCheckBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.otherCheckBox.Location = new System.Drawing.Point(204, 45);
            this.otherCheckBox.Name = "otherCheckBox";
            this.otherCheckBox.Size = new System.Drawing.Size(62, 18);
            this.otherCheckBox.TabIndex = 8;
            this.otherCheckBox.Text = "Other";
            this.otherCheckBox.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(102, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 14);
            this.label1.TabIndex = 9;
            this.label1.Text = "Filters";
            // 
            // pictureBoxHouse
            // 
            this.pictureBoxHouse.Location = new System.Drawing.Point(966, 33);
            this.pictureBoxHouse.Name = "pictureBoxHouse";
            this.pictureBoxHouse.Size = new System.Drawing.Size(399, 216);
            this.pictureBoxHouse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxHouse.TabIndex = 10;
            this.pictureBoxHouse.TabStop = false;
            // 
            // pictureBoxSenate
            // 
            this.pictureBoxSenate.Location = new System.Drawing.Point(966, 292);
            this.pictureBoxSenate.Name = "pictureBoxSenate";
            this.pictureBoxSenate.Size = new System.Drawing.Size(399, 219);
            this.pictureBoxSenate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxSenate.TabIndex = 11;
            this.pictureBoxSenate.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(962, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(409, 21);
            this.label2.TabIndex = 12;
            this.label2.Text = "Balance of Power: House of Representatives";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(1013, 268);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 21);
            this.label3.TabIndex = 13;
            this.label3.Text = "Balance of Power: U.S Senate";
            // 
            // refreshGovernmentButton
            // 
            this.refreshGovernmentButton.Location = new System.Drawing.Point(1074, 532);
            this.refreshGovernmentButton.Name = "refreshGovernmentButton";
            this.refreshGovernmentButton.Size = new System.Drawing.Size(165, 23);
            this.refreshGovernmentButton.TabIndex = 14;
            this.refreshGovernmentButton.Text = "Refresh Government";
            this.refreshGovernmentButton.UseVisualStyleBackColor = true;
            this.refreshGovernmentButton.Click += new System.EventHandler(this.refreshGovernmentButton_Click);
            // 
            // buildCongressionalDatabaseButton
            // 
            this.buildCongressionalDatabaseButton.Location = new System.Drawing.Point(1074, 561);
            this.buildCongressionalDatabaseButton.Name = "buildCongressionalDatabaseButton";
            this.buildCongressionalDatabaseButton.Size = new System.Drawing.Size(165, 23);
            this.buildCongressionalDatabaseButton.TabIndex = 15;
            this.buildCongressionalDatabaseButton.Text = "Retrieve All Trades";
            this.buildCongressionalDatabaseButton.UseVisualStyleBackColor = true;
            this.buildCongressionalDatabaseButton.Click += new System.EventHandler(this.buildCongressionalDatabaseButton_Click);
            // 
            // checkRecentActivityButton
            // 
            this.checkRecentActivityButton.Location = new System.Drawing.Point(1074, 590);
            this.checkRecentActivityButton.Name = "checkRecentActivityButton";
            this.checkRecentActivityButton.Size = new System.Drawing.Size(165, 23);
            this.checkRecentActivityButton.TabIndex = 16;
            this.checkRecentActivityButton.Text = "Check Recent Activity";
            this.checkRecentActivityButton.UseVisualStyleBackColor = true;
            this.checkRecentActivityButton.Click += new System.EventHandler(this.checkRecentActivityButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(1394, 668);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(69, 23);
            this.exitButton.TabIndex = 17;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // Gullveig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1516, 703);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Gullveig";
            this.Text = "Gullveig";
            this.tabControl1.ResumeLayout(false);
            this.PoliticiansTab.ResumeLayout(false);
            this.PoliticiansTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.politicianTradeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.politicianPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHouse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSenate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox politiciansListBox;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage PoliticiansTab;
        private System.Windows.Forms.TabPage BillsTab;
        private System.Windows.Forms.CheckBox senatorsCheckBox;
        private System.Windows.Forms.CheckBox representativesCheckBox;
        private System.Windows.Forms.CheckBox democratCheckBox;
        private System.Windows.Forms.CheckBox republicansCheckBox;
        private System.Windows.Forms.CheckBox otherCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxHouse;
        private System.Windows.Forms.PictureBox pictureBoxSenate;
        private System.Windows.Forms.Label partyLabel;
        private System.Windows.Forms.Label politicianNameLabel;
        private System.Windows.Forms.PictureBox politicianPictureBox;
        private System.Windows.Forms.Label chamberLabel;
        private System.Windows.Forms.Label lastTradeLabel;
        private System.Windows.Forms.Label volumeLabel;
        private System.Windows.Forms.Label totalTradesLabel;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.DataGridView politicianTradeDataGridView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button refreshGovernmentButton;
        private System.Windows.Forms.Button buildCongressionalDatabaseButton;
        private System.Windows.Forms.Button checkRecentActivityButton;
        private System.Windows.Forms.Button exitButton;
    }
}