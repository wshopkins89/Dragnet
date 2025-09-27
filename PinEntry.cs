using DragnetControl;
using System;
using System.Windows.Forms;

namespace PinEntryControl
{
    public partial class PinEntry : Form
    {
        string CorrectPin = GlobalVariables.pin;

        public PinEntry()
        {
            InitializeComponent();

            // Add event handlers for the TextBox controls
            TextBox1.KeyDown += TextBox_KeyDown;
            TextBox2.KeyDown += TextBox_KeyDown;
            TextBox3.KeyDown += TextBox_KeyDown;
            TextBox4.KeyDown += TextBox_KeyDown;

            TextBox1.TextChanged += TextBox_TextChanged;
            TextBox2.TextChanged += TextBox_TextChanged;
            TextBox3.TextChanged += TextBox_TextChanged;
            TextBox4.TextChanged += TextBox_TextChanged;

            // Set up the Tag property to control the focus flow
            TextBox1.Tag = TextBox2;
            TextBox2.Tag = TextBox3;
            TextBox3.Tag = TextBox4;
            TextBox4.Tag = submitButton;

            submitButton.Click += SubmitButton_Click;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;

            // Check if backspace was pressed in an empty TextBox
            if (e.KeyCode == Keys.Back && string.IsNullOrEmpty(currentTextBox.Text))
            {
                MoveFocusToPreviousTextBox(currentTextBox);
                e.SuppressKeyPress = true; // Prevent further processing
            }
            else if (!(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 || e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 || e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = true; // Suppress non-numeric and non-backspace keys
            }
        }

        private void MoveFocusToPreviousTextBox(TextBox currentTextBox)
        {
            // Determine the previous TextBox based on the current TextBox's name
            switch (currentTextBox.Name)
            {
                case "TextBox2":
                    TextBox1.Focus();
                    TextBox1.Text = "";
                    break;
                case "TextBox3":
                    TextBox2.Focus();
                    TextBox2.Text = "";
                    break;
                case "TextBox4":
                    TextBox3.Focus();
                    TextBox3.Text = "";
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            if (currentTextBox != null && currentTextBox.Text.Length == 1 && currentTextBox.Tag is System.Windows.Forms.Control nextControl)
            {
                nextControl.Focus();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string enteredPin = $"{TextBox1.Text}{TextBox2.Text}{TextBox3.Text}{TextBox4.Text}";

            if (enteredPin == CorrectPin)
            {
                // If the PIN is correct, you can close this form and let the calling form know the PIN was correct
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // If the PIN is incorrect, update the label to indicate an invalid PIN
                statusLabel.Text = "Invalid PIN";
                statusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            this.Close();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            TextBox1.Text = "";
            TextBox2.Text = "";
            TextBox3.Text = "";
            TextBox4.Text = "";
        }
    }
}
