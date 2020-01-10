using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebCore.Wke
{
    /// <summary>
    /// HTML文档加载完成时发生
    /// </summary>
    /// <param name="view"></param>
    /// <param name="url"></param>
    public delegate void OnDocumentReady(WebView view,string url);

    /// <summary>
    /// URL加载失败
    /// </summary>
    /// <param name="view"></param>
    /// <param name="message"></param>
    public delegate void OnLoadingFail(WebView view,string url,string message);

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
        }

        public void Application_Close()
        {
            WkeApi.wkeFinalize();
        }


    }
}
