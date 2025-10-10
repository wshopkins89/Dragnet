
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
            LoginButton = new Button();
            UsernameLabel = new Label();
            PasswordLabel = new Label();
            ExitButton = new Button();
            UsernameBox = new TextBox();
            PasswordBox = new TextBox();
            ConnectionStatusStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            AccessRequestLabel = new LinkLabel();
            InformationLabel = new Label();
            RememberUserNameCheckBox = new CheckBox();
            ConfigurationProgressBar = new ProgressBar();
            ConfigurationStatusLabel = new Label();
            ConnectionStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // LoginButton
            // 
            LoginButton.Font = new Font("Audiowide", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoginButton.Location = new Point(185, 269);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(75, 23);
            LoginButton.TabIndex = 3;
            LoginButton.Text = "&Login";
            LoginButton.UseVisualStyleBackColor = true;
            LoginButton.Click += LoginButton_Click;
            // 
            // UsernameLabel
            // 
            UsernameLabel.AutoSize = true;
            UsernameLabel.BackColor = Color.Transparent;
            UsernameLabel.Font = new Font("Audiowide", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UsernameLabel.ForeColor = Color.Cyan;
            UsernameLabel.Location = new Point(12, 159);
            UsernameLabel.Name = "UsernameLabel";
            UsernameLabel.Size = new Size(200, 35);
            UsernameLabel.TabIndex = 1;
            UsernameLabel.Text = "USERNAME:";
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.BackColor = Color.Transparent;
            PasswordLabel.Font = new Font("Audiowide", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PasswordLabel.ForeColor = Color.Cyan;
            PasswordLabel.Location = new Point(9, 206);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(203, 35);
            PasswordLabel.TabIndex = 2;
            PasswordLabel.Text = "PASSWORD:";
            // 
            // ExitButton
            // 
            ExitButton.Font = new Font("Audiowide", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ExitButton.Location = new Point(266, 269);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(75, 23);
            ExitButton.TabIndex = 4;
            ExitButton.Text = "&Exit";
            ExitButton.UseVisualStyleBackColor = true;
            ExitButton.Click += ExitButton_Click;
            // 
            // UsernameBox
            // 
            UsernameBox.Font = new Font("Audiowide", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UsernameBox.Location = new Point(170, 157);
            UsernameBox.Name = "UsernameBox";
            UsernameBox.Size = new Size(213, 41);
            UsernameBox.TabIndex = 1;
            // 
            // PasswordBox
            // 
            PasswordBox.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PasswordBox.Location = new Point(170, 204);
            PasswordBox.Name = "PasswordBox";
            PasswordBox.PasswordChar = '•';
            PasswordBox.Size = new Size(212, 37);
            PasswordBox.TabIndex = 2;
            // 
            // ConnectionStatusStrip
            // 
            ConnectionStatusStrip.BackColor = SystemColors.ActiveCaptionText;
            ConnectionStatusStrip.ImageScalingSize = new Size(20, 20);
            ConnectionStatusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            ConnectionStatusStrip.Location = new Point(0, 369);
            ConnectionStatusStrip.Name = "ConnectionStatusStrip";
            ConnectionStatusStrip.Size = new Size(511, 22);
            ConnectionStatusStrip.TabIndex = 6;
            ConnectionStatusStrip.Text = "statusStrip1";
            ConnectionStatusStrip.ItemClicked += ConnectionStatusStrip_ItemClicked;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("Audiowide", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel1.ForeColor = Color.Cyan;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(131, 16);
            toolStripStatusLabel1.Text = "Connection Status";
            // 
            // AccessRequestLabel
            // 
            AccessRequestLabel.AutoSize = true;
            AccessRequestLabel.BackColor = Color.Transparent;
            AccessRequestLabel.Font = new Font("Audiowide", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AccessRequestLabel.Location = new Point(194, 320);
            AccessRequestLabel.Name = "AccessRequestLabel";
            AccessRequestLabel.Size = new Size(134, 18);
            AccessRequestLabel.TabIndex = 7;
            AccessRequestLabel.TabStop = true;
            AccessRequestLabel.Text = "&Request Access";
            // 
            // InformationLabel
            // 
            InformationLabel.AutoSize = true;
            InformationLabel.BackColor = Color.Transparent;
            InformationLabel.Font = new Font("Audiowide", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            InformationLabel.ForeColor = Color.Cyan;
            InformationLabel.Location = new Point(185, 295);
            InformationLabel.Name = "InformationLabel";
            InformationLabel.Size = new Size(151, 18);
            InformationLabel.TabIndex = 9;
            InformationLabel.Text = "Enter Credentials:";
            // 
            // RememberUserNameCheckBox
            // 
            RememberUserNameCheckBox.AutoSize = true;
            RememberUserNameCheckBox.BackColor = Color.Transparent;
            RememberUserNameCheckBox.Font = new Font("Audiowide", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RememberUserNameCheckBox.ForeColor = Color.Cyan;
            RememberUserNameCheckBox.Location = new Point(170, 242);
            RememberUserNameCheckBox.Name = "RememberUserNameCheckBox";
            RememberUserNameCheckBox.Size = new Size(193, 22);
            RememberUserNameCheckBox.TabIndex = 10;
            RememberUserNameCheckBox.Text = "Remember Username";
            RememberUserNameCheckBox.UseVisualStyleBackColor = false;
            // 
            // ConfigurationProgressBar
            // 
            ConfigurationProgressBar.Location = new Point(34, 348);
            ConfigurationProgressBar.Name = "ConfigurationProgressBar";
            ConfigurationProgressBar.Size = new Size(442, 18);
            ConfigurationProgressBar.Style = ProgressBarStyle.Continuous;
            ConfigurationProgressBar.TabIndex = 11;
            ConfigurationProgressBar.Visible = false;
            // 
            // ConfigurationStatusLabel
            // 
            ConfigurationStatusLabel.AutoSize = true;
            ConfigurationStatusLabel.BackColor = Color.Transparent;
            ConfigurationStatusLabel.ForeColor = Color.Cyan;
            ConfigurationStatusLabel.Location = new Point(34, 291);
            ConfigurationStatusLabel.Name = "ConfigurationStatusLabel";
            ConfigurationStatusLabel.Size = new Size(0, 18);
            ConfigurationStatusLabel.TabIndex = 12;
            ConfigurationStatusLabel.Visible = false;
            // 
            // Authentication
            // 
            AcceptButton = LoginButton;
            BackColor = Color.White;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(511, 391);
            Controls.Add(ConfigurationStatusLabel);
            Controls.Add(ConfigurationProgressBar);
            Controls.Add(RememberUserNameCheckBox);
            Controls.Add(InformationLabel);
            Controls.Add(AccessRequestLabel);
            Controls.Add(ConnectionStatusStrip);
            Controls.Add(PasswordBox);
            Controls.Add(UsernameBox);
            Controls.Add(ExitButton);
            Controls.Add(PasswordLabel);
            Controls.Add(UsernameLabel);
            Controls.Add(LoginButton);
            DoubleBuffered = true;
            Font = new Font("Audiowide", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Authentication";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dragnet Login";
            Load += Authentication_Load;
            ConnectionStatusStrip.ResumeLayout(false);
            ConnectionStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.ProgressBar ConfigurationProgressBar;
        private System.Windows.Forms.Label ConfigurationStatusLabel;
    }
}