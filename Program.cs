using System;
using System.Windows.Forms;
using System.Configuration;
using DragnetControl.Infrastructure.Configuration;

namespace DragnetControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                _ = DatabaseSettings.Current;
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show(
                    $"Unable to load database configuration.\n\n{ex.Message}",
                    "Configuration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Authentication());
        }
    }
}
