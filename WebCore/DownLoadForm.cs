using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace WebCore
{
    public class Tuple<T, T1>
    {
        public Tuple(T t1, T1 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }

        public T Item1 { get; set; }

        public T1 Item2 { get; set; }
    }

    public partial class DownLoadForm : Form
    {

        private string _downLoadUrl = null;

        private long _contentLength = 0;

        public DownLoadForm()
        {
            InitializeComponent();
        }

        private void btnFloder_Click(object sender, EventArgs e)
        {
            var url = new Uri(_downLoadUrl);
            string fName = Path.GetFileName(url.AbsolutePath);
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "选择文件保存的路径";
                dialog.FileName = fName;
                dialog.DefaultExt = Path.GetExtension(fName);
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    txtSaveDir.Text = dialog.FileName;
                }
            }
        }

        private void btnOpenFloder_Click(object sender, EventArgs e)
        {
            string fileName=txtSaveDir.Text;
            var fInfo = new FileInfo(fileName);
            fInfo.Refresh();
            if (fInfo.Directory.Exists)
            {
                Process.Start(fInfo.Directory.FullName);
            }
        }

        private void btnDownLoad_Click(object sender, EventArgs e)
        {
            if (_ischecking)
            {
                _waitLock.WaitOne();
            }
            if (_contentLength <= 0)
            {
                MessageBox.Show("下载信息获取失败!");
                DialogResult = DialogResult.Cancel;
            }
            string downLoadUrl = txtDownLoadUrl.Text.Trim();
            string savePath = txtSaveDir.Text.Trim();
            if (string.IsNullOrEmpty(savePath))
            {
                MessageBox.Show("未设置文件保存路径");
                DialogResult = DialogResult.Cancel;
            }
            DownLoadWorker.RunWorkerAsync(new Tuple<string, string>(downLoadUrl,savePath));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (DownLoadWorker.IsBusy)
            {
                if (DialogResult.Yes ==
                    MessageBox.Show("当前正在执行下载任务，是否取消", "提示", MessageBoxButtons.YesNo))
                {
                    DownLoadWorker.CancelAsync();
                }
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }

        }

        private void DownLoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action<int>(p => {
                DownLoadPBar.Value = p;
                ProLabel.Text = string.Format("下载进度: {0}%", p);
            }), e.ProgressPercentage);
        }

        private void DownLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
            if (e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                btnOpenFloder.Enabled = true;
            }
        }

        private void DownLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Invoke(new Action<int>(x => {
                try
                {
                    btnDownLoad.Enabled = false;
                    btnFloder.Enabled = false;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                    SecurityProtocolType.Ssl3;
                    ServicePointManager.DefaultConnectionLimit = 50;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }), 0);
            var param = e.Argument as Tuple<string,string>;
            var downUrl = param.Item1;
            var saveFileName = param.Item2;
            var tempFileInfo = new FileInfo(saveFileName + ".tfile");
            tempFileInfo.Refresh();
            long currentIndex = 0;
            using (WebClient client = new WebClient())
            {
                using (var stream = client.OpenRead(new Uri(downUrl)))
                {
                   WriteFile(tempFileInfo, currentIndex, stream);
                }
            }
            tempFileInfo.Refresh();
            FileInfo oldFileInfo = new FileInfo(saveFileName);
            oldFileInfo.Refresh();
            if (oldFileInfo.Exists)
            {
                //删除现有文件
                oldFileInfo.Delete();
            }
            tempFileInfo.MoveTo(saveFileName);
        }


        private void WriteFile(FileInfo tempFileInfo,
            long currentIndex, Stream stream)
        {
            byte[] data = new byte[1024*1024];
            using (var fStream = new FileStream(tempFileInfo.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fStream.Position = 0;
                fStream.SetLength(_contentLength);
                var r = stream.Read(data, 0, data.Length);
                while (r >= 0 &&
                    currentIndex < _contentLength)
                {
                    fStream.Position = currentIndex;
                    fStream.Write(data, 0, r);
                    currentIndex += r;
                    var pre = (int)(((decimal)currentIndex /
                        (decimal)_contentLength) * 100);
                    DownLoadWorker.ReportProgress(pre);
                    r = stream.Read(data, 0, data.Length);
                }
                DownLoadWorker.ReportProgress(100);
            }
        }

        private void DownLoadForm_Load(object sender, EventArgs e)
        {
            _waitLock.Reset();
            ThreadPool.QueueUserWorkItem(GetFileSizeAsync, _downLoadUrl);
        }

        public string GetSizeText(decimal contentLen)
        {
            char appChar = 'B';
            decimal head = 0;
            if (contentLen == 0)
            {
                head = 0;
            }
            else if (contentLen >= 1024 && contentLen < 1024 * 1024)
            {
                appChar = 'K';
                head = contentLen / 1024m;
            }
            else if (contentLen >= 1024 * 1024 && contentLen < 1024 * 1024 * 1024)
            {
                appChar = 'M';
                head = contentLen / (1024m * 1024m);
            }
            else if (contentLen >= 1024 * 1024 * 1024 && contentLen < 1024L * 1024L * 1024L * 1024L)
            {
                appChar = 'G';
                head = contentLen / (1024m * 1024m * 1024m);
            }
            else
            {
                appChar = 'T';
                head = contentLen / (1024m * 1024m * 1024m*1024m);
            }
            return string.Format("{0} {1}", Math.Round(head,2), appChar);
        }

        private bool _ischecking = false;

        private readonly ManualResetEvent _waitLock = new ManualResetEvent(false);

        public void Init(string downLoadUrl)
        {
            _downLoadUrl = downLoadUrl;
            txtDownLoadUrl.Text = _downLoadUrl;
        }

        private void GetFileSizeAsync(object state)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            string url = state.ToString();
            Invoke(new Action<string>(downLoadUrl => {
                try
                {
                    _ischecking = true;
                    var req = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(downLoadUrl));
                    using (var res = req.GetResponse())
                    {
                        _contentLength = res.ContentLength;
                        FileLenLabel.Text = GetSizeText(_contentLength);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    DialogResult = DialogResult.Cancel;
                }
                finally
                {
                    _ischecking = false;
                    _waitLock.Set();
                }
            }), url);

        }

        private void DownLoadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_ischecking)
            {
                _waitLock.WaitOne();
            }
            if (DownLoadWorker.IsBusy)
            {
                DownLoadWorker.CancelAsync();
            }
        }
    }
}
