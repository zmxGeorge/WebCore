using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using WebCore.Wke.JavaScript;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WebCore.Wke.Csharp
{
    public static class FunctionCreater
    {
        public class FunctionParamterCollection:List<object>
        {
            public void AddParamter<T>(T value)
            {
                base.Add(value);
            }

            public long[] ToJsValue(IntPtr es)
            {
                long[] res = new long[Count];
                int index = 0;
                foreach (var item in this)
                {
                    res[index]=JSConvert.ConvertObjectToJS(es, item);
                }
                return res;
            }
        }

        public static void CancelByView(IntPtr webView)
        {
            lock (_funsDic)
            {
                if (_funsDic.ContainsKey(webView))
                {
                    var list = _funsDic[webView];
                    foreach (var item in list)
                    {
                        Marshal.WriteByte(item.CancelPtr, 1);
                    }
                    list.Clear();
                    _funsDic.Remove(webView);
                }
            }
        }

        public class FunctionInfo
        {
            public IntPtr CancelPtr { get; set; }

            public Delegate Delegate { get; set; }

            public IntPtr WebView { get; set; }
        }

        private static readonly Dictionary<IntPtr, List<FunctionInfo>> _funsDic = new Dictionary<IntPtr, List<FunctionInfo>>();

        private static MethodInfo AddParmMethod = typeof(FunctionParamterCollection).
    GetMethod("AddParamter", BindingFlags.Instance | BindingFlags.Public);

        private static MethodInfo CallMethod = typeof(FunctionCreater).GetMethod("Call", BindingFlags.Static | BindingFlags.NonPublic);

        public static Delegate CreateJsFunctionCallBack(
            IntPtr formPtr,
            IntPtr webView,
            string jsFunctionData,
            Type delType
            )
        {
            var type = delType;
            if (!type.IsSubclassOf(typeof(Delegate)))
            {
                return null;
            }
            var delMethod = type.GetMethods()[0];
            var paramterInfos = delMethod.GetParameters();
            if (paramterInfos.Any(x => x.IsOut ||
             x.ParameterType.IsByRef))
            {
                return null;
            }
            var pTypes = paramterInfos.Select(x => x.ParameterType).ToArray();
            DynamicMethod dyMethod = new DynamicMethod(string.Empty, delMethod.ReturnType,
                pTypes, true);
            var cancelPtr = GetCancelPtr();
            var gen = dyMethod.GetILGenerator();
            var collBulider = gen.DeclareLocal(typeof(FunctionParamterCollection));
            gen.Emit(OpCodes.Newobj, typeof(FunctionParamterCollection).GetConstructor(Type.EmptyTypes));
            gen.Emit(OpCodes.Stloc, collBulider);
            for (int i = 0; i < pTypes.Length; i++)
            {
                var pType = pTypes[i];
                gen.Emit(OpCodes.Ldloc, collBulider);
                gen.Emit(OpCodes.Ldarg, i);
                gen.Emit(OpCodes.Call, AddParmMethod.MakeGenericMethod(pType));
            }
            gen.Emit(OpCodes.Ldc_I8, formPtr.ToInt64());
            gen.Emit(OpCodes.Ldc_I8, webView.ToInt64());
            gen.Emit(OpCodes.Ldc_I8, cancelPtr.ToInt64());
            gen.Emit(OpCodes.Ldstr, jsFunctionData);
            gen.Emit(OpCodes.Ldloc, collBulider);
            var dType = typeof(object);
            if (dyMethod.ReturnType != typeof(void))
            {
                dType = dyMethod.ReturnType;
            }
            gen.Emit(OpCodes.Call, CallMethod.MakeGenericMethod(dType));
            if (dyMethod.ReturnType == typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }
            gen.Emit(OpCodes.Ret);
            var del = dyMethod.CreateDelegate(type);
            lock (_funsDic)
            {
                FunctionInfo funInfo = new FunctionInfo {
                    WebView=webView,
                    CancelPtr=cancelPtr,
                    Delegate=del
                };
                if (!_funsDic.ContainsKey(funInfo.WebView))
                {
                    _funsDic[funInfo.WebView] = new List<FunctionInfo>();
                }
                _funsDic[funInfo.WebView].Add(funInfo);
            }
            return del;
        }

        /// <summary>
        /// 注册一个指针，可指示是否再需要调用委托的标志位
        /// </summary>
        /// <returns></returns>
        private static IntPtr GetCancelPtr()
        {
            var ptr = Marshal.AllocHGlobal(1);
            Marshal.WriteByte(ptr, 0);
            return ptr;
        }

        private const string FUNCTION_CALL_FORMAT = "window.webCoreCallFunction={0};";

        private const string FUNCTION_ATTR = "webCoreCallFunction";

        private static T Call<T>(
            long formPtr,
            long i_webView,
            long cancelPtr,
            string jsFunction,
            FunctionParamterCollection paramterCollection)
        {
            IntPtr wV = new IntPtr(i_webView);
            Form form = Form.FromHandle(new IntPtr(formPtr)) as Form;
            return (T)form.Invoke(new Func<IntPtr,T>(webView => {
                var ptr = new IntPtr(cancelPtr);
                try
                {
                    var tag = Marshal.ReadByte(ptr);
                    if (tag == 1)
                    {
                        return default(T);
                    }
                    var es = WkeApi.wkeGlobalExec(webView);
                    var args = paramterCollection.ToJsValue(es);
                    var script = string.Format(FUNCTION_CALL_FORMAT, jsFunction);
                    var funV = JSApi.wkeJSEval(es, script);
                    funV = JSApi.wkeJSGetGlobal(es, FUNCTION_ATTR);
                    if (JSApi.wkeJSIsUndefined(es, funV))
                    {
                        return default(T);
                    }
                    long resV = JSApi.wkeJSCallGlobal(es, funV, args, args.Length);
                    var obj = JSConvert.ConvertJSToObject(es, resV, typeof(T));
                    if (obj == null)
                    {
                        return default(T);
                    }
                    return (T)obj;
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                    JSGC.Current.Collect();
                }
            }), wV);
        }
    }
}
