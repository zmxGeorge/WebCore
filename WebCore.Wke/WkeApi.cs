using System;
using System.Collections.Generic;
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
    public delegate void wkePaintUpdatedCallback(IntPtr webView, IntPtr param, IntPtr hdc, 
        int x, int y, int cx, int cy);

    public enum wkeLoadingResult
    {
        WKE_LOADING_SUCCEEDED,
        WKE_LOADING_FAILED,
        WKE_LOADING_CANCELED
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeLoadingFinishCallback(IntPtr webView, IntPtr param, 
        IntPtr url, wkeLoadingResult result,IntPtr failedReason);

    public static class WkeApi
    {
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

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void wkeLoad(IntPtr webView, string url);

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
    }
}
