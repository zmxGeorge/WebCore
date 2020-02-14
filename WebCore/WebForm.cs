using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebCore.Wke;

namespace WebCore
{
    public partial class WebForm : Form
    {
        private WebView _view = null;

        public WebView WebView { get { return _view; } }

        public WebForm()
        {
            InitializeComponent();
            
        }

        public void Init(string url,string title, FormWindowState windowState,
            Icon icon,
            int x, int y, int width, int height)
        {
            this.Text = title;
            _view = new WebView();
            _view.ConselMessage += _view_ConselMessage;
            _view.PopMessageBox += _view_PopMessageBox;
            _view.Dock = DockStyle.Fill;
            _view.Load(url);
            _view.Visible = true;
            if (x >= 0 && y >= 0 && width > 0 && height > 0)
            {
                SetDesktopBounds(x, y, width, height);
            }
            Icon = icon;
            WindowState = windowState;
            Controls.Add(_view);
            BringToFront();

        }

        private void _view_PopMessageBox(WebView view, PopMessageBoxType popMessageType, string message)
        {
            if (popMessageType == PopMessageBoxType.Alert ||
                 popMessageType == PopMessageBoxType.Prompt)
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show(message, string.Empty, MessageBoxButtons.YesNo);
            }
        }

        private void _view_ConselMessage(WebView view, MessageSource source, MessageType msgType, MessageLevel level, int lineNumber, string url, string message)
        {
            StringBuilder sbContent = new StringBuilder();
            sbContent.AppendFormat("Source:{0}\r\n",source.ToString());
            sbContent.AppendFormat("Type:{0}\r\n", msgType.ToString());
            sbContent.AppendFormat("Level:{0}\r\n", level.ToString());
            sbContent.AppendFormat("LineNumber:{0}\r\n", lineNumber);
            sbContent.AppendFormat("URL:{0}\r\n", url);
            sbContent.AppendFormat("Message:{0}\r\n", message);
            DCLogger.Current.WriteLog(LoggerLevel.Info, sbContent.ToString());
            Console.WriteLine(sbContent);
        }
    }
}
