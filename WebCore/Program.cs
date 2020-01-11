using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebCore.Wke;

namespace WebCore
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Browser.Current.Application_Init();
            Application.Run(new MainForm());
            Browser.Current.Application_Close();
        }
    }
}
