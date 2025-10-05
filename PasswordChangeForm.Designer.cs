
namespace DragnetControl
{
    partial class PasswordChangeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordChangeForm));
            this.NewPasswordTextBox = new System.Windows.Forms.TextBox();
            this.NewPasswordTextBox2 = new System.Windows.Forms.TextBox();
            this.oldPasswordLabel = new System.Windows.Forms.Label();
            this.newPassswordLabel = new System.Windows.Forms.Label();
            this.reEnterPasswordLabel = new System.Windows.Forms.Label();
            this.OldPasswordTextBox = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.CloseLabel = new System.Windows.Forms.Button();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.changePasswordLabel = new System.Windows.Forms.Label();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NewPasswordTextBox
            // 
            this.NewPasswordTextBox.Location = new System.Drawing.Point(48, 210);
            this.NewPasswordTextBox.Name = "NewPasswordTextBox";
            this.NewPasswordTextBox.Size = new System.Drawing.Size(266, 29);
            this.NewPasswordTextBox.TabIndex = 2;
            // 
            // NewPasswordTextBox2
            // 
            this.NewPasswordTextBox2.Location = new System.Drawing.Point(48, 291);
            this.NewPasswordTextBox2.Name = "NewPasswordTextBox2";
            this.NewPasswordTextBox2.Size = new System.Drawing.Size(266, 29);
            this.NewPasswordTextBox2.TabIndex = 3;
            // 
            // oldPasswordLabel
            // 
            this.oldPasswordLabel.AutoSize = true;
            this.oldPasswordLabel.Location = new System.Drawing.Point(101, 84);
            this.oldPasswordLabel.Name = "oldPasswordLabel";
            this.oldPasswordLabel.Size = new System.Drawing.Size(146, 22);
            this.oldPasswordLabel.TabIndex = 3;
            this.oldPasswordLabel.Text = "Old Password:";
            // 
            // newPassswordLabel
            // 
            this.newPassswordLabel.AutoSize = true;
            this.newPassswordLabel.Location = new System.Drawing.Point(92, 168);
            this.newPassswordLabel.Name = "newPassswordLabel";
            this.newPassswordLabel.Size = new System.Drawing.Size(155, 22);
            this.newPassswordLabel.TabIndex = 4;
            this.newPassswordLabel.Text = "New Password:";
            // 
            // reEnterPasswordLabel
            // 
            this.reEnterPasswordLabel.AutoSize = true;
            this.reEnterPasswordLabel.Location = new System.Drawing.Point(73, 256);
            this.reEnterPasswordLabel.Name = "reEnterPasswordLabel";
            this.reEnterPasswordLabel.Size = new System.Drawing.Size(204, 22);
            this.reEnterPasswordLabel.TabIndex = 5;
            this.reEnterPasswordLabel.Text = "Re-Enter Password:";
            // 
            // OldPasswordTextBox
            // 
            this.OldPasswordTextBox.Location = new System.Drawing.Point(48, 118);
            this.OldPasswordTextBox.Name = "OldPasswordTextBox";
            this.OldPasswordTextBox.Size = new System.Drawing.Size(266, 29);
            this.OldPasswordTextBox.TabIndex = 1;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(62, 377);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(92, 31);
            this.SubmitButton.TabIndex = 7;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // CloseLabel
            // 
            this.CloseLabel.Location = new System.Drawing.Point(198, 377);
            this.CloseLabel.Name = "CloseLabel";
            this.CloseLabel.Size = new System.Drawing.Size(96, 31);
            this.CloseLabel.TabIndex = 8;
            this.CloseLabel.Text = "Close";
            this.CloseLabel.UseVisualStyleBackColor = true;
            this.CloseLabel.Click += new System.EventHandler(this.CloseLabel_Click);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(73, 48);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(58, 22);
            this.UsernameLabel.TabIndex = 9;
            this.UsernameLabel.Text = "User:";
            // 
            // changePasswordLabel
            // 
            this.changePasswordLabel.AutoSize = true;
            this.changePasswordLabel.Location = new System.Drawing.Point(99, 18);
            this.changePasswordLabel.Name = "changePasswordLabel";
            this.changePasswordLabel.Size = new System.Drawing.Size(181, 22);
            this.changePasswordLabel.TabIndex = 10;
            this.changePasswordLabel.Text = "Change Password";
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessageLabel.Location = new System.Drawing.Point(30, 343);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(134, 17);
            this.MessageLabel.TabIndex = 11;
            this.MessageLabel.Text = "Enter Credentials";
            // 
            // PasswordChangeForm
            // 
            this.ClientSize = new System.Drawing.Size(354, 420);
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.changePasswordLabel);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.CloseLabel);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.OldPasswordTextBox);
            this.Controls.Add(this.reEnterPasswordLabel);
            this.Controls.Add(this.newPassswordLabel);
            this.Controls.Add(this.oldPasswordLabel);
            this.Controls.Add(this.NewPasswordTextBox2);
            this.Controls.Add(this.NewPasswordTextBox);
            this.Font = new System.Drawing.Font("Audiowide", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PasswordChangeForm";
            this.Load += new System.EventHandler(this.PasswordChangeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CurrentPasswordLabel;
        private System.Windows.Forms.TextBox CurrentPasswordTextBox;
        private System.Windows.Forms.Label NewPasswordLabel;
        private System.Windows.Forms.TextBox NewPasswordTextbox1;
        private System.Windows.Forms.TextBox NewPasswordTextBox;
        private System.Windows.Forms.TextBox NewPasswordTextBox2;
        private System.Windows.Forms.Label oldPasswordLabel;
        private System.Windows.Forms.Label newPassswordLabel;
        private System.Windows.Forms.Label reEnterPasswordLabel;
        private System.Windows.Forms.TextBox OldPasswordTextBox;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Button CloseLabel;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Label changePasswordLabel;
        private System.Windows.Forms.Label MessageLabel;
    }
}