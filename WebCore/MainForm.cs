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
            _view.Load(@"https://www.baidu.com");
            _view.Dock = DockStyle.Fill;
            _view.Visible = true;
            Controls.Add(_view);
        }
    }
}
