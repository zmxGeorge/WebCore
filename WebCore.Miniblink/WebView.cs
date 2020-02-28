using MiniBlinkPinvokeVIP.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace WebCore.Miniblink
{
    public partial class WebView : Control
    {
        private const string CACHE_ATTR = "cache";

        private IntPtr _webView = IntPtr.Zero;

        private wkeNavigationCallback _wkeNavCallBack=null;

        private wkeLoadUrlBeginCallback _loadUrlBeginCallBack = null;

        private wkeLoadUrlEndCallback _loadUrlEndCallBack = null;

        private wkeConsoleCallback _consoleCallback = null;

        private wkeDocumentReadyCallback _documentReadyCallBack = null;

        private wkeDownloadCallback _downLoadCallBack = null;

        private wkeTitleChangedCallback _titleChangeCallBack = null;

        private wkeLoadingFinishCallback _loadingFinishCallBack = null;

        private wkeAlertBoxCallback _onAlertBoxCallBack = null;

        private wkeConfirmBoxCallback _onConfirmBoxCallBack = null;

        private wkePromptBoxCallback _onPromptBoxCallBack = null;

        public event OnNavigation Navigation;

        public event OnTitleChanged TitleChanged;

        public event OnConselMessage ConselMessage;

        public event OnDocumentReady DocumentReady;

        public event OnDownLoad DownLoad;

        public event OnLoadingComplete LoadingComplete;

        private readonly DateTime _loadTime = DateTime.Now;

        public IntPtr ViewHandle { get { return _webView; } }

        public bool UseCache { get; set; }

        public WebView()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint|
                ControlStyles.Selectable|ControlStyles.SupportsTransparentBackColor|
                ControlStyles.EnableNotifyMessage|
                ControlStyles.Opaque, true);
            _wkeNavCallBack = new wkeNavigationCallback(OnNavigation);
            _loadUrlBeginCallBack = new wkeLoadUrlBeginCallback(OnLoadUrlBegin);
            _loadUrlEndCallBack = new wkeLoadUrlEndCallback(OnLoadUrlEnd);
            _consoleCallback = new wkeConsoleCallback(OnConsel);
            _downLoadCallBack = new wkeDownloadCallback(OnDownLoad);
            _documentReadyCallBack = new wkeDocumentReadyCallback(DocumentReadyCallBack);
            _titleChangeCallBack = new wkeTitleChangedCallback(OnTitleChange);
            _loadingFinishCallBack = new wkeLoadingFinishCallback(OnLoadingFinish);
            _onAlertBoxCallBack = new wkeAlertBoxCallback(OnAlertMessage);
            _onConfirmBoxCallBack = new wkeConfirmBoxCallback(OnConfirmMessage);
            _onPromptBoxCallBack = new wkePromptBoxCallback(OnPromptMessage);
            _webView =CreateCore();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        private bool OnPromptMessage(IntPtr webView, IntPtr param, IntPtr msg, 
            IntPtr defaultResult, IntPtr result)
        {
            //暂不提供Prompt的支持
            return true;
        }

        private bool OnConfirmMessage(IntPtr webView, IntPtr param, IntPtr msg)
        {
            string message = ExtApi.GetWkeString(msg);
            var res = MessageBox.Show(message, "提示", MessageBoxButtons.OKCancel);
            return DialogResult.OK == res ? true : false;
        }

        private bool OnAlertMessage(IntPtr webView, IntPtr param, IntPtr msg)
        {
            string message = ExtApi.GetWkeString(msg);
            return DialogResult.OK==MessageBox.Show(message);
        }

        private void OnLoadingFinish(IntPtr webView, IntPtr param,
            IntPtr urlPtr, wkeLoadingResult result, IntPtr failedReasonPtr)
        {
            string url = ExtApi.GetWkeString(urlPtr);
            string failedReason = ExtApi.GetWkeString(failedReasonPtr);
            if (LoadingComplete != null)
            {
                LoadingComplete(this, url, failedReason, (UrlLoadResult)result);
            }
        }

        private void OnTitleChange(IntPtr webView, IntPtr param, IntPtr titlePtr)
        {
            string title = ExtApi.GetWkeString(titlePtr);
            if (TitleChanged != null)
            {
                TitleChanged(this, title);
            }
        }

        private void DocumentReadyCallBack(IntPtr webView, IntPtr param)
        {
            if (DocumentReady != null)
            {
                DocumentReady(this);
            }
        }

        private bool OnDownLoad(IntPtr webView, IntPtr param,string url)
        {
            if (DownLoad != null)
            {
                DownLoad(this, url);
            }
            return false;
        }

        private void OnConsel(IntPtr webView, IntPtr param, wkeConsoleLevel level, 
            IntPtr messagePtr, IntPtr sourceNamePtr, uint sourceLine, IntPtr stackTracePtr)
        {
            string message = ExtApi.GetWkeString(messagePtr);
            string sourceName = ExtApi.GetWkeString(sourceNamePtr);
            string stackTrace = ExtApi.GetWkeString(stackTracePtr);
            if (ConselMessage != null)
            {
                ConselMessage(this, message, sourceName, (int)sourceLine, stackTrace);
            }
        }

        private void OnLoadUrlEnd(IntPtr webView, IntPtr param, string url, IntPtr job, IntPtr buf, int len)
        {
            var hostUrl = new Uri(url);
            if (len == 0)
            {
                return;
            }
            byte[] data = new byte[len];
            Marshal.Copy(buf,data,0,data.Length);
            string fileName = Path.Combine(CACHE_ATTR, string.Format(PATH_FORMAT, hostUrl.Host,
                    hostUrl.Port, GetHashString(url), Path.GetExtension(url)));
            File.WriteAllBytes(fileName, data);
        }

        private static readonly string[] htmlExt = new string[] {
            ".html",".htm",".shtml",".dhtml"
        };

        private static readonly string[] cssExt = new string[] {
            ".css"
        };

        private static readonly string[] jsExt = new string[] {
            ".js"
        };

        private static readonly string[] imgExt = new string[] {
            ".gif",".jpg",".jpeg",".tiff",
            ".png",".woff",".svg",".bmp",".ico",".ai",".emf",".exif",
            ".tga",".psd",".pcx","tif"
        };

        private static readonly string[] fontExt = new string[] {
            ".woff",".woff2",".tff",".eot"
        };

        private const string X_TWO = "X2";

        private const string PATH_FORMAT = "{0}/{1}/{2}.{3}";

        private string GetHashString(string url)
        {
            SHA256 sha256 = new SHA256Managed();
            var hash=sha256.ComputeHash(Encoding.UTF8.GetBytes(url));
            StringBuilder sb = new StringBuilder();
            foreach (var c in hash)
            {
                sb.Append(c.ToString(X_TWO));
            }
            return sb.ToString();
        }

        private bool OnLoadUrlBegin(IntPtr webView, IntPtr param, string url, IntPtr job)
        {
            if (!UseCache)
            {
                return false;
            }
            string ext = Path.GetExtension(url).ToLower();
            if (Array.IndexOf(htmlExt,ext)!=-1||
                Array.IndexOf(cssExt, ext) != -1 ||
                Array.IndexOf(jsExt, ext) != -1 ||
                Array.IndexOf(imgExt, ext) != -1 ||
                Array.IndexOf(fontExt, ext) != -1)
            {
                
                var hostUrl = new Uri(url);
                FileInfo fInfo =new FileInfo(Path.Combine(CACHE_ATTR, string.Format(PATH_FORMAT,hostUrl.Host,
                    hostUrl.Port,GetHashString(url),ext)));
                fInfo.Refresh();
                if (!fInfo.Directory.Exists)
                {
                    fInfo.Directory.Create();
                }
                var t = (DateTime.Now - fInfo.LastAccessTime);
                if (fInfo.Exists&&t.TotalMinutes<=15)
                {
                    byte[] fData = File.ReadAllBytes(fInfo.FullName);
                    IntPtr buf = Marshal.AllocHGlobal(fData.Length);
                    Marshal.Copy(fData, 0, buf, fData.Length);
                    MBApi.wkeNetSetData(job, buf, fData.Length);
                    fInfo.LastAccessTime = DateTime.Now;
                    return true;
                }
                MBApi.wkeNetHookRequest(job);
                return true;
            }
            return false;
            
        }

        private bool OnNavigation(IntPtr webView, IntPtr param, wkeNavigationType navigationType, IntPtr urlPtr)
        {
            if (Navigation != null)
            {
                var c = (int)navigationType;
                var url = ExtApi.GetWkeString(urlPtr);
                var res= Navigation(this, (NavigationType)c, url);
                if (res)
                {
                    List<Control> controls = new List<Control>();
                    foreach (Control cs in Controls)
                    {
                        controls.Add(cs);
                    }
                    Controls.Clear();
                    foreach (var item in controls)
                    {
                        //释放通过js创建的控件
                        item.Dispose();
                    }
                    Browser.Current.ClearViewObject(_webView);
                }
                return res;
            }
            return true;
        }

        private IntPtr CreateCore()
        {
            var window = MBApi.wkeCreateWebWindow(_wkeWindowType.WKE_WINDOW_TYPE_CONTROL, Handle, 0, 0, Width, Height);
            MBApi.wkeSetCookieEnabled(window, true);
            MBApi.wkeSetNpapiPluginsEnabled(window, true);
            MBApi.wkeSetMemoryCacheEnable(window, true);
            MBApi.wkeSetNavigationToNewWindowEnable(window, false);
            MBApi.wkeSetCspCheckEnable(window, false);
            MBApi.wkeResize(window, Width, Height);
            MBApi.wkeEnableWindow(window, true);
            MBApi.wkeShowWindow(window, true);
            MBApi.wkeSetUserAgent(window, ExtApi.SetWkeString(BlinkCommon.BlinkUserAgent));
            MBApi.wkeSetDebugConfig(window, "antiAlias", BlinkCommon.BlinkAntiAlias);
            MBApi.wkeSetDebugConfig(window, "minimumFontSize", BlinkCommon.BlinkMinimumFontSize);
            MBApi.wkeSetDebugConfig(window,"minimumLogicalFontSize", BlinkCommon.BlinkMinimumLogicalFontSize);
            MBApi.wkeSetDebugConfig(window, "defaultFixedFontSize", BlinkCommon.BlinkDefaultFixedFontSize);
            MBApi.wkeSetContextMenuItemShow(window, wkeMenuItemId.kWkeMenuCopyImageId |
                wkeMenuItemId.kWkeMenuCutId | wkeMenuItemId.kWkeMenuGoBackId |
                wkeMenuItemId.kWkeMenuGoForwardId |
                wkeMenuItemId.kWkeMenuInspectElementAtId |
                wkeMenuItemId.kWkeMenuPasteId |
                wkeMenuItemId.kWkeMenuPrintId |
                wkeMenuItemId.kWkeMenuReloadId |
                wkeMenuItemId.kWkeMenuSelectedAllId |
                wkeMenuItemId.kWkeMenuSelectedTextId |
                wkeMenuItemId.kWkeMenuUndoId, true);
            MBApi.wkeOnNavigation(window, _wkeNavCallBack, IntPtr.Zero);
            MBApi.wkeOnLoadUrlBegin(window, _loadUrlBeginCallBack, IntPtr.Zero);
            MBApi.wkeOnLoadUrlEnd(window, _loadUrlEndCallBack, IntPtr.Zero);
            MBApi.wkeOnConsole(window, _consoleCallback, IntPtr.Zero);
            MBApi.wkeOnDownload(window, _downLoadCallBack, IntPtr.Zero);
            MBApi.wkeOnDocumentReady(window, _documentReadyCallBack, IntPtr.Zero);
            MBApi.wkeOnTitleChanged(window, _titleChangeCallBack, IntPtr.Zero);
            MBApi.wkeOnLoadingFinish(window, _loadingFinishCallBack, IntPtr.Zero);
            MBApi.wkeOnAlertBox(window, _onAlertBoxCallBack, IntPtr.Zero);
            MBApi.wkeOnConfirmBox(window, _onConfirmBoxCallBack, IntPtr.Zero);
            MBApi.wkeOnPromptBox(window, _onPromptBoxCallBack, IntPtr.Zero);
            var cacheDir = new DirectoryInfo(CACHE_ATTR);
            cacheDir.Refresh();
            if (!cacheDir.Exists)
            {
                cacheDir.Create();
            }
            return window;
        }

        

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                //调试工具
                if (_webView != IntPtr.Zero)
                {
                    string toolFileUrl = Path.GetFullPath(@"front_end\inspector.html");
                    MBApi.wkeShowDevtools(_webView, toolFileUrl, null, IntPtr.Zero);
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                //刷新
                if (_webView != IntPtr.Zero)
                {
                    MBApi.wkeReload(_webView);
                }
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            MBApi.wkeSetFocus(_webView);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            MBApi.wkeKillFocus(_webView);
            base.OnLostFocus(e);
        }

        public void Load(string url)
        {

            Uri fUrl = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out fUrl))
            {
                MBApi.wkeLoadFileW(_webView, Path.GetFullPath(url));
            }
            else
            {
                MBApi.wkeLoadURLW(_webView, url);
            }
            MBApi.wkeSetFocus(_webView);
        }

        protected override void OnResize(EventArgs e)
        {
            if (_webView != IntPtr.Zero)
            {
                MBApi.wkeResize(_webView, Width,Height);
            }
            base.OnResize(e);
        }



        protected override void OnVisibleChanged(EventArgs e)
        {
            if (_webView != IntPtr.Zero)
            {
                if (Visible)
                {
                    MBApi.wkeShowWindow(_webView, true);
                }
                else
                {
                    MBApi.wkeShowWindow(_webView, false);
                }
            }
            base.OnVisibleChanged(e);
        }
    }
}
