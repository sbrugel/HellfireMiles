using System;
using System.IO;
using System.Windows.Forms;

namespace HellfireMiles
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
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HellfireMiles\\dir.txt"))
            {
                Application.Run(new DataLocation());
            } else Application.Run(new JourneyView());
        }
    }
}
