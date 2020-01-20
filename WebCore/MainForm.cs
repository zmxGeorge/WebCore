using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebCore.Wke;

namespace WebCore
{
    public partial class MainForm : Form
    {
        private WebView _view = new WebView();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _view.ConselMessage += _view_ConselMessage;
            _view.PopMessageBox += _view_PopMessageBox;
            _view.Load(@"test.html");
            _view.Dock = DockStyle.Fill;
            _view.Visible = true;
            Controls.Add(_view);
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
                MessageBox.Show(message,string.Empty, MessageBoxButtons.YesNo);
            }
        }

        private void _view_ConselMessage(WebView view, MessageSource source, MessageType msgType, MessageLevel level, int lineNumber, string url, string message)
        {
            Console.WriteLine(message);
        }
    }
}
