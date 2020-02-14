using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WebCore.Wke;
using System.Configuration;
using System.IO;

namespace WebCore
{
    static class Program
    {
        private static Icon _icon = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DCLogger.Current.Init();
            Browser.DownLoad += Browser_DownLoad;
            Browser.OpenNewWindow += Browser_OpenNewWindow;
            Browser.Current.Application_Init();
            WebForm form = new WebForm();
            Screen mainScreen = Screen.PrimaryScreen;
            string localUrl = ConfigurationManager.AppSettings["localUrl"];
            string localTitle = ConfigurationManager.AppSettings["localTitle"];
            string iconPath = ConfigurationManager.AppSettings["icon"];
            FileInfo finfo = new FileInfo(iconPath);
            finfo.Refresh();
            if (finfo.Exists)
            {
                _icon = new Icon(iconPath);
            }
            var rect = mainScreen.Bounds;
            form.Init(localUrl, localTitle, FormWindowState.Maximized,_icon,
                rect.X, rect.Y, rect.Width, rect.Height);
            Application.Run(form);
            Browser.Current.Application_Close();
        }

        private static void Browser_OpenNewWindow(string url, string title, int x, int y, int width, int height)
        {
            WebForm form = new WebForm();
            form.Init(url,title,FormWindowState.Normal, _icon, x, y, width, height);
            form.Show();
        }

        private static void Browser_DownLoad(string url)
        {
            
        }
    }
}
