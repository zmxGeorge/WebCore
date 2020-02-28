using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;

namespace WebCore.Miniblink
{
    public class CSharpObject
    {
        private IntPtr _webView = IntPtr.Zero;

        private long _jsValue = 0;

        public long ScriptValue { get { return _jsValue; } }

        private object _obj = null;

        public object ObjectValue { get { return _obj; } }

        public CSharpObject(IntPtr webView, object obj)
        {
            _webView = webView;
            _obj = obj;
            var es = MBApi.wkeGlobalExec(_webView);
            _jsValue = CSharpStore.Current.CreateJsObject(es, obj);
        }

        public void Dispose()
        {
            CSharpStore.Current.DisposeWithObject(_obj);
            GC.Collect(0);
        }

        ~CSharpObject()
        {
            
        }
    }
}
