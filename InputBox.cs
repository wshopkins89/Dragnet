using Org.BouncyCastle.Asn1.Crmf;
using System.Windows.Forms;

public class InputBox : Form
{
    private TextBox textBox;
    public string InputText { get { return textBox.Text; } }

    public InputBox()
    {
        // Set the title of the dialog
        this.Text = "Add Schema";

        // Set the custom icon
        //this.Icon = new System.Drawing.Icon("icon_path.ico");

        // Set the position to open the dialog in the center of the screen
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create and add TextBox, OK and Cancel buttons to the dialog
        textBox = new TextBox { Location = new System.Drawing.Point(15, 15), Width = 200 };
        var buttonOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(15, 45) };
        var buttonCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(100, 45) };

        // Set the OK button as the default button and the Cancel button as the cancel button
        AcceptButton = buttonOK;
        CancelButton = buttonCancel;

        // Add controls to the dialog
        Controls.Add(textBox);
        Controls.Add(buttonOK);
        Controls.Add(buttonCancel);
    }
}