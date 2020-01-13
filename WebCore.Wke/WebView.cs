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

        private JavaScriptContext _scriptContext = null;

        private wkePaintUpdatedCallback _onPaint = null;

        private wkeLoadingFinishCallback _onFinish = null;


        /// <summary>
        /// 获取或设置浏览器控件标题
        /// </summary>
        public string Title
        {
            get
            {
                if (_webView == IntPtr.Zero)
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

        /// <summary>
        /// 获取JS上下文对象
        /// </summary>
        public JavaScriptContext ScriptContext { get { return _scriptContext; } }

        public WebView()
        {
            _onPaint = new wkePaintUpdatedCallback(OnWebPaint);
            _onFinish = new wkeLoadingFinishCallback(OnWebFinsh);
            this.SetStyle(ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Selectable |
                ControlStyles.Opaque, true);
        }

        /// <summary>
        /// 页面加载完成之后引发的事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="result"></param>
        /// <param name="failedReason"></param>
        private void OnWebFinsh(IntPtr webView, IntPtr param,
            IntPtr url, wkeLoadingResult result,
            IntPtr failedReason)
        {
            if (result == wkeLoadingResult.WKE_LOADING_SUCCEEDED)
            {
                WkeApi.wkeRepaintAllNeeded();
            }
        }

        #region 绘制

        private readonly ManualResetEvent _threadWait = new ManualResetEvent(true);

        private readonly ManualResetEvent _messageSet = new ManualResetEvent(false);

        private Bitmap imgMap = new Bitmap(1,1);

        private void OnWebPaint(IntPtr webView,
            IntPtr param, IntPtr hdc,
            int x, int y, int width, int height)
        {
            if (Created && !IsDisposed && !Disposing &&
                Visible && Width > 0 && Height > 0)
            {
                try
                {
                    lock (imgMap)
                    {
                        using (Graphics g = Graphics.FromImage(imgMap))
                        {
                            var cDc = g.GetHdc();
                            WindowApi.BitBlt(cDc, x, y, width, height,
                                hdc, x, y, CopyPixelOperation.SourceCopy);
                            g.ReleaseHdc(cDc);
                        }
                    }
                }
                finally
                {
                    this.Invalidate(new Rectangle(x, y, width, height));
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
            //WindowApi.SelectObject(img_hdc, hibitMap);
            WindowApi.DeleteObject(objMapPtr);
            WindowApi.DeleteDC(img_hdc);
            WindowApi.DeleteObject(hibitMap);
        }

        private const int WM_KEYDOWN = 0x100;

        private const int WM_KEYUP = 0x101;

        private const int WM_CHAR = 0x102;


       

        private void RunPaint(object state)
        {
            try
            {
                _threadWait.Reset();
                while (!_messageSet.WaitOne(25))
                {
                    try
                    {
                        if (!Disposing && !IsDisposed &&
                            Visible && Width > 0 && Height > 0 && Created)
                        {
                            var msg = new Message();
                            Invoke(new Action<Message>(x =>
                            {
                                //通过Windows消息刷新
                                x.HWnd = Handle;
                                x.Msg = (int)WindowApi.WM_PAINT;
                                ReflectMessage(Handle, ref x);
                            }), msg);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            finally
            {
                _threadWait.Set();
            }
        }

        #endregion

        #region 绘制相关事件重写
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                WkeApi.wkeRepaintAllNeeded();
                _threadWait.WaitOne();
                _messageSet.Reset();
                ThreadPool.QueueUserWorkItem(RunPaint, null);
            }
            else
            {
                _messageSet.Set();
            }
            base.OnVisibleChanged(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeSetFocus(_webView);
            }
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (_webView != IntPtr.Zero)
            {
                WkeApi.wkeKillFocus(_webView);
            }
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
                if (Width > 0 && Height > 0)
                {
                    lock (imgMap)
                    {
                        imgMap.Dispose();
                        imgMap = null;
                        GC.Collect();
                        imgMap = new Bitmap(Width, Height);
                    }
                }
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
                _messageSet.Set();
                WkeApi.wkeDestroyWebView(_webView);
                _webView = IntPtr.Zero;
            }
            base.OnHandleDestroyed(e);
        }

        #endregion

        #region 操作

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
                if (ScriptContext != null)
                {
                    ScriptContext.Dispose();
                    GC.Collect();
                }
                _scriptContext = new JavaScriptContext(_webView);
                WkeApi.wkeSetCookieEnabled(_webView,true);
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

        #endregion

        #region 处理键盘消息
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            ProcessKey(ref m);
            return base.ProcessKeyEventArgs(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //就这个事件里，监听的到按向上下左右的KEY_DOWN消息
            if (msg.Msg == WM_KEYDOWN)
            {
                bool isSys = false;
                if (keyData >= Keys.F1 && keyData <= Keys.F24||
                    keyData==Keys.Escape)
                {
                    isSys = true;
                }
                uint virtualKeyCode, flags;
                ProcessKeyEvent(msg, out virtualKeyCode, out flags);
                if (WkeApi.wkeFireKeyDownEvent(_webView, virtualKeyCode, flags, isSys))
                {
                    msg.Result = IntPtr.Zero;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void ProcessKey(ref Message m,bool isSys=false)
        {
            switch (m.Msg)
            {
                case WM_KEYUP:
                    {
                        uint virtualKeyCode, flags;
                        ProcessKeyEvent(m, out virtualKeyCode, out flags);
                        if (virtualKeyCode >= (uint)Keys.F1 && virtualKeyCode <= (uint)Keys.F24 ||
                            virtualKeyCode == (uint)Keys.Escape)
                        {
                            isSys = true;
                        }
                        if (WkeApi.wkeFireKeyUpEvent(_webView, virtualKeyCode, flags, isSys))
                        {
                            m.Result = IntPtr.Zero;
                        }
                    }
                    break;
                case WM_CHAR:
                    {
                        uint virtualKeyCode, flags;
                        ProcessKeyEvent(m, out virtualKeyCode, out flags);
                        if (virtualKeyCode >= (uint)Keys.F1 && virtualKeyCode <= (uint)Keys.F24 ||
                            virtualKeyCode == (uint)Keys.Escape)
                        {
                            isSys = true;
                        }
                        var keys = (Keys)virtualKeyCode;
                        if (WkeApi.wkeFireKeyPressEvent(_webView, virtualKeyCode, flags, isSys))
                        {
                            m.Result = IntPtr.Zero;
                        }
                    }
                    break;
            }
        }

        private short HIWORD(IntPtr LPARAM)
        {
            return ((short)(((LPARAM.ToInt32()) >> 16) & 0xffff));
        }

        private short LOWORD(IntPtr LPARAM)
        {
            return ((short)((LPARAM.ToInt32()) & 0xffff));
        }

        private void ProcessKeyEvent(Message msg,
            out uint virtualKeyCode, out uint flags)
        {
            virtualKeyCode = (uint)msg.WParam.ToInt32();
            flags = 0;
            if ((HIWORD(msg.LParam) & WkeApi.KF_REPEAT) != 0)
                flags |= (uint)wkeKeyFlags.WKE_REPEAT;
            if ((HIWORD(msg.LParam) & WkeApi.KF_EXTENDED) != 0)
                flags |= (uint)wkeKeyFlags.WKE_EXTENDED;
        }
        #endregion

        protected override void DefWndProc(ref Message m)
        {
            base.DefWndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WindowApi.WM_NCHITTEST ||
                m.Msg == WindowApi.WM_SETCURSOR ||
                m.Msg == WindowApi.WM_REFRESH)
            {
                WkeApi.wkeRepaintAllNeeded();
            }
            else
            {
                #region 处理鼠标消息
                ProcessMouseEvent(ref m);
                #endregion

                #region 输入法支持
                if (m.Msg == WindowApi.WM_IME_STARTCOMPOSITION)
                {
                    wkeRect caret =WkeApi.wkeGetCaretRect(_webView);
                    CANDIDATEFORM form=new CANDIDATEFORM();
                    form.dwIndex = 0;
                    form.dwStyle =(int)WindowApi.CFS_EXCLUDE;
                    form.ptCurrentPos.X = caret.x;
                    form.ptCurrentPos.Y = caret.y;
                    form.rcArea = new Rectangle(caret.x, caret.y, caret.w, caret.h);
                    COMPOSITIONFORM compForm=new COMPOSITIONFORM();
                    compForm.ptCurrentPos = form.ptCurrentPos;
                    compForm.rcArea = form.rcArea;
                    compForm.dwStyle = (int)WindowApi.CFS_POINT;
                    var form_size = Marshal.SizeOf(form);
                    var formPtr = Marshal.AllocHGlobal(form_size);
                    Marshal.StructureToPtr(form, formPtr, true);
                    var comp_size = Marshal.SizeOf(compForm);
                    var compFormPtr = Marshal.AllocHGlobal(form_size);
                    Marshal.StructureToPtr(compForm, compFormPtr, true);
                    var hIMC = WindowApi.ImmGetContext(Handle);
                    WindowApi.ImmSetCandidateWindow(hIMC, formPtr);
                    WindowApi.ImmSetCompositionWindow(hIMC, compFormPtr);
                    WindowApi.ImmReleaseContext(Handle, hIMC);
                    Marshal.FreeHGlobal(formPtr);
                    Marshal.FreeHGlobal(compFormPtr);
                }
                #endregion
            }
            base.WndProc(ref m);
        }

        private void ProcessMouseEvent(ref Message m)
        {
            if (m.Msg == WindowApi.WM_LBUTTONDBLCLK ||
                               m.Msg == WindowApi.WM_LBUTTONDOWN ||
                               m.Msg == WindowApi.WM_LBUTTONUP ||
                               m.Msg == WindowApi.WM_MBUTTONDBLCLK ||
                               m.Msg == WindowApi.WM_MBUTTONDOWN ||
                               m.Msg == WindowApi.WM_MBUTTONUP ||
                               m.Msg == WindowApi.WM_RBUTTONDBLCLK ||
                               m.Msg == WindowApi.WM_RBUTTONDOWN ||
                               m.Msg == WindowApi.WM_RBUTTONUP ||
                               m.Msg == WindowApi.WM_MOUSEFIRST ||
                                m.Msg == WindowApi.WM_MOUSEMOVE)
            {
                var message = m.Msg;
                int x = LOWORD(m.LParam);
                int y = HIWORD(m.LParam);

                uint flags = 0;
                var wParam = m.WParam.ToInt32();
                if ((wParam & WindowApi.MK_CONTROL) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_CONTROL;
                if ((wParam & WindowApi.MK_SHIFT) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_SHIFT;

                if ((wParam & WindowApi.MK_LBUTTON) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_LBUTTON;
                if ((wParam & WindowApi.MK_MBUTTON) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_MBUTTON;
                if ((wParam & WindowApi.MK_RBUTTON) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_RBUTTON;

                if (WkeApi.wkeFireMouseEvent(_webView,
                    (uint)message, x, y, flags))
                {
                    m.Result = IntPtr.Zero;
                }
            }
            else if (m.Msg == WindowApi.WM_MOUSEWHEEL)
            {
                int x = LOWORD(m.LParam);
                int y = HIWORD(m.LParam);
                var pt = PointToClient(new Point(x, y));
                var wParam = m.WParam.ToInt32();
                int delta = HIWORD(m.WParam);
                uint flags = 0;
                if ((wParam & WindowApi.MK_CONTROL) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_CONTROL;
                if ((wParam & WindowApi.MK_SHIFT) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_SHIFT;

                if ((wParam & WindowApi.MK_LBUTTON) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_LBUTTON;
                if ((wParam & WindowApi.MK_MBUTTON) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_MBUTTON;
                if ((wParam & WindowApi.MK_RBUTTON) != 0)
                    flags |= (uint)wkeMouseFlags.WKE_RBUTTON;

                if (WkeApi.wkeFireMouseWheelEvent(_webView, pt.X, pt.Y,
                    delta, flags))
                {
                    m.Result = IntPtr.Zero;
                }
            }
        }
    }
}
