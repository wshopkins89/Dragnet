namespace DragnetControl
{
    partial class Ratatoskr_Control
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mimir_Control));
            this.SourceDataGroupBox = new System.Windows.Forms.GroupBox();
            this.ByArticleRadioButton = new System.Windows.Forms.RadioButton();
            this.BySentenceRadioButton = new System.Windows.Forms.RadioButton();
            this.retrieveButton = new System.Windows.Forms.Button();
            this.ArticlesDataGridView = new System.Windows.Forms.DataGridView();
            this.AssetListBox = new System.Windows.Forms.ListBox();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.SchemaComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TrainingDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.addSchemaButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.TableNameTextBox = new System.Windows.Forms.TextBox();
            this.SchemaComboBox2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.SourceDataGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ArticlesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrainingDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SourceDataGroupBox
            // 
            this.SourceDataGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.SourceDataGroupBox.Controls.Add(this.ByArticleRadioButton);
            this.SourceDataGroupBox.Controls.Add(this.BySentenceRadioButton);
            this.SourceDataGroupBox.Controls.Add(this.retrieveButton);
            this.SourceDataGroupBox.Controls.Add(this.ArticlesDataGridView);
            this.SourceDataGroupBox.Controls.Add(this.AssetListBox);
            this.SourceDataGroupBox.Controls.Add(this.URLTextBox);
            this.SourceDataGroupBox.Controls.Add(this.SchemaComboBox);
            this.SourceDataGroupBox.Controls.Add(this.label1);
            this.SourceDataGroupBox.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SourceDataGroupBox.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.SourceDataGroupBox.Location = new System.Drawing.Point(26, 12);
            this.SourceDataGroupBox.Name = "SourceDataGroupBox";
            this.SourceDataGroupBox.Size = new System.Drawing.Size(1549, 228);
            this.SourceDataGroupBox.TabIndex = 0;
            this.SourceDataGroupBox.TabStop = false;
            this.SourceDataGroupBox.Text = "Source Data";
            // 
            // ByArticleRadioButton
            // 
            this.ByArticleRadioButton.AutoSize = true;
            this.ByArticleRadioButton.Location = new System.Drawing.Point(10, 202);
            this.ByArticleRadioButton.Name = "ByArticleRadioButton";
            this.ByArticleRadioButton.Size = new System.Drawing.Size(101, 21);
            this.ByArticleRadioButton.TabIndex = 7;
            this.ByArticleRadioButton.Text = "By &Article";
            this.ByArticleRadioButton.UseVisualStyleBackColor = true;
            // 
            // BySentenceRadioButton
            // 
            this.BySentenceRadioButton.AutoSize = true;
            this.BySentenceRadioButton.Checked = true;
            this.BySentenceRadioButton.Location = new System.Drawing.Point(10, 179);
            this.BySentenceRadioButton.Name = "BySentenceRadioButton";
            this.BySentenceRadioButton.Size = new System.Drawing.Size(120, 21);
            this.BySentenceRadioButton.TabIndex = 6;
            this.BySentenceRadioButton.TabStop = true;
            this.BySentenceRadioButton.Text = "By &Sentence";
            this.BySentenceRadioButton.UseVisualStyleBackColor = true;
            // 
            // retrieveButton
            // 
            this.retrieveButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.retrieveButton.Location = new System.Drawing.Point(10, 145);
            this.retrieveButton.Name = "retrieveButton";
            this.retrieveButton.Size = new System.Drawing.Size(178, 27);
            this.retrieveButton.TabIndex = 5;
            this.retrieveButton.Text = "&Retrieve";
            this.retrieveButton.UseVisualStyleBackColor = true;
            this.retrieveButton.Click += new System.EventHandler(this.retrieveButton_Click);
            // 
            // ArticlesDataGridView
            // 
            this.ArticlesDataGridView.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.ArticlesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ArticlesDataGridView.Location = new System.Drawing.Point(194, 50);
            this.ArticlesDataGridView.Name = "ArticlesDataGridView";
            this.ArticlesDataGridView.RowHeadersWidth = 51;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.ArticlesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.ArticlesDataGridView.RowTemplate.Height = 24;
            this.ArticlesDataGridView.Size = new System.Drawing.Size(1349, 172);
            this.ArticlesDataGridView.TabIndex = 4;
            this.ArticlesDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ArticlesDataGridView_CellContentClick);
            // 
            // AssetListBox
            // 
            this.AssetListBox.FormattingEnabled = true;
            this.AssetListBox.ItemHeight = 17;
            this.AssetListBox.Location = new System.Drawing.Point(10, 50);
            this.AssetListBox.Name = "AssetListBox";
            this.AssetListBox.Size = new System.Drawing.Size(178, 89);
            this.AssetListBox.TabIndex = 3;
            this.AssetListBox.SelectedIndexChanged += new System.EventHandler(this.AssetListBox_SelectedIndexChanged);
            // 
            // URLTextBox
            // 
            this.URLTextBox.Location = new System.Drawing.Point(440, 21);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(1100, 24);
            this.URLTextBox.TabIndex = 2;
            // 
            // SchemaComboBox
            // 
            this.SchemaComboBox.FormattingEnabled = true;
            this.SchemaComboBox.Location = new System.Drawing.Point(97, 19);
            this.SchemaComboBox.Name = "SchemaComboBox";
            this.SchemaComboBox.Size = new System.Drawing.Size(214, 25);
            this.SchemaComboBox.TabIndex = 1;
            this.SchemaComboBox.SelectedIndexChanged += new System.EventHandler(this.SchemaComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(427, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Schema:                                                                     Targe" +
    "t URL:";
            // 
            // TrainingDataGridView
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TrainingDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.TrainingDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.TrainingDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.TrainingDataGridView.Location = new System.Drawing.Point(26, 246);
            this.TrainingDataGridView.Name = "TrainingDataGridView";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TrainingDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.TrainingDataGridView.RowHeadersWidth = 51;
            this.TrainingDataGridView.RowTemplate.Height = 24;
            this.TrainingDataGridView.Size = new System.Drawing.Size(1543, 455);
            this.TrainingDataGridView.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.addSchemaButton);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.TableNameTextBox);
            this.groupBox1.Controls.Add(this.SchemaComboBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Location = new System.Drawing.Point(26, 707);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1549, 99);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target Data";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Audiowide", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button4.Location = new System.Drawing.Point(281, 57);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(178, 27);
            this.button4.TabIndex = 10;
            this.button4.Text = "Tokenize";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Audiowide", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button3.Location = new System.Drawing.Point(97, 57);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 27);
            this.button3.TabIndex = 9;
            this.button3.Text = "Auto-Score";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // addSchemaButton
            // 
            this.addSchemaButton.Font = new System.Drawing.Font("Audiowide", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addSchemaButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.addSchemaButton.Location = new System.Drawing.Point(318, 20);
            this.addSchemaButton.Name = "addSchemaButton";
            this.addSchemaButton.Size = new System.Drawing.Size(26, 24);
            this.addSchemaButton.TabIndex = 8;
            this.addSchemaButton.Text = "+";
            this.addSchemaButton.UseVisualStyleBackColor = true;
            this.addSchemaButton.Click += new System.EventHandler(this.addSchemaButton_Click);
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button2.Location = new System.Drawing.Point(1371, 47);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 27);
            this.button2.TabIndex = 7;
            this.button2.Text = "Save As";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Audiowide", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button1.Location = new System.Drawing.Point(1371, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 27);
            this.button1.TabIndex = 6;
            this.button1.Text = "Auto-Generate and Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TableNameTextBox
            // 
            this.TableNameTextBox.Location = new System.Drawing.Point(456, 20);
            this.TableNameTextBox.Name = "TableNameTextBox";
            this.TableNameTextBox.Size = new System.Drawing.Size(527, 24);
            this.TableNameTextBox.TabIndex = 2;
            // 
            // SchemaComboBox2
            // 
            this.SchemaComboBox2.FormattingEnabled = true;
            this.SchemaComboBox2.Location = new System.Drawing.Point(97, 19);
            this.SchemaComboBox2.Name = "SchemaComboBox2";
            this.SchemaComboBox2.Size = new System.Drawing.Size(214, 25);
            this.SchemaComboBox2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(443, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Schema:                                                                          " +
    "Table Name:";
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.closeButton.Location = new System.Drawing.Point(1460, 812);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(106, 27);
            this.closeButton.TabIndex = 8;
            this.closeButton.Text = "Exit";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // Mimir_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1578, 849);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TrainingDataGridView);
            this.Controls.Add(this.SourceDataGroupBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Mimir_Control";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mimir Control";
            this.SourceDataGroupBox.ResumeLayout(false);
            this.SourceDataGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ArticlesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrainingDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SourceDataGroupBox;
        private System.Windows.Forms.Button retrieveButton;
        private System.Windows.Forms.DataGridView ArticlesDataGridView;
        private System.Windows.Forms.ListBox AssetListBox;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.ComboBox SchemaComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton ByArticleRadioButton;
        private System.Windows.Forms.RadioButton BySentenceRadioButton;
        private System.Windows.Forms.DataGridView TrainingDataGridView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox TableNameTextBox;
        private System.Windows.Forms.ComboBox SchemaComboBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button addSchemaButton;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
    }
}