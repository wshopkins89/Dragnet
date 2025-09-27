
namespace DragnetControl
{
    public partial class AssetManagement : Form

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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.InternalCatalogLabel = new System.Windows.Forms.Label();
            this.AssetTypeLabel = new System.Windows.Forms.Label();
            this.SearchButton = new System.Windows.Forms.Button();
            this.ModifyAddButton = new System.Windows.Forms.Button();
            this.AssetTypeComboBox = new System.Windows.Forms.ComboBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.IntRefNo = new System.Windows.Forms.Label();
            this.ReferenceNumberLabel = new System.Windows.Forms.Label();
            this.ScannerAPIKeyLabel = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(324, 24);
            this.textBox1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(195, 33);
            this.textBox1.TabIndex = 0;
            // 
            // InternalCatalogLabel
            // 
            this.InternalCatalogLabel.AutoSize = true;
            this.InternalCatalogLabel.Location = new System.Drawing.Point(28, 29);
            this.InternalCatalogLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.InternalCatalogLabel.Name = "InternalCatalogLabel";
            this.InternalCatalogLabel.Size = new System.Drawing.Size(285, 26);
            this.InternalCatalogLabel.TabIndex = 1;
            this.InternalCatalogLabel.Text = "Internal Catalog Symbol:";
            this.InternalCatalogLabel.Click += new System.EventHandler(this.InternalCatalogLabel_Click);
            // 
            // AssetTypeLabel
            // 
            this.AssetTypeLabel.AutoSize = true;
            this.AssetTypeLabel.Location = new System.Drawing.Point(93, 81);
            this.AssetTypeLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.AssetTypeLabel.Name = "AssetTypeLabel";
            this.AssetTypeLabel.Size = new System.Drawing.Size(145, 26);
            this.AssetTypeLabel.TabIndex = 2;
            this.AssetTypeLabel.Text = "Asset Type:";
            this.AssetTypeLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(114, 876);
            this.SearchButton.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(175, 46);
            this.SearchButton.TabIndex = 3;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            // 
            // ModifyAddButton
            // 
            this.ModifyAddButton.Location = new System.Drawing.Point(303, 876);
            this.ModifyAddButton.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ModifyAddButton.Name = "ModifyAddButton";
            this.ModifyAddButton.Size = new System.Drawing.Size(175, 46);
            this.ModifyAddButton.TabIndex = 4;
            this.ModifyAddButton.Text = "Modify/Add";
            this.ModifyAddButton.UseVisualStyleBackColor = true;
            // 
            // AssetTypeComboBox
            // 
            this.AssetTypeComboBox.FormattingEnabled = true;
            this.AssetTypeComboBox.Items.AddRange(new object[] {
            "Stock",
            "Crypto",
            "Fiat Currency"});
            this.AssetTypeComboBox.Location = new System.Drawing.Point(254, 76);
            this.AssetTypeComboBox.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.AssetTypeComboBox.Name = "AssetTypeComboBox";
            this.AssetTypeComboBox.Size = new System.Drawing.Size(277, 34);
            this.AssetTypeComboBox.TabIndex = 5;
            this.AssetTypeComboBox.Text = "(Select One)";
            this.AssetTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.AssetTypeComboBox_SelectedIndexChanged);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(492, 876);
            this.ClearButton.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(175, 46);
            this.ClearButton.TabIndex = 6;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(681, 876);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(175, 46);
            this.CloseButton.TabIndex = 7;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // IntRefNo
            // 
            this.IntRefNo.AutoSize = true;
            this.IntRefNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IntRefNo.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.IntRefNo.Location = new System.Drawing.Point(616, 50);
            this.IntRefNo.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.IntRefNo.Name = "IntRefNo";
            this.IntRefNo.Size = new System.Drawing.Size(46, 25);
            this.IntRefNo.TabIndex = 11;
            this.IntRefNo.Text = "N/A";
            // 
            // ReferenceNumberLabel
            // 
            this.ReferenceNumberLabel.AutoSize = true;
            this.ReferenceNumberLabel.Location = new System.Drawing.Point(618, 24);
            this.ReferenceNumberLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.ReferenceNumberLabel.Name = "ReferenceNumberLabel";
            this.ReferenceNumberLabel.Size = new System.Drawing.Size(322, 26);
            this.ReferenceNumberLabel.TabIndex = 10;
            this.ReferenceNumberLabel.Text = "Internal Reference Number:";
            // 
            // ScannerAPIKeyLabel
            // 
            this.ScannerAPIKeyLabel.AutoSize = true;
            this.ScannerAPIKeyLabel.Location = new System.Drawing.Point(28, 136);
            this.ScannerAPIKeyLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.ScannerAPIKeyLabel.Name = "ScannerAPIKeyLabel";
            this.ScannerAPIKeyLabel.Size = new System.Drawing.Size(206, 26);
            this.ScannerAPIKeyLabel.TabIndex = 8;
            this.ScannerAPIKeyLabel.Text = "Scanner API Key:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(254, 130);
            this.textBox2.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(734, 33);
            this.textBox2.TabIndex = 9;
            // 
            // AssetManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 926);
            this.Controls.Add(this.IntRefNo);
            this.Controls.Add(this.ReferenceNumberLabel);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ScannerAPIKeyLabel);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.AssetTypeComboBox);
            this.Controls.Add(this.ModifyAddButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.AssetTypeLabel);
            this.Controls.Add(this.InternalCatalogLabel);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.MaximumSize = new System.Drawing.Size(1056, 973);
            this.MinimumSize = new System.Drawing.Size(1056, 973);
            this.Name = "AssetManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Management";
            this.Load += new System.EventHandler(this.AssetManagement_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label InternalCatalogLabel;
        private System.Windows.Forms.Label AssetTypeLabel;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button ModifyAddButton;
        private System.Windows.Forms.ComboBox AssetTypeComboBox;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label IntRefNo;
        private System.Windows.Forms.Label ReferenceNumberLabel;
        private System.Windows.Forms.Label ScannerAPIKeyLabel;
        private System.Windows.Forms.TextBox textBox2;
    }
}