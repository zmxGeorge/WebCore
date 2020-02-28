using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WebCore.Miniblink
{
    public class CSharpMethodInfo:IDisposable
    {
        private long _jsValue = 0;

        private object _obj = null;

        public long JsValue { get { return _jsValue; } }

        public string Name { get { return _methodInfo.Name; } }

        private MethodInfo _methodInfo = null;

        private IntPtr _webView = IntPtr.Zero;

        private jsCallAsFunctionCallback _methodCall = null;

        private jsFinalizeCallback _jsFinalizeCallback = null;

        public CSharpMethodInfo(IntPtr es,
            object obj,
            MethodInfo methodInfo)
        {
            _obj = obj;
            _webView = MBApi.jsGetWebView(es);
            _methodInfo = methodInfo;
            _methodCall = new jsCallAsFunctionCallback(OnMethodCall);
            _jsFinalizeCallback = new jsFinalizeCallback(OnMehtodDispose);
            jsData data = new jsData();
            data.typeName = new byte[100];
            data.finalize = _jsFinalizeCallback;
            data.callAsFunction = _methodCall;
            var s = Marshal.SizeOf(typeof(jsData));
            var ptr = Marshal.AllocHGlobal(s);
            Marshal.StructureToPtr(data, ptr, true);
            _jsValue = MBApi.jsFunction(es, ptr);
            MBApi.jsAddRef(es, _jsValue);
        }

        public void Dispose()
        {
            if (_jsValue > 0)
            {
                MBApi.jsReleaseRef(MBApi.wkeGlobalExec(_webView), _jsValue);
            }
        }

        private void OnMehtodDispose(IntPtr data)
        {
            Marshal.FreeHGlobal(data);
        }

        private const string VALUE_ATTR = "value";

        private long OnMethodCall(IntPtr es, long obj,
            IntPtr args, int argCount)
        {
            var paramters = _methodInfo.GetParameters();
            if (argCount < paramters.Length)
            {
                return MBApi.jsUndefined();
            }
            object[] param = new object[argCount];
            for (int i = 0; i < param.Length; i++)
            {
                var paramInfo = paramters[i];
                var pType = paramInfo.ParameterType;
                var tType = paramInfo.ParameterType.GetElementType();
                if (tType != null)
                {
                    pType = tType;
                }
                long t = MBApi.jsArg(es, i);
                param[i] = JsConvert.
                    ConvertJSToObject(es, t, pType);
            }
            var res = _methodInfo.Invoke(_obj, param);
            return JsConvert.ConvertObjectToJS(es, res);
        }
    }

    public class CSharpStore
    {
        private static readonly CSharpStore _store = new CSharpStore();

        public static CSharpStore Current { get { return _store; } }

        private readonly Dictionary<object, List<CSharpMethodInfo>> _methodDic = new Dictionary<object, List<CSharpMethodInfo>>();


        private const string ATTR_VALUE = "value";

        public long CreateJsObject(IntPtr es,object obj)
        {
            var t = obj.GetType();
            List<CSharpMethodInfo> listMethod = null;
            lock (_methodDic)
            {
                if (_methodDic.ContainsKey(obj))
                {
                    listMethod = _methodDic[obj];
                }
                else
                {
                    listMethod = new List<CSharpMethodInfo>();
                    var methods = t.GetMethods();
                    foreach (var m in methods)
                    {
                        listMethod.Add(new CSharpMethodInfo(es, obj, m));
                    }
                    _methodDic.Add(obj, listMethod);
                }
            }
            var jsObj = MBApi.jsEmptyObject(es);
            foreach (var ml in listMethod)
            {
                MBApi.jsSet(es, jsObj, ml.Name, ml.JsValue);
            }
            return jsObj;
        }

        public void DisposeWithObject(object obj)
        {
            lock (_methodDic)
            {
                if (_methodDic.ContainsKey(obj))
                {
                    _methodDic.Remove(obj);
                }
            }
            GC.Collect();
        }

        public void Close()
        {
            foreach (var item in _methodDic)
            {
                foreach (var m in item.Value)
                {
                    m.Dispose();
                }
                item.Value.Clear();
            }
            _methodDic.Clear();
            GC.Collect();
        }
    }
}
