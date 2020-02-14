using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WebCore.Wke.Csharp;

namespace WebCore.Wke.JavaScript
{
    public static class JSConvert
    {
        private const string SCRIPT_FUN = @"
        return function(e) {
        var arr = [];
        var index = 0;
        if (e == undefined ||
            e == null) {
            return arr;
        }
        for (var key in e) {
        if (typeof (e[key]) != 'function' &&
            typeof (e[key]) != 'object') {
            arr[index] = {
                name: key,
                value: e[key]
                };
            }
        }
        return arr;
         };
        ";

        private const string NAME_ATTR = "name";

        private const string VALUE_ATTR = "value";

        /// <summary>
        /// 将JS对象转换为C#对象
        /// </summary>
        /// <param name="es"></param>
        /// <param name="v"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static object ConvertJSToObject(IntPtr es, long v,Type objType)
        {
            if (JSApi.wkeJSIsUndefined(es, v))
            {
                return null;
            }
            else if (JSApi.wkeJSIsNull(es, v))
            {
                return null;
            }
            else if (JSApi.wkeJSIsFunction(es, v))
            {
                var jsData = JSHelper.GetJsString(es, v);
                var webView = JSApi.wkeJSGetWebView(es);
                return FunctionCreater.CreateJsFunctionCallBack(Form.ActiveForm.Handle,webView,
                    jsData, objType);
            }
            else if (JSApi.wkeJSIsBool(es, v))
            {
                return JSApi.wkeJSToBool(es, v);
            }
            else if (JSApi.wkeJSIsNumber(es, v))
            {
                return Convert.ChangeType(JSApi.wkeJSToDouble(es, v),objType);
            }
            else if (JSApi.wkeJSIsString(es, v))
            {
                return JSHelper.GetJsString(es, v);
            }
            else if (JSApi.wkeJSIsArray(es, v))
            {
                var eType = objType.GetElementType();
                var len = JSApi.wkeJSGetLength(es, v);
                var args =Array.CreateInstance(eType,len);
                for (int i = 0; i < len; i++)
                {
                    var argV = ConvertJSToObject(es, JSApi.wkeJSGetAt(es, v, i), eType);
                    args.SetValue(argV, i);
                }
                return args;
            }
            else if (JSApi.wkeJSIsObject(es, v))
            {
                var base_obj = Activator.CreateInstance(objType);
                var pInfos = objType.GetProperties();
                var dicFun = JSApi.wkeJSEval(es, SCRIPT_FUN);
                //将object转化成{name,value}的数组
                var dv = JSApi.wkeJSCallGlobal(es, (long)dicFun, new long[] { v }, 1);
                var dLen = JSApi.wkeJSGetLength(es, dv);
                for (int i = 0; i < dLen; i++)
                {
                    var item = JSApi.wkeJSGetAt(es, dv, i);
                    var name = JSHelper.GetJsString(es, JSApi.wkeJSGet(es, item, NAME_ATTR));
                    var pInfo = pInfos.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                    if (pInfo != null)
                    {
                        var js_obj = JSApi.wkeJSGet(es, item, VALUE_ATTR);
                        var obj = ConvertJSToObject(es, js_obj, pInfo.PropertyType);
                        pInfo.SetValue(base_obj, obj, null);
                    }
                }
                return base_obj;
            }
            return null;
        }

        private const string DEF_TEMP_ARR = "window.TempArr=function(){return [];};";

        private const string TEMPARRAY = "TempArr";


        /// <summary>
        /// 将Object对象转换为JS对象
        /// </summary>
        /// <param name="es"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ConvertObjectToJS(IntPtr es, object obj)
        {
            if (obj == null)
            {
                return JSApi.wkeJSNull(es);
            }
            long resIndex = JSApi.wkeJSUndefined(es);
            var type = obj.GetType();
            if (type == typeof(byte[]))
            {
                var strData =Encoding.UTF8.GetBytes(Convert.ToBase64String((byte[])obj));
                var strPtr = Marshal.UnsafeAddrOfPinnedArrayElement(strData, 0);
                resIndex =JSApi.wkeJSString(es, strPtr);
            }
            else if (type.IsSubclassOf(typeof(Array)))
            {
                var arr = obj as Array;
                resIndex = JSApi.wkeJSEmptyArray(es);
                JSApi.wkeJSSetLength(es, resIndex, arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    JSApi.wkeJSSetAt(es, resIndex, i,
                        ConvertObjectToJS(es, arr.GetValue(i)));
                }
            }
            else if (type == typeof(string))
            {
                var strData = Encoding.UTF8.GetBytes(obj.ToString());
                var strPtr = Marshal.UnsafeAddrOfPinnedArrayElement(strData, 0);
                resIndex = JSApi.wkeJSString(es, strPtr);
            }
            else if (type == typeof(DateTime))
            {
                DateTime curTime = (DateTime)obj;
                resIndex = JSApi.wkeJSDouble(es,(curTime - DateTime.MinValue).TotalMilliseconds);
            }
            else if (type == typeof(int)||
                type == typeof(uint)|| type == typeof(short) ||
                type == typeof(ushort) || type == typeof(byte) ||
                type == typeof(sbyte))
            {
                var curValue = (int)obj;
                resIndex = JSApi.wkeJSInt(es,curValue);
            }
            else if (type == typeof(double) ||
                type == typeof(decimal)||
                type == typeof(long)||
                type == typeof(ulong)
                )
            {
                var curValue = (double)obj;
                resIndex = JSApi.wkeJSDouble(es,curValue);
            }
            else if (type == typeof(float))
            {
                var curValue = (float)obj;
                resIndex = JSApi.wkeJSFloat(es,curValue);
            }
            else if (type == typeof(bool))
            {
                var curValue = (bool)obj;
                resIndex = JSApi.wkeJSBool(es,curValue);
            }
            else
            {
                resIndex = JSApi.wkeJSEmptyObject(es);
                var pInfos = type.GetProperties();
                //将实例中所有保护值类型属性赋值
                foreach (var pInfo in pInfos)
                {
                    var val = pInfo.GetValue(obj, null);
                    if (val != null && val.GetType().IsValueType)
                    {
                        JSApi.wkeJSSet(es, resIndex, pInfo.Name, ConvertObjectToJS(es, val));
                    }
                }
            }
            return resIndex;
        }
    }
}
