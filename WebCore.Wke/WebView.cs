using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace WebCore.Wke
{
    public class WebView:Control
    {
        private IntPtr _webView = IntPtr.Zero;

        private string _url = null;

        private wkePaintUpdatedCallback _onPaint = null;

        private wkeLoadingFinishCallback _onFinish = null;

        public WebView()
        {
            _onPaint = new wkePaintUpdatedCallback(OnWebPaint);
            _onFinish = new wkeLoadingFinishCallback(OnWebFinsh);
        }

        private void OnWebFinsh(IntPtr webView, IntPtr param,
            IntPtr url, wkeLoadingResult result,
            IntPtr failedReason)
        {
            if (result == wkeLoadingResult.WKE_LOADING_SUCCEEDED)
            {
                WkeApi.wkeRepaintAllNeeded();
            }
        }

        private readonly Semaphore _lock = new Semaphore(1, 1);

        private Bitmap imgMap = null;

        private void OnWebPaint(IntPtr webView, 
            IntPtr param, IntPtr hdc,
            int x, int y, int width, int height)
        {
            if (Created&&!IsDisposed&&!Disposing&&
                Visible&&Width>0&&Height>0)
            {
                _lock.WaitOne();
                try
                {
                    if (imgMap != null)
                    {
                        imgMap.Dispose();
                        imgMap = null;
                        GC.Collect(0);
                    }
                    imgMap = new Bitmap(Width, Height);
                    using (Graphics g = Graphics.FromImage(imgMap))
                    {
                        var cDc = g.GetHdc();
                        WindowApi.BitBlt(cDc, x, y, width, height,
                            hdc, x, y, CopyPixelOperation.SourceCopy);
                        g.ReleaseHdc(cDc);
                    }
                }
                finally
                {
                    _lock.Release();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="hdc"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Draw(Bitmap bitmap, IntPtr hdc, int width, int height)
        {
            var hibitMap = bitmap.GetHbitmap();
            var img_hdc = WindowApi.CreateCompatibleDC(hdc);
            var objMapPtr = WindowApi.SelectObject(img_hdc, hibitMap);
            var b_rect = new Rectangle(0, 0, width, height);
            var rs = WindowApi.BitBlt(hdc, 0, 0, bitmap.Width, bitmap.Height,
                img_hdc, 0, 0, CopyPixelOperation.SourceCopy);
            WindowApi.SelectObject(img_hdc, hibitMap);
            WindowApi.DeleteDC(img_hdc);
            WindowApi.DeleteObject(hibitMap);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (imgMap != null)
            {
                var hdc = e.Graphics.GetHdc();
                Draw(imgMap, hdc, Width, Height);
                e.Graphics.ReleaseHdc(hdc);
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// 获取或设置浏览器控件标题
        /// </summary>
        public string Title
        {
            get
            {
                if(_webView==IntPtr.Zero)
                {
                    return null;
                }
                return WkeApi.wkeGetTitle(_webView);
            }
            set
            {
                if (_webView == IntPtr.Zero)
                {
                    return;
                }
                WkeApi.wkeSetWindowTitle(_webView, value);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                WkeApi.wkeRepaintAllNeeded();
            }
            base.OnVisibleChanged(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {

            base.OnLostFocus(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (_webView != IntPtr.Zero)
            {
                //改变大小
                WkeApi.wkeResize(_webView, Width, Height);
                WkeApi.wkeRepaintAllNeeded();
            }
            else
            {
                this.Invalidate(true);
            }
            base.OnSizeChanged(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeDestroyWebView(_webView);
                _webView = IntPtr.Zero;
            }
            base.OnHandleDestroyed(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        /// <summary>
        /// 加载URL地址
        /// </summary>
        /// <param name="url"></param>
        public void Load(string url)
        {
            _url = url;
            if (_webView == IntPtr.Zero)
            {
                //离屏渲染模式
                _webView = WkeApi.wkeCreateWebView();
                WkeApi.wkeSetHostWindow(_webView, Handle);
                //设置缩放大小，默认为1
                WkeApi.wkeSetZoomFactor(_webView,1f);
                //给出初始大小
                WkeApi.wkeResize(_webView, Width, Height);
                //监听绘制事件
                WkeApi.wkeOnPaintUpdated(_webView,
                    _onPaint, IntPtr.Zero);
                //加载完成事件
                WkeApi.wkeOnLoadingFinish(_webView,
                    _onFinish, IntPtr.Zero);
            }
            WkeApi.wkeLoad(_webView, url);
        }

        /// <summary>
        /// 读取HTML文件加载到浏览器
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFile(string fileName)
        {
            _url = Path.GetFullPath(fileName);
        }

        /// <summary>
        /// 后退
        /// </summary>
        /// <returns></returns>
        public bool GoBack()
        {
            if (_webView != IntPtr.Zero &&
                WkeApi.wkeCanGoBack(_webView))
            {
                return WkeApi.wkeGoBack(_webView);
            }
            return false;
        }

        /// <summary>
        /// 前进
        /// </summary>
        /// <returns></returns>
        public bool GoForward()
        {
            if (_webView != IntPtr.Zero &&
                WkeApi.wkeCanGoForward(_webView))
            {
                return WkeApi.wkeGoForward(_webView);
            }
            return false;
        }

        /// <summary>
        /// 复制
        /// </summary>
        public void Copy()
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeEditorCopy(_webView);
            }
        }

        /// <summary>
        /// 剪切
        /// </summary>
        public void Cut()
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeEditorCut(_webView);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeEditorDelete(_webView);
            }
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        public void Paste()
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeEditorPaste(_webView);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
    }
}
