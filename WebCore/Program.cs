using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Text;
using WebCore.Miniblink;

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
            Application.ApplicationExit += Application_ApplicationExit;
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DCLogger.Current.Init();
            Browser.Current.Init();
            WebForm form = new WebForm();
            Screen mainScreen = Screen.PrimaryScreen;
            string localUrl = ConfigurationManager.AppSettings["localUrl"];
            string iconPath = ConfigurationManager.AppSettings["icon"];
            FileInfo finfo = new FileInfo(iconPath);
            finfo.Refresh();
            if (finfo.Exists)
            {
                _icon = new Icon(iconPath);
            }
            var rect = mainScreen.WorkingArea;
            form.Init(localUrl, string.Empty, FormWindowState.Maximized,_icon,
                rect.X, rect.Y, 0,0);
            Application.AddMessageFilter(new WebMessageFilter());
            Application.Run(form);
            
            //Browser.Current.Application_Close();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Browser.Current.Close();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                DCLogger.Current.WriteLog(LoggerLevel.Exception, e.ExceptionObject.ToString());
                Browser.Current.Close();
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception == null)
            {
                return;
            }
            DCLogger.Current.WriteLog(LoggerLevel.Exception, e.Exception.ToString());
        }


    }
}
