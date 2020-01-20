using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace WebCore.Wke.JavaScript
{
    /// <summary>
    /// JS对象垃圾回收器
    /// </summary>
    public class JSGC
    {
        private static readonly JSGC _gc = new JSGC();

        /// <summary>
        /// 获取当前垃圾回收器
        /// </summary>
        public static JSGC Current { get { return _gc; } }

        private readonly Semaphore _lock = new Semaphore(1, 1);

        private static readonly Dictionary<IntPtr, List<WkeObjectRef>> _objDic = new Dictionary<IntPtr, List<WkeObjectRef>>();

        public void AddRef(IntPtr es, WkeObjectRef obj)
        {
            var webView = JSApi.wkeJSGetWebView(es);
            JSApi.wkeJSAddRef(es, obj.JsValue);
            lock (_objDic)
            {
                if (!_objDic.ContainsKey(webView))
                {
                    _objDic[webView] = new List<WkeObjectRef>();
                }
                _objDic[webView].Add(obj);
            }
        }

        public void CollectObject(IntPtr webView)
        {
            try
            {
                lock (_objDic)
                {
                    if (_objDic.ContainsKey(webView))
                    {
                        var es = WkeApi.wkeGlobalExec(webView);
                        var list = _objDic[webView];
                        foreach (var item in list)
                        {
                            JSApi.wkeJSReleaseRef(es, item.JsValue);
                        }
                        list.Clear();
                        _objDic.Remove(webView);
                    }
                }
            }
            finally
            {
                JSApi.wkeJSCollectGarbge();
            }

        }

        /// <summary>
        /// 调用GC回收
        /// </summary>
        public void Collect()
        {
            try
            {
                _lock.WaitOne();
                JSApi.wkeJSCollectGarbge();
            }
            finally
            {
                _lock.Release();
            }
        }

    }
}
