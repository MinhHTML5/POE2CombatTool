using System;
using System.Windows.Forms;
using POE2RuleTool.Forms;

namespace POE2RuleTool;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
