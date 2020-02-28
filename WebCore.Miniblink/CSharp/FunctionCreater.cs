using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WebCore.Miniblink.Csharp
{
    public delegate TResult Func<T, TResult>(T param);

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
                    res[index]= JsConvert.ConvertObjectToJS(es, item);
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
            IntPtr webView,
            string jsFunctionData,
            Type delType
            )
        {
            IntPtr controlViewHandle =Control.FromChildHandle(MBApi.wkeGetWindowHandle(webView)).Handle;
            var type = delType;
            if (!type.IsSubclassOf(typeof(Delegate)))
            {
                return null;
            }
            var delMethod = type.GetMethods()[0];
            var paramterInfos = delMethod.GetParameters();
            bool find = false;
            foreach (var item in paramterInfos)
            {
                if (item.IsOut ||
                    item.ParameterType.IsByRef)
                {
                    find = true;
                    break;
                }
            }
            if (find)
            {
                return null;
            }
            Type[] pTypes = new Type[paramterInfos.Length];
            for (int i = 0; i < paramterInfos.Length; i++)
            {
                pTypes[i] = paramterInfos[i].ParameterType;
            }
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
            gen.Emit(OpCodes.Ldc_I8, webView.ToInt64());
            gen.Emit(OpCodes.Ldc_I8, controlViewHandle.ToInt64());
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

        private const string FUNCTION_CALL_FORMAT = "return {0};";

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static T Call<T>(
            long i_webView,
            long i_control,
            long cancelPtr,
            string jsFunction,
            FunctionParamterCollection paramterCollection)
        {
            IntPtr controlPtr = new IntPtr(i_control);
            Control control = Control.FromHandle(controlPtr);
            return (T)control.Invoke(new Func<long,T>(w => {
                try
                {
                    IntPtr wV = new IntPtr(w);
                    var es = MBApi.wkeGlobalExec(wV);
                    var args = paramterCollection.ToJsValue(es);
                    var script = string.Format(FUNCTION_CALL_FORMAT, jsFunction);
                    var funV = MBApi.jsEvalW(es, script);
                    if (MBApi.jsIsUndefined(funV))
                    {
                        return default(T);
                    }
                    long resV = MBApi.jsCallGlobal(es, funV, args, args.Length);
                    var obj = JsConvert.ConvertJSToObject(es, resV, typeof(T));
                    if (obj == null)
                    {
                        return default(T);
                    }
                    return (T)obj;
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }), i_webView);
            
        }
    }
}
