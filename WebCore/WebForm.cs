using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WebCore.Miniblink;

namespace WebCore
{
    public partial class WebForm : Form
    {
        private WebView _view = null;

        private wkeOnNetGetFaviconCallback _getFavicon = null;

        public WebView WebView { get { return _view; } }

        public WebForm()
        {
            InitializeComponent();
            _getFavicon = new wkeOnNetGetFaviconCallback(OnGetFavicon);
        }

        private void OnGetFavicon(IntPtr webView, IntPtr param, IntPtr url, IntPtr buf)
        {
            if (buf == IntPtr.Zero)
            {
                return;
            }
            wkeMemBuf memBuf = (wkeMemBuf)Marshal.PtrToStructure(buf, typeof(wkeMemBuf));
            if (memBuf.length > 0)
            {
                byte[] iconData = new byte[memBuf.length];
                Marshal.Copy(memBuf.data, iconData, 0, iconData.Length);
                using (MemoryStream ms = new MemoryStream(iconData))
                {
                    Icon = new Icon(ms);
                }
            }
        }

        public void Init(string url,string title, FormWindowState windowState,
            Icon icon,
            int x, int y, int width, int height)
        {
            this.Text = title;
            _view = new WebView();
            var uCache = ConfigurationManager.AppSettings["use_cache"].ToLower();
            if (uCache == "false")
            {
                _view.UseCache = false;
            }
            else if(uCache=="true")
            {
                _view.UseCache = true;
            }
            _view.DownLoad += _view_DownLoad;
            _view.TitleChanged += _view_TitleChanged;
            _view.ConselMessage += _view_ConselMessage;
            _view.LoadingComplete += _view_LoadingComplete;
            _view.Dock = DockStyle.Fill;
            _view.Load(url);
            _view.Visible = true;
            if (x >= 0 && y >= 0 && width > 0 && height > 0)
            {
                SetDesktopBounds(x, y, width, height);
            }
            else if (x >= 0 && y >= 0)
            {
                DesktopLocation = new Point(x, y);
            }
            Icon = icon;
            WindowState = windowState;
            Controls.Add(_view);
            BringToFront();

        }

        private void _view_LoadingComplete(WebView view, string url, string reason, UrlLoadResult result)
        {
            if (result == UrlLoadResult.Canceled)
            {
                MessageBox.Show(string.Format("{0} 页面加载被取消", url));
            }
            else if (result == UrlLoadResult.Failed)
            {
                MessageBox.Show(string.Format("{0} 页面加载失败\r\n 原因:{1}", url, reason));
            }
            else
            {
                //获取网站图标并执行修改窗体图标的操作
                MBApi.wkeNetGetFavicon(view.ViewHandle, _getFavicon, IntPtr.Zero);
            }
        }

        private void _view_ConselMessage(WebView view, string message, string sourceName, int lineNumber, string stackTrace)
        {
            StringBuilder sbContent = new StringBuilder();
            sbContent.AppendFormat("Source:{0}\r\n", sourceName);
            sbContent.AppendFormat("LineNumber:{0}\r\n", lineNumber);
            sbContent.AppendFormat("Message:{0}\r\n", message);
            sbContent.AppendFormat("StackTrace:{0}\r\n", stackTrace);
            DCLogger.Current.WriteLog(LoggerLevel.Info, sbContent.ToString());
            Console.WriteLine(sbContent);
        }

        private void _view_DownLoad(WebView view, string url)
        {
            using (DownLoadForm form = new DownLoadForm())
            {
                form.Init(url);
                form.ShowDialog();
            }
        }

        private void _view_TitleChanged(WebView view, string title)
        {
            Invoke(new Action<string>(t =>
            {
                Text = title;
            }), title);
        }
    }
}
