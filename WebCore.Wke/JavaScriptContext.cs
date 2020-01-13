using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebCore.Wke
{
    public class JavaScriptContext:IDisposable
    {
        private IntPtr _webView = IntPtr.Zero;

        /// <summary>
        /// 获取相关句柄
        /// </summary>
        public IntPtr Handle { get { return WkeApi.wkeGlobalExec(_webView); } }


        /// <summary>
        /// 获取相关的web页面句柄
        /// </summary>
        public IntPtr WebView { get { return _webView; } }

        public JavaScriptContext(IntPtr webView)
        {
            _webView = webView;
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// 执行JS
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public long RunJs(string script)
        {
            return WkeApi.wkeRunJS(_webView, script);
        }

    }
}
