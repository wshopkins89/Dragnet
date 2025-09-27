namespace DragnetControl
{
    partial class AddNode
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
            ipTextBox = new TextBox();
            nodeIPLabel = new Label();
            usernameTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            passwordTextBox = new TextBox();
            portTextBox = new TextBox();
            label3 = new Label();
            enabledCheckBox = new CheckBox();
            cpuScoreComboBox = new ComboBox();
            label4 = new Label();
            label5 = new Label();
            RAMScoreComboBox = new ComboBox();
            noteTextBox = new TextBox();
            label6 = new Label();
            label7 = new Label();
            submitButton = new Button();
            ClearButton = new Button();
            exitButton = new Button();
            SuspendLayout();
            // 
            // ipTextBox
            // 
            ipTextBox.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ipTextBox.Location = new Point(160, 59);
            ipTextBox.Name = "ipTextBox";
            ipTextBox.Size = new Size(296, 23);
            ipTextBox.TabIndex = 0;
            // 
            // nodeIPLabel
            // 
            nodeIPLabel.AutoSize = true;
            nodeIPLabel.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            nodeIPLabel.Location = new Point(34, 64);
            nodeIPLabel.Name = "nodeIPLabel";
            nodeIPLabel.Size = new Size(120, 15);
            nodeIPLabel.TabIndex = 1;
            nodeIPLabel.Text = "Node IP Address:";
            // 
            // usernameTextBox
            // 
            usernameTextBox.Font = new Font("Audiowide", 9F);
            usernameTextBox.Location = new Point(160, 88);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(148, 23);
            usernameTextBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(40, 91);
            label1.Name = "label1";
            label1.Size = new Size(114, 15);
            label1.TabIndex = 3;
            label1.Text = "Node Username:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(40, 124);
            label2.Name = "label2";
            label2.Size = new Size(114, 15);
            label2.TabIndex = 4;
            label2.Text = "Node Password:";
            // 
            // passwordTextBox
            // 
            passwordTextBox.Font = new Font("Audiowide", 9F);
            passwordTextBox.Location = new Point(160, 121);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(148, 23);
            passwordTextBox.TabIndex = 5;
            // 
            // portTextBox
            // 
            portTextBox.Font = new Font("Audiowide", 9F);
            portTextBox.Location = new Point(160, 150);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(63, 23);
            portTextBox.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(74, 153);
            label3.Name = "label3";
            label3.Size = new Size(80, 15);
            label3.TabIndex = 7;
            label3.Text = "Node Port:";
            // 
            // enabledCheckBox
            // 
            enabledCheckBox.AutoSize = true;
            enabledCheckBox.Checked = true;
            enabledCheckBox.CheckState = CheckState.Checked;
            enabledCheckBox.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            enabledCheckBox.Location = new Point(249, 152);
            enabledCheckBox.Name = "enabledCheckBox";
            enabledCheckBox.Size = new Size(117, 19);
            enabledCheckBox.TabIndex = 8;
            enabledCheckBox.Text = "Node Enabled";
            enabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // cpuScoreComboBox
            // 
            cpuScoreComboBox.AutoCompleteCustomSource.AddRange(new string[] { "1", "2", "3" });
            cpuScoreComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cpuScoreComboBox.Font = new Font("Audiowide", 9F);
            cpuScoreComboBox.FormattingEnabled = true;
            cpuScoreComboBox.Items.AddRange(new object[] { "1", "2", "3" });
            cpuScoreComboBox.Location = new Point(160, 179);
            cpuScoreComboBox.Name = "cpuScoreComboBox";
            cpuScoreComboBox.Size = new Size(47, 23);
            cpuScoreComboBox.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(72, 182);
            label4.Name = "label4";
            label4.Size = new Size(82, 15);
            label4.TabIndex = 10;
            label4.Text = "CPU Score:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(228, 182);
            label5.Name = "label5";
            label5.Size = new Size(85, 15);
            label5.TabIndex = 11;
            label5.Text = "RAM Score:";
            // 
            // RAMScoreComboBox
            // 
            RAMScoreComboBox.AutoCompleteCustomSource.AddRange(new string[] { "1", "2", "3" });
            RAMScoreComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RAMScoreComboBox.Font = new Font("Audiowide", 9F);
            RAMScoreComboBox.FormattingEnabled = true;
            RAMScoreComboBox.Items.AddRange(new object[] { "1", "2", "3" });
            RAMScoreComboBox.Location = new Point(319, 179);
            RAMScoreComboBox.Name = "RAMScoreComboBox";
            RAMScoreComboBox.Size = new Size(47, 23);
            RAMScoreComboBox.TabIndex = 12;
            // 
            // noteTextBox
            // 
            noteTextBox.Font = new Font("Audiowide", 9F);
            noteTextBox.Location = new Point(160, 208);
            noteTextBox.Name = "noteTextBox";
            noteTextBox.Size = new Size(296, 23);
            noteTextBox.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Audiowide", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(111, 211);
            label6.Name = "label6";
            label6.Size = new Size(43, 15);
            label6.TabIndex = 14;
            label6.Text = "Note:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Audiowide", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.Location = new Point(160, 19);
            label7.Name = "label7";
            label7.Size = new Size(163, 24);
            label7.TabIndex = 15;
            label7.Text = "Add New Node";
            // 
            // submitButton
            // 
            submitButton.Font = new Font("Audiowide", 9F);
            submitButton.Location = new Point(132, 264);
            submitButton.Name = "submitButton";
            submitButton.Size = new Size(75, 23);
            submitButton.TabIndex = 16;
            submitButton.Text = "Submit";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += submitButton_Click;
            // 
            // ClearButton
            // 
            ClearButton.Font = new Font("Audiowide", 9F);
            ClearButton.Location = new Point(213, 264);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(75, 23);
            ClearButton.TabIndex = 17;
            ClearButton.Text = "Clear";
            ClearButton.UseVisualStyleBackColor = true;
            ClearButton.Click += clearButton_Click;
            // 
            // exitButton
            // 
            exitButton.Font = new Font("Audiowide", 9F);
            exitButton.Location = new Point(294, 264);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(75, 23);
            exitButton.TabIndex = 18;
            exitButton.Text = "Exit";
            exitButton.UseVisualStyleBackColor = true;
            exitButton.Click += exitButton_Click;
            // 
            // AddNode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 311);
            Controls.Add(exitButton);
            Controls.Add(ClearButton);
            Controls.Add(submitButton);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(noteTextBox);
            Controls.Add(RAMScoreComboBox);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(cpuScoreComboBox);
            Controls.Add(enabledCheckBox);
            Controls.Add(label3);
            Controls.Add(portTextBox);
            Controls.Add(passwordTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(usernameTextBox);
            Controls.Add(nodeIPLabel);
            Controls.Add(ipTextBox);
            Name = "AddNode";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add New Node";
            Load += AddNode_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox ipTextBox;
        private Label nodeIPLabel;
        private TextBox usernameTextBox;
        private Label label1;
        private Label label2;
        private TextBox passwordTextBox;
        private TextBox portTextBox;
        private Label label3;
        private CheckBox enabledCheckBox;
        private ComboBox cpuScoreComboBox;
        private Label label4;
        private Label label5;
        private ComboBox RAMScoreComboBox;
        private TextBox noteTextBox;
        private Label label6;
        private Label label7;
        private Button submitButton;
        private Button ClearButton;
        private Button exitButton;
    }
}