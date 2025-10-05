using System;
using System.Windows.Forms;

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

            var configuration = Configuration.DragnetConfiguration.FromAppConfig();
            Application.Run(new Authentication(configuration));
        }
    }
}
