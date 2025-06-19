using System;
using System.Windows.Forms;

namespace MiniTranslator
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Ensure only one instance is running
            bool createdNew;
            using (var mutex = new System.Threading.Mutex(true, "MiniTranslatorApp", out createdNew))
            {
                if (createdNew)
                {
                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("MiniTranslator is already running. Check the system tray.", 
                        "MiniTranslator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
} 