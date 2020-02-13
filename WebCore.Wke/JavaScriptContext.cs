using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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

        private long _funDownLoad = 0;

        private wkeJSCallAsFunctionCallback _createControl = null;

        private wkeJSCallAsFunctionCallback _changeControl = null;

        private WebView _view=null;

        public JavaScriptContext(WebView view, IntPtr webView)
        {
            _view = view;
            _webView = webView;
            var es = WkeApi.wkeGlobalExec(webView);
            _createControl = new wkeJSCallAsFunctionCallback(CreateControl);
            _changeControl = new wkeJSCallAsFunctionCallback(ChangeControl);
            _funLoadLibrary = JSApi.JsCreateFunction(es, Browser.Current._loadLib);
            _funCreateObject = JSApi.JsCreateFunction(es, Browser.Current._createObj);
            _funCreateComObject= JSApi.JsCreateFunction(es, Browser.Current._createComObj);
            _funCreateControl= JSApi.JsCreateFunction(es, _createControl);
            _funChangeControl= JSApi.JsCreateFunction(es, _changeControl);
            _funDownLoad= JSApi.JsCreateFunction(es, Browser.Current._downLoadURL);
            JSApi.wkeJSAddRef(es, _funLoadLibrary);
            JSApi.wkeJSAddRef(es, _funCreateObject);
            JSApi.wkeJSAddRef(es, _funCreateComObject);
            JSApi.wkeJSAddRef(es, _funCreateControl);
            JSApi.wkeJSAddRef(es, _funChangeControl);
            JSApi.wkeJSAddRef(es, _funDownLoad);
            JSApi.wkeJSSetGlobal(es, "_loadAssembly", _funLoadLibrary);
            JSApi.wkeJSSetGlobal(es, "_createObject", _funCreateObject);
            JSApi.wkeJSSetGlobal(es, "_createComObject", _funCreateComObject);
            JSApi.wkeJSSetGlobal(es, "_createControl", _funCreateControl);
            JSApi.wkeJSSetGlobal(es, "_changeControl", _funChangeControl);
            JSApi.wkeJSSetGlobal(es, "_downLoadURL", _funDownLoad);
        }

        private long ChangeControl(IntPtr es, long obj, IntPtr args, int argCount)
        {
            if (argCount != 5)
            {
                return JSApi.wkeJSUndefined(es);
            }
            var vPtr= JSApi.wkeJSParam(es, 0);
            var vX = JSApi.wkeJSParam(es, 1);
            var vY = JSApi.wkeJSParam(es, 2);
            var vWidth = JSApi.wkeJSParam(es, 3);
            var vHeight = JSApi.wkeJSParam(es, 4);
            IntPtr controlPtr = IntPtr.Zero;
            if (JSApi.wkeJSIsString(es, vPtr))
            {
                var strPtr = JSHelper.GetJsString(es, vPtr);
                int n = 0;
                if (!int.TryParse(strPtr, out n))
                {
                    return JSApi.wkeJSUndefined(es);
                }
                controlPtr = new IntPtr(n);
            }
            else if (JSApi.wkeJSIsNumber(es, vPtr))
            {
                var ptr = JSApi.wkeJSToInt(es, vPtr);
                controlPtr = new IntPtr(ptr);
            }
            else
            {
                return JSApi.wkeJSUndefined(es);
            }
            NativeControl control = NativeControl.FromHandle(controlPtr) as NativeControl;
            if (control == null)
            {
                return JSApi.wkeJSUndefined(es);
            }
            int x, y, width, height = 0;
            if (!JSApi.wkeJSIsNumber(es, vX) ||
               !JSApi.wkeJSIsNumber(es, vY) ||
               !JSApi.wkeJSIsNumber(es, vWidth) ||
               !JSApi.wkeJSIsNumber(es, vHeight))
            {
                return JSApi.wkeJSUndefined(es);
            }
            x = JSApi.wkeJSToInt(es, vX);
            y = JSApi.wkeJSToInt(es, vY);
            width = JSApi.wkeJSToInt(es, vWidth);
            height = JSApi.wkeJSToInt(es, vHeight);
            control.SetBounds(x, y, width, height);
            return JSApi.wkeJSTrue(es);
        }

        private long CreateControl(IntPtr es, long obj, IntPtr args, int argCount)
        {
            if (argCount != 4)
            {
                return JSApi.wkeJSUndefined(es);
            }
            var vX = JSApi.wkeJSParam(es, 0);
            var vY = JSApi.wkeJSParam(es, 1);
            var vWidth = JSApi.wkeJSParam(es, 2);
            var vHeight = JSApi.wkeJSParam(es, 3);
            int x, y, width, height = 0;
            if (!JSApi.wkeJSIsNumber(es, vX) ||
               !JSApi.wkeJSIsNumber(es, vY) ||
               !JSApi.wkeJSIsNumber(es, vWidth) ||
               !JSApi.wkeJSIsNumber(es, vHeight))
            {
                return JSApi.wkeJSUndefined(es);
            }
            x = JSApi.wkeJSToInt(es, vX);
            y = JSApi.wkeJSToInt(es, vY);
            width = JSApi.wkeJSToInt(es, vWidth);
            height = JSApi.wkeJSToInt(es, vHeight);
            NativeControl control = new NativeControl();
            control.Location = new Point(x, y);
            control.Width = width;
            control.Height = height;
            _view.Controls.Add(control);
            control.BringToFront();
            var handle = control.Handle.ToInt32();
            return JSApi.wkeJSInt(es,handle);
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
            JSApi.wkeJSReleaseRef(es, _funDownLoad);
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
