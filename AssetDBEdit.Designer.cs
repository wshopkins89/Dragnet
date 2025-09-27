
namespace DragnetControl
{
    partial class AssetDBEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetDBEdit));
            this.AssetDatabaseLabel = new System.Windows.Forms.Label();
            this.HostLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.DatabaseNameLabel = new System.Windows.Forms.Label();
            this.databasebox = new System.Windows.Forms.TextBox();
            this.PortsLabel = new System.Windows.Forms.Label();
            this.portbox1 = new System.Windows.Forms.TextBox();
            this.tolabel = new System.Windows.Forms.Label();
            this.portbox2 = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SaveandCloseButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.updatelabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AssetDatabaseLabel
            // 
            this.AssetDatabaseLabel.AutoSize = true;
            this.AssetDatabaseLabel.Font = new System.Drawing.Font("Audiowide", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetDatabaseLabel.Location = new System.Drawing.Point(40, 9);
            this.AssetDatabaseLabel.Name = "AssetDatabaseLabel";
            this.AssetDatabaseLabel.Size = new System.Drawing.Size(253, 17);
            this.AssetDatabaseLabel.TabIndex = 0;
            this.AssetDatabaseLabel.Text = "Asset Database Connection Setup";
            // 
            // HostLabel
            // 
            this.HostLabel.AutoSize = true;
            this.HostLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostLabel.Location = new System.Drawing.Point(40, 47);
            this.HostLabel.Name = "HostLabel";
            this.HostLabel.Size = new System.Drawing.Size(40, 14);
            this.HostLabel.TabIndex = 1;
            this.HostLabel.Text = "Host:";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordLabel.Location = new System.Drawing.Point(14, 109);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(72, 14);
            this.PasswordLabel.TabIndex = 3;
            this.PasswordLabel.Text = "Password:";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(98, 106);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(121, 20);
            this.passwordBox.TabIndex = 4;
            // 
            // DatabaseNameLabel
            // 
            this.DatabaseNameLabel.AutoSize = true;
            this.DatabaseNameLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DatabaseNameLabel.Location = new System.Drawing.Point(18, 139);
            this.DatabaseNameLabel.Name = "DatabaseNameLabel";
            this.DatabaseNameLabel.Size = new System.Drawing.Size(68, 14);
            this.DatabaseNameLabel.TabIndex = 5;
            this.DatabaseNameLabel.Text = "Database:";
            // 
            // databasebox
            // 
            this.databasebox.Location = new System.Drawing.Point(98, 136);
            this.databasebox.Name = "databasebox";
            this.databasebox.Size = new System.Drawing.Size(121, 20);
            this.databasebox.TabIndex = 6;
            // 
            // PortsLabel
            // 
            this.PortsLabel.AutoSize = true;
            this.PortsLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortsLabel.Location = new System.Drawing.Point(40, 172);
            this.PortsLabel.Name = "PortsLabel";
            this.PortsLabel.Size = new System.Drawing.Size(46, 14);
            this.PortsLabel.TabIndex = 7;
            this.PortsLabel.Text = "Ports:";
            // 
            // portbox1
            // 
            this.portbox1.Location = new System.Drawing.Point(98, 169);
            this.portbox1.Name = "portbox1";
            this.portbox1.Size = new System.Drawing.Size(46, 20);
            this.portbox1.TabIndex = 8;
            // 
            // tolabel
            // 
            this.tolabel.AutoSize = true;
            this.tolabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tolabel.Location = new System.Drawing.Point(150, 172);
            this.tolabel.Name = "tolabel";
            this.tolabel.Size = new System.Drawing.Size(21, 14);
            this.tolabel.TabIndex = 9;
            this.tolabel.Text = "to";
            // 
            // portbox2
            // 
            this.portbox2.Location = new System.Drawing.Point(177, 169);
            this.portbox2.Name = "portbox2";
            this.portbox2.Size = new System.Drawing.Size(46, 20);
            this.portbox2.TabIndex = 10;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(40, 234);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "&Apply";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // SaveandCloseButton
            // 
            this.SaveandCloseButton.Location = new System.Drawing.Point(121, 234);
            this.SaveandCloseButton.Name = "SaveandCloseButton";
            this.SaveandCloseButton.Size = new System.Drawing.Size(75, 23);
            this.SaveandCloseButton.TabIndex = 12;
            this.SaveandCloseButton.Text = "&Save/Close";
            this.SaveandCloseButton.UseVisualStyleBackColor = true;
            this.SaveandCloseButton.Click += new System.EventHandler(this.SaveandCloseButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(202, 234);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 13;
            this.CancelButton.Text = "&Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLabel.Location = new System.Drawing.Point(12, 78);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(71, 14);
            this.usernameLabel.TabIndex = 14;
            this.usernameLabel.Text = "Username:";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(98, 75);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(121, 20);
            this.usernameTextBox.TabIndex = 15;
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(98, 44);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(121, 20);
            this.hostTextBox.TabIndex = 16;
            // 
            // updatelabel
            // 
            this.updatelabel.AutoSize = true;
            this.updatelabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updatelabel.Location = new System.Drawing.Point(40, 207);
            this.updatelabel.Name = "updatelabel";
            this.updatelabel.Size = new System.Drawing.Size(0, 14);
            this.updatelabel.TabIndex = 17;
            // 
            // AssetDBEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 269);
            this.Controls.Add(this.updatelabel);
            this.Controls.Add(this.hostTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveandCloseButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.portbox2);
            this.Controls.Add(this.tolabel);
            this.Controls.Add(this.portbox1);
            this.Controls.Add(this.PortsLabel);
            this.Controls.Add(this.databasebox);
            this.Controls.Add(this.DatabaseNameLabel);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.HostLabel);
            this.Controls.Add(this.AssetDatabaseLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AssetDBEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Database Setup";
            this.Load += new System.EventHandler(this.AssetDBEdit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AssetDatabaseLabel;
        private System.Windows.Forms.Label HostLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label DatabaseNameLabel;
        private System.Windows.Forms.TextBox databasebox;
        private System.Windows.Forms.Label PortsLabel;
        private System.Windows.Forms.TextBox portbox1;
        private System.Windows.Forms.Label tolabel;
        private System.Windows.Forms.TextBox portbox2;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button SaveandCloseButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.Label updatelabel;
    }
}