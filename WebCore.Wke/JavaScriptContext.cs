using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WebCore.Wke.JavaScript;

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

        private long _funCreateObject = 0;

        private long _funCreateComObject = 0;

        private long _funCreateControl = 0;

        private long _funChangeControl = 0;

        private long _funLoadLibrary = 0;

        public JavaScriptContext(IntPtr webView)
        {
            _webView = webView;
            var es = WkeApi.wkeGlobalExec(webView);
            _funLoadLibrary = JSApi.JsCreateFunction(es, Browser.Current._loadLib);
            _funCreateObject = JSApi.JsCreateFunction(es, Browser.Current._createObj);
            _funCreateComObject= JSApi.JsCreateFunction(es, Browser.Current._createComObj);
            _funCreateControl= JSApi.JsCreateFunction(es, Browser.Current._createControl);
            _funChangeControl= JSApi.JsCreateFunction(es, Browser.Current._changeControl);
            JSApi.wkeJSSetGlobal(es, "_loadAssembly", _funLoadLibrary);
            JSApi.wkeJSSetGlobal(es, "_createObject", _funCreateObject);
            JSApi.wkeJSSetGlobal(es, "_createComObject", _funCreateComObject);
            JSApi.wkeJSSetGlobal(es, "_createControl", _funCreateControl);
            JSApi.wkeJSSetGlobal(es, "_changeControl", _funChangeControl);
            JSApi.wkeJSAddRef(es, _funLoadLibrary);
            JSApi.wkeJSAddRef(es, _funCreateObject);
            JSApi.wkeJSAddRef(es, _funCreateComObject);
            JSApi.wkeJSAddRef(es, _funCreateControl);
            JSApi.wkeJSAddRef(es, _funChangeControl);
        }

        public void GC_Collect()
        {
            JSGC.Current.Collect();
        }


        public void Dispose()
        {
            var es = WkeApi.wkeGlobalExec(_webView);
            JSApi.wkeJSReleaseRef(es, _funLoadLibrary);
            JSApi.wkeJSReleaseRef(es, _funCreateObject);
            JSApi.wkeJSReleaseRef(es, _funCreateComObject);
            JSApi.wkeJSReleaseRef(es, _funCreateControl);
            JSApi.wkeJSReleaseRef(es, _funChangeControl);
            JSApi.wkeJSCollectGarbge();
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
