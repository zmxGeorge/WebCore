using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WebCore.Wke
{
    public static class WindowApi
    {

        public const uint WM_PAINT = 0xf;

        public const uint WM_REFRESH = 0x200f;

        public const uint WM_NCHITTEST = 0x84;

        public const uint WM_SETCURSOR = 0x20;

        public const uint WM_MOUSEWHEEL = 0x20a;
        
        public const uint WM_MOUSEMOVE = 0x200;

        public const uint WM_MOUSEFIRST  =0x0200;

        public const uint WM_LBUTTONDOWN = 0x0201;

        public const uint WM_LBUTTONUP = 0x0202;

        public const uint WM_LBUTTONDBLCLK = 0x0203;

        public const uint WM_RBUTTONDOWN = 0x0204;

        public const uint WM_RBUTTONUP = 0x0205;

        public const uint WM_RBUTTONDBLCLK = 0x0206;

        public const uint WM_MBUTTONDOWN = 0x0207;

        public const uint WM_MBUTTONUP = 0x0208;

        public const uint WM_MBUTTONDBLCLK = 0x0209;

        public const uint MK_LBUTTON = 0x0001;

        public const uint MK_RBUTTON = 0x0002;

        public const uint MK_SHIFT = 0x0004;

        public const uint MK_CONTROL = 0x0008;

        public const uint MK_MBUTTON = 0x0010;

        #region WindowsApi
        public struct RECT
        {
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
        }
        /// <summary>
        /// 返回指定窗口客户区矩形的大小
        /// </summary>
        /// <param name="hwnd">欲计算大小的目标窗口</param>
        /// <param name="lpRect">指定一个矩形，用客户区域的大小载入（以像素为单位）</param>
        /// <returns></returns>
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);
        /// <summary>
        /// 获取指定窗口的设备场景
        /// </summary>
        /// <param name="hwnd">将获取其设备场景的窗口的句柄。若为0，则要获取整个屏幕的DC</param>
        /// <returns></returns>
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetDC(IntPtr hwnd);
        /// <summary>
        /// 释放由调用GetDC或GetWindowDC函数获取的指定设备场景。它对类或私有设备场景无效（但这样的调用不会造成损害）
        /// </summary>
        /// <param name="hWnd">要释放的设备场景相关的窗口句柄</param>
        /// <param name="hdc">要释放的设备场景句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);
        /// <summary>
        /// 创建一个与特定设备场景一致的内存设备场景
        /// </summary>
        /// <param name="hdc">设备场景句柄。新的设备场景将与它一致。也可能为0以创建一个与屏幕一致的设备场景</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        /// <summary>
        /// 每个设备场景都可能有选入其中的图形对象。其中包括位图、刷子、字体、画笔以及区域等等。一次选入设备场景的只能有一个对象。选定的对象会在设备场景的绘图操作中使用。例如，当前选定的画笔决定了在设备场景中描绘的线段颜色及样式
        /// </summary>
        /// <param name="hdc">一个设备场景的句柄</param>
        /// <param name="objPtr">一个画笔、位图、刷子、字体或区域的句柄</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr objPtr);
        /// <summary>
        /// 用这个函数删除GDI对象，比如画笔、刷子、字体、位图、区域以及调色板等等。对象使用的所有系统资源都会被释放
        /// </summary>
        /// <param name="objPtr">一个GDI对象的句柄</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr DeleteObject(IntPtr objPtr);
        /// <summary>
        /// 删除专用设备场景或信息场景，释放所有相关窗口资源。不要将它用于GetDC函数取回的设备场景
        /// </summary>
        /// <param name="hdc">将要删除的设备场景</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr DeleteDC(IntPtr hdc);
        /// <summary>
        /// 将一幅位图从一个设备场景复制到另一个。源和目标DC相互间必须兼容
        /// </summary>
        /// <param name="hdcDest">目标设备场景</param>
        /// <param name="nXDest">对目标DC中目标矩形左上角位置进行描述的那个点。用目标DC的逻辑坐标表示</param>
        /// <param name="nYDest"></param>
        /// <param name="wDest">欲传输图象的宽度和高度</param>
        /// <param name="hDest"></param>
        /// <param name="hdcSource">源设备场景。如光栅运算未指定源，则应设为0</param>
        /// <param name="xSrc">对源DC中源矩形左上角位置进行描述的那个点。用源DC的逻辑坐标表示</param>
        /// <param name="ySrc"></param>
        /// <param name="rop">传输过程要执行的光栅运算</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int wDest, int hDest,
            IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

        #endregion
    }
}
