namespace DragnetControl
{
    partial class ManuallyBuildDatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManuallyBuildDatabase));
            this.exchangeassetlabel = new System.Windows.Forms.Label();
            this.exchangeComboBox = new System.Windows.Forms.ComboBox();
            this.assetComboBox = new System.Windows.Forms.ComboBox();
            this.intervalLabel = new System.Windows.Forms.Label();
            this.intervalComboBox = new System.Windows.Forms.ComboBox();
            this.dateLabel = new System.Windows.Forms.Label();
            this.StartDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.endDateLabel = new System.Windows.Forms.Label();
            this.endTimeDatePicker = new System.Windows.Forms.DateTimePicker();
            this.BuildButton = new System.Windows.Forms.Button();
            this.RecalculateButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.StartTimeTextBox = new System.Windows.Forms.MaskedTextBox();
            this.EndTimeTextBox = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // exchangeassetlabel
            // 
            this.exchangeassetlabel.AutoSize = true;
            this.exchangeassetlabel.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exchangeassetlabel.Location = new System.Drawing.Point(32, 22);
            this.exchangeassetlabel.Name = "exchangeassetlabel";
            this.exchangeassetlabel.Size = new System.Drawing.Size(481, 21);
            this.exchangeassetlabel.TabIndex = 0;
            this.exchangeassetlabel.Text = "&Exchange:                                         Asset:                        " +
    "";
            // 
            // exchangeComboBox
            // 
            this.exchangeComboBox.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exchangeComboBox.FormattingEnabled = true;
            this.exchangeComboBox.Items.AddRange(new object[] {
            "Coinbase",
            "Kraken",
            "Binance.Us"});
            this.exchangeComboBox.Location = new System.Drawing.Point(174, 18);
            this.exchangeComboBox.Name = "exchangeComboBox";
            this.exchangeComboBox.Size = new System.Drawing.Size(214, 29);
            this.exchangeComboBox.TabIndex = 1;
            // 
            // assetComboBox
            // 
            this.assetComboBox.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.assetComboBox.FormattingEnabled = true;
            this.assetComboBox.Items.AddRange(new object[] {
            "All Assets"});
            this.assetComboBox.Location = new System.Drawing.Point(514, 19);
            this.assetComboBox.Name = "assetComboBox";
            this.assetComboBox.Size = new System.Drawing.Size(229, 29);
            this.assetComboBox.TabIndex = 2;
            // 
            // intervalLabel
            // 
            this.intervalLabel.AutoSize = true;
            this.intervalLabel.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intervalLabel.Location = new System.Drawing.Point(32, 85);
            this.intervalLabel.Name = "intervalLabel";
            this.intervalLabel.Size = new System.Drawing.Size(83, 21);
            this.intervalLabel.TabIndex = 3;
            this.intervalLabel.Text = "Interval:";
            // 
            // intervalComboBox
            // 
            this.intervalComboBox.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intervalComboBox.FormattingEnabled = true;
            this.intervalComboBox.Items.AddRange(new object[] {
            "Working Database",
            "One Minute",
            "Five Minutes",
            "Fifteen Minutes",
            "One Hour",
            "Six Hours",
            "One Day"});
            this.intervalComboBox.Location = new System.Drawing.Point(174, 82);
            this.intervalComboBox.Name = "intervalComboBox";
            this.intervalComboBox.Size = new System.Drawing.Size(214, 29);
            this.intervalComboBox.TabIndex = 4;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLabel.Location = new System.Drawing.Point(32, 140);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(444, 21);
            this.dateLabel.TabIndex = 5;
            this.dateLabel.Text = "Start Date:                                                          Time:";
            // 
            // StartDateTimePicker
            // 
            this.StartDateTimePicker.CalendarFont = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartDateTimePicker.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartDateTimePicker.Location = new System.Drawing.Point(165, 135);
            this.StartDateTimePicker.MinDate = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            this.StartDateTimePicker.Name = "StartDateTimePicker";
            this.StartDateTimePicker.Size = new System.Drawing.Size(394, 28);
            this.StartDateTimePicker.TabIndex = 7;
            this.StartDateTimePicker.ValueChanged += new System.EventHandler(this.StartDateTimePicker_ValueChanged);
            // 
            // endDateLabel
            // 
            this.endDateLabel.AutoSize = true;
            this.endDateLabel.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endDateLabel.Location = new System.Drawing.Point(32, 203);
            this.endDateLabel.Name = "endDateLabel";
            this.endDateLabel.Size = new System.Drawing.Size(444, 21);
            this.endDateLabel.TabIndex = 8;
            this.endDateLabel.Text = "End Date:                                                             Time:";
            // 
            // endTimeDatePicker
            // 
            this.endTimeDatePicker.CalendarFont = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endTimeDatePicker.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endTimeDatePicker.Location = new System.Drawing.Point(165, 198);
            this.endTimeDatePicker.MinDate = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            this.endTimeDatePicker.Name = "endTimeDatePicker";
            this.endTimeDatePicker.Size = new System.Drawing.Size(394, 28);
            this.endTimeDatePicker.TabIndex = 9;
            this.endTimeDatePicker.Value = new System.DateTime(2023, 11, 27, 16, 5, 20, 0);
            // 
            // BuildButton
            // 
            this.BuildButton.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuildButton.Location = new System.Drawing.Point(37, 302);
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(156, 32);
            this.BuildButton.TabIndex = 10;
            this.BuildButton.Text = "&Build";
            this.BuildButton.UseVisualStyleBackColor = true;
            this.BuildButton.Click += new System.EventHandler(this.BuildButton_Click);
            // 
            // RecalculateButton
            // 
            this.RecalculateButton.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecalculateButton.Location = new System.Drawing.Point(199, 302);
            this.RecalculateButton.Name = "RecalculateButton";
            this.RecalculateButton.Size = new System.Drawing.Size(176, 32);
            this.RecalculateButton.TabIndex = 11;
            this.RecalculateButton.Text = "&Recalculate";
            this.RecalculateButton.UseVisualStyleBackColor = true;
            this.RecalculateButton.Click += new System.EventHandler(this.RecalculateButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(655, 302);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(88, 32);
            this.exitButton.TabIndex = 12;
            this.exitButton.Text = "&Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // StartTimeTextBox
            // 
            this.StartTimeTextBox.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartTimeTextBox.Location = new System.Drawing.Point(637, 137);
            this.StartTimeTextBox.Mask = "90:00";
            this.StartTimeTextBox.Name = "StartTimeTextBox";
            this.StartTimeTextBox.Size = new System.Drawing.Size(100, 28);
            this.StartTimeTextBox.TabIndex = 14;
            this.StartTimeTextBox.Text = "0000";
            this.StartTimeTextBox.ValidatingType = typeof(System.DateTime);
            // 
            // EndTimeTextBox
            // 
            this.EndTimeTextBox.Font = new System.Drawing.Font("Audiowide", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndTimeTextBox.Location = new System.Drawing.Point(637, 200);
            this.EndTimeTextBox.Mask = "90:00";
            this.EndTimeTextBox.Name = "EndTimeTextBox";
            this.EndTimeTextBox.Size = new System.Drawing.Size(100, 28);
            this.EndTimeTextBox.TabIndex = 15;
            this.EndTimeTextBox.ValidatingType = typeof(System.DateTime);
            // 
            // ManuallyBuildDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 344);
            this.Controls.Add(this.EndTimeTextBox);
            this.Controls.Add(this.StartTimeTextBox);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.RecalculateButton);
            this.Controls.Add(this.BuildButton);
            this.Controls.Add(this.endTimeDatePicker);
            this.Controls.Add(this.endDateLabel);
            this.Controls.Add(this.StartDateTimePicker);
            this.Controls.Add(this.dateLabel);
            this.Controls.Add(this.intervalComboBox);
            this.Controls.Add(this.intervalLabel);
            this.Controls.Add(this.assetComboBox);
            this.Controls.Add(this.exchangeComboBox);
            this.Controls.Add(this.exchangeassetlabel);
            this.Font = new System.Drawing.Font("Audiowide", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ManuallyBuildDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Build Database";
            this.Load += new System.EventHandler(this.ManuallyBuildDatabase_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label exchangeassetlabel;
        private System.Windows.Forms.ComboBox exchangeComboBox;
        private System.Windows.Forms.ComboBox assetComboBox;
        private System.Windows.Forms.Label intervalLabel;
        private System.Windows.Forms.ComboBox intervalComboBox;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.DateTimePicker StartDateTimePicker;
        private System.Windows.Forms.Label endDateLabel;
        private System.Windows.Forms.DateTimePicker endTimeDatePicker;
        private System.Windows.Forms.Button BuildButton;
        private System.Windows.Forms.Button RecalculateButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.MaskedTextBox StartTimeTextBox;
        private System.Windows.Forms.MaskedTextBox EndTimeTextBox;
    }
}