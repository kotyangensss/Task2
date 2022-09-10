using System;
using System.IO;
using System.Windows.Forms;

namespace Task2
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1(Directory.GetParent(Environment.CurrentDirectory)?.Parent?.FullName + Path.DirectorySeparatorChar + "settings.xml");
            Application.Run(form);
        }
    }
}