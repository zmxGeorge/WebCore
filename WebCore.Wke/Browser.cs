using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace WebCore.Wke
{
   

    public class Browser
    {
        private static readonly Browser _browser = new Browser();

        /// <summary>
        /// 获取当前对象实例
        /// </summary>
        public static Browser Current { get { return _browser; } }

        public void Application_Init()
        {
            WkeApi.wkeInitialize();
            wkeSettings settings = new wkeSettings();
            settings.mask = 2;
            settings.cookieFilePath = new char[1024];
            "cookies.dat".ToCharArray().CopyTo(settings.cookieFilePath, 0);
            var size = Marshal.SizeOf(settings);
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(settings, ptr, true);
            WkeApi.wkeConfigure(ptr);
            Marshal.FreeHGlobal(ptr);
            #region 加载扩展库
            //DirectoryInfo extDir = new DirectoryInfo("ext");
            //extDir.Refresh();
            //if (extDir.Exists)
            //{
            //    var files = extDir.GetFiles("*.dll");
            //    foreach (var item in files)
            //    {
            //        using (var stream = item.OpenRead())
            //        {
            //        }
            //    }
            //}
            #endregion
        }

        public void Application_Close()
        {
            WkeApi.wkeFinalize();
        }


    }
}
