using System;
using System.Collections.Generic;
using System.Linq;
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

        public JavaScriptContext(IntPtr webView)
        {
            _webView = webView;
        }

        public void GC_Collect()
        {
            JSGC.Current.Collect();
        }

        private readonly List<long> _lockValue = new List<long>();

        private readonly Semaphore _lockV = new Semaphore(1, 1);

        /// <summary>
        /// 锁定JS值不被垃圾回收
        /// </summary>
        /// <param name="v"></param>
        public void LockJSObject(long v)
        {
            JSApi.wkeJSAddRef(Handle, v);
        }

        /// <summary>
        /// 解锁被锁定的值
        /// </summary>
        /// <param name="v"></param>
        public void UnLockJSObject(long v)
        {
            JSApi.wkeJSReleaseRef(Handle, v);
        }

        public void LockCollect()
        {
            try
            {
                _lockV.WaitOne();
                var es = WkeApi.wkeGlobalExec(_webView);
                foreach (var item in _lockValue)
                {
                    JSApi.wkeJSReleaseRef(es, item);
                }
                _lockValue.Clear();
            }
            finally
            {
                _lockV.Release();
            }
        }

        public void Dispose()
        {
            LockCollect();
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
