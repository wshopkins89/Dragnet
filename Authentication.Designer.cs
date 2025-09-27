
namespace DragnetControl
{
    partial class Authentication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Authentication));
            this.LoginButton = new System.Windows.Forms.Button();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.ConnectionStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.AccessRequestLabel = new System.Windows.Forms.LinkLabel();
            this.InformationLabel = new System.Windows.Forms.Label();
            this.RememberUserNameCheckBox = new System.Windows.Forms.CheckBox();
            this.ConnectionStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginButton
            // 
            this.LoginButton.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginButton.Location = new System.Drawing.Point(185, 269);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(75, 23);
            this.LoginButton.TabIndex = 3;
            this.LoginButton.Text = "&Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.BackColor = System.Drawing.Color.Transparent;
            this.UsernameLabel.Font = new System.Drawing.Font("Audiowide", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsernameLabel.ForeColor = System.Drawing.Color.Cyan;
            this.UsernameLabel.Location = new System.Drawing.Point(12, 159);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(156, 27);
            this.UsernameLabel.TabIndex = 1;
            this.UsernameLabel.Text = "USERNAME:";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.BackColor = System.Drawing.Color.Transparent;
            this.PasswordLabel.Font = new System.Drawing.Font("Audiowide", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordLabel.ForeColor = System.Drawing.Color.Cyan;
            this.PasswordLabel.Location = new System.Drawing.Point(9, 206);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(157, 27);
            this.PasswordLabel.TabIndex = 2;
            this.PasswordLabel.Text = "PASSWORD:";
            // 
            // ExitButton
            // 
            this.ExitButton.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.Location = new System.Drawing.Point(266, 269);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 4;
            this.ExitButton.Text = "&Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // UsernameBox
            // 
            this.UsernameBox.Font = new System.Drawing.Font("Audiowide", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsernameBox.Location = new System.Drawing.Point(170, 157);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(213, 34);
            this.UsernameBox.TabIndex = 1;
            // 
            // PasswordBox
            // 
            this.PasswordBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordBox.Location = new System.Drawing.Point(170, 204);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.PasswordChar = '•';
            this.PasswordBox.Size = new System.Drawing.Size(212, 31);
            this.PasswordBox.TabIndex = 2;
            // 
            // ConnectionStatusStrip
            // 
            this.ConnectionStatusStrip.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ConnectionStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.ConnectionStatusStrip.Location = new System.Drawing.Point(0, 338);
            this.ConnectionStatusStrip.Name = "ConnectionStatusStrip";
            this.ConnectionStatusStrip.Size = new System.Drawing.Size(511, 22);
            this.ConnectionStatusStrip.TabIndex = 6;
            this.ConnectionStatusStrip.Text = "statusStrip1";
            this.ConnectionStatusStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ConnectionStatusStrip_ItemClicked);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Audiowide", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Cyan;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(98, 17);
            this.toolStripStatusLabel1.Text = "Connection Status";
            // 
            // AccessRequestLabel
            // 
            this.AccessRequestLabel.AutoSize = true;
            this.AccessRequestLabel.BackColor = System.Drawing.Color.Transparent;
            this.AccessRequestLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AccessRequestLabel.Location = new System.Drawing.Point(194, 320);
            this.AccessRequestLabel.Name = "AccessRequestLabel";
            this.AccessRequestLabel.Size = new System.Drawing.Size(107, 14);
            this.AccessRequestLabel.TabIndex = 7;
            this.AccessRequestLabel.TabStop = true;
            this.AccessRequestLabel.Text = "&Request Access";
            // 
            // InformationLabel
            // 
            this.InformationLabel.AutoSize = true;
            this.InformationLabel.BackColor = System.Drawing.Color.Transparent;
            this.InformationLabel.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InformationLabel.ForeColor = System.Drawing.Color.Cyan;
            this.InformationLabel.Location = new System.Drawing.Point(182, 295);
            this.InformationLabel.Name = "InformationLabel";
            this.InformationLabel.Size = new System.Drawing.Size(119, 14);
            this.InformationLabel.TabIndex = 9;
            this.InformationLabel.Text = "Enter Credentials:";
            // 
            // RememberUserNameCheckBox
            // 
            this.RememberUserNameCheckBox.AutoSize = true;
            this.RememberUserNameCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.RememberUserNameCheckBox.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RememberUserNameCheckBox.ForeColor = System.Drawing.Color.Cyan;
            this.RememberUserNameCheckBox.Location = new System.Drawing.Point(170, 242);
            this.RememberUserNameCheckBox.Name = "RememberUserNameCheckBox";
            this.RememberUserNameCheckBox.Size = new System.Drawing.Size(155, 18);
            this.RememberUserNameCheckBox.TabIndex = 10;
            this.RememberUserNameCheckBox.Text = "Remember Username";
            this.RememberUserNameCheckBox.UseVisualStyleBackColor = false;
            // 
            // Authentication
            // 
            this.AcceptButton = this.LoginButton;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(511, 360);
            this.Controls.Add(this.RememberUserNameCheckBox);
            this.Controls.Add(this.InformationLabel);
            this.Controls.Add(this.AccessRequestLabel);
            this.Controls.Add(this.ConnectionStatusStrip);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.UsernameBox);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.LoginButton);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Audiowide", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Authentication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dragnet Login";
            this.Load += new System.EventHandler(this.Authentication_Load);
            this.ConnectionStatusStrip.ResumeLayout(false);
            this.ConnectionStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.StatusStrip ConnectionStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        protected System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.LinkLabel AccessRequestLabel;
        private System.Windows.Forms.Label InformationLabel;
        private System.Windows.Forms.CheckBox RememberUserNameCheckBox;
    }
}