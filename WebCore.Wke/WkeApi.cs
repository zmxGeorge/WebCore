using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WebCore.Wke
{
    public enum wkeWindowType
    {
        WKE_WINDOW_TYPE_POPUP,
        WKE_WINDOW_TYPE_TRANSPARENT,
        WKE_WINDOW_TYPE_CONTROL
    }

    public enum wkeProxyType
    {
        WKE_PROXY_NONE,
        WKE_PROXY_HTTP,
        WKE_PROXY_SOCKS4,
        WKE_PROXY_SOCKS4A,
        WKE_PROXY_SOCKS5,
        WKE_PROXY_SOCKS5HOSTNAME

    }


    public enum wkeMouseFlags
    {
        WKE_LBUTTON = 0x01,
        WKE_RBUTTON = 0x02,
        WKE_SHIFT = 0x04,
        WKE_CONTROL = 0x08,
        WKE_MBUTTON = 0x10,

    }

    public enum wkeKeyFlags
    {
        WKE_EXTENDED = 0x0100,
        WKE_REPEAT = 0x4000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public long left;
        public long right;
        public long top;
        public long bottom;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public long y;
        public long x;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CANDIDATEFORM
    {
        public int dwIndex;
        public int dwStyle;
        public Point ptCurrentPos;
        public Rectangle rcArea;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COMPOSITIONFORM
    {
        public int dwStyle;
        public Point ptCurrentPos;
        public Rectangle rcArea;
    }



    public struct wkeRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    public struct wkeProxy
    {
        public wkeProxyType type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public char[] hostname;
        public ushort port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public char[] username;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =50)]
        public char[] password;
    }
    ;

    enum wkeSettingMask
    {
        WKE_SETTING_PROXY = 1,
        WKE_SETTING_COOKIE_FILE_PATH = 1 << 1
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeSettings
    {
        public wkeProxy proxy;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public char[] cookieFilePath;
        public uint mask;
    };

    public struct BITMAPFILEHEADER
    {
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    public struct BITMAPINFOHEADER
    {
        public int biSize;
        public long biWidth;
        public long biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public long biXPelsPerMeter;
        public long biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeMessageBoxCallback(IntPtr webView, IntPtr param, IntPtr msg);

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeConsoleMessage
    {
        public MessageSource source;

        public MessageType type;

        public MessageLevel level;

        public IntPtr message;

        public IntPtr url;

        public int lineNumber;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeConsoleMessageCallback (IntPtr webView, IntPtr param,IntPtr message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkePaintUpdatedCallback(IntPtr webView, IntPtr param, IntPtr hdc, 
        int x, int y, int cx, int cy);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FILE_OPEN(string path);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FILE_CLOSE(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long FILE_SIZE(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FILE_READ(IntPtr handle, IntPtr buffer, long size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FILE_SEEK(IntPtr handle, int offset, int origin);

    public enum wkeLoadingResult
    {
        WKE_LOADING_SUCCEEDED,
        WKE_LOADING_FAILED,
        WKE_LOADING_CANCELED
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeLoadingFinishCallback(IntPtr webView, IntPtr param, 
        IntPtr url, wkeLoadingResult result,IntPtr failedReason);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool wkeNavigationCallback(IntPtr webView, IntPtr param,
        NavigationType navigationType, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeDocumentReadyCallback(IntPtr webView, IntPtr param, IntPtr info);


    public static class WkeApi
    {
        public const int KF_EXTENDED = 0x0100;
        public const int KF_DLGMODE = 0x0800;
        public const int KF_MENUMODE = 0x1000;
        public const int KF_ALTDOWN = 0x2000;
        public const int KF_REPEAT = 0x4000;
        public const int KF_UP = 0x8000;

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern string wkeGetString(IntPtr strPtr);

        [DllImport("core/wke.dll",EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr wkeGetStringPtr(IntPtr strPtr);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeConfigure(IntPtr settings);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeInitialize();

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeFinalize();

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeUpdate();

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr wkeGetViewDC(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeLayoutIfNeeded();

        /// <summary>
        /// 绘制消息监听
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnPaintUpdated(IntPtr webView, wkePaintUpdatedCallback callback, IntPtr callbackParam);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeRepaintIfNeeded();
        
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeRepaintAllNeeded();

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetEditable(IntPtr webView, bool edit);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetTransparent(IntPtr webView, bool tran);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkePaint2(IntPtr webView, IntPtr bits, int pitch);

        /// <summary>
        /// 创建一个wke浏览器窗体
        /// </summary>
        /// <param name="windowType"></param>
        /// <param name="parent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport("core/wke.dll",CallingConvention =CallingConvention.Cdecl,CharSet =CharSet.Unicode)]
        public static extern IntPtr wkeCreateWebWindow(wkeWindowType windowType, 
            IntPtr parent, int x, int y, int width, int height);

        /// <summary>
        /// 创建一个无窗体浏览器
        /// </summary>
        /// <returns></returns>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr wkeCreateWebView();

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetHostWindow(IntPtr webView, IntPtr handle);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetZoomFactor(IntPtr webView, float factor);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeDestroyWebView(IntPtr webView);

        /// <summary>
        /// 销毁一个浏览器窗体
        /// </summary>
        /// <param name="webView"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeDestroyWebWindow(IntPtr webView);

        /// <summary>
        /// 改变窗体位置和大小
        /// </summary>
        /// <param name="webWindow"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeMoveWindow(IntPtr webWindow, int x, int y, int width, int height);

        /// <summary>
        /// 改变窗体大小
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeResize(IntPtr webView, int width, int height);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeReload(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeWake(IntPtr webView);

        /// <summary>
        /// 改变窗体大小
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeResizeWindow(IntPtr webView, int width, int height);

        /// <summary>
        /// 是否显示浏览器
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="visible"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeShowWindow(IntPtr webView,bool visible);

        /// <summary>
        /// 是否启用浏览器
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeEnableWindow(IntPtr webView, bool enable);

        /// <summary>
        /// 加载完成时发生
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnLoadingFinish(IntPtr webView, wkeLoadingFinishCallback callback,IntPtr param);

        /// <summary>
        /// 页面跳转时发生
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnNavigation(IntPtr webView, wkeNavigationCallback callback, IntPtr param);

        /// <summary>
        /// 当文档完全加载时发生
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnDocumentReady(IntPtr webView, wkeDocumentReadyCallback callback, IntPtr param);

        /// <summary>
        /// 发送控制台消息时发生
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnConsoleMessage(IntPtr webView, wkeConsoleMessageCallback callback,IntPtr callbackParam);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnAlertBox(IntPtr webView, wkeMessageBoxCallback callback, IntPtr callbackParam);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnConfirmBox(IntPtr webView, wkeMessageBoxCallback callback, IntPtr callbackParam);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeOnPromptBox(IntPtr webView, wkeMessageBoxCallback callback, IntPtr callbackParam);

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern string wkeGetTitle(IntPtr webView);

        /// <summary>
        /// 设置窗体标题
        /// </summary>
        /// <param name="webWindow"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern string wkeSetWindowTitle(IntPtr webWindow, string title);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeLoad(IntPtr webView, string url);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeLoadHTML(IntPtr webView, IntPtr html);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeAddDirtyArea(IntPtr webView, int x, int y, int width, int height);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeCanGoBack(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeGoBack(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeCanGoForward(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeGoForward(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeEditorSelectAll(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeEditorCopy(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeEditorCut(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeEditorPaste(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeEditorDelete(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern long wkeRunJS(IntPtr webView, string script);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetCookieEnabled(IntPtr webView,bool enable);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetFocus(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeKillFocus(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeFireMouseEvent(IntPtr webView, uint message, int x, int y, uint flags);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeFireContextMenuEvent(IntPtr webView, int x, int y, uint flags);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeFireMouseWheelEvent(IntPtr webView, int x, int y, int delta, uint flags);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeFireKeyUpEvent(IntPtr webView, uint virtualKeyCode, uint flags, bool systemKey);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeFireKeyDownEvent(IntPtr webView,  uint virtualKeyCode, uint flags, bool systemKey);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool wkeFireKeyPressEvent(IntPtr webView,  uint charCode, uint flags, bool systemKey);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern wkeRect wkeGetCaretRect(IntPtr webView);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetUserAgent(IntPtr webView, string userAgent);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeSetFileSystem(FILE_OPEN pfn_open, FILE_CLOSE pfn_close, FILE_SIZE pfn_size, FILE_READ pfn_read, FILE_SEEK pfn_seek);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr wkeGlobalExec(IntPtr webView);

    }
}
