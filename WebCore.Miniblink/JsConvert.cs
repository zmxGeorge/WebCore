using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using WebCore.Miniblink.Csharp;

namespace WebCore.Miniblink
{
    public static class JsConvert
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


        public static object ConvertJSToObject(IntPtr es, long v,
            Type objType)
        {
            var pt = MBApi.jsTypeOf(v);
            if (pt == jsType.UNDEFINED)
            {
                return null;
            }
            else if (pt==jsType.FUNCTION)
            {
                string funData = ExtApi.GetJsString(es, v);
                //TODO:创建动态委托
                return FunctionCreater.CreateJsFunctionCallBack(MBApi.jsGetWebView(es),
                    funData, objType);
            }
            else if (pt == jsType.BOOLEAN)
            {
                return MBApi.jsToBoolean(es, v);
            }
            else if (pt == jsType.NUMBER)
            {
                return Convert.ChangeType(MBApi.jsToDouble(es, v), objType);
            }
            else if (pt == jsType.STRING)
            {
                return ExtApi.GetJsString(es, v);
            }
            else if (MBApi.jsIsArray(v))
            {
                var eType = objType.GetElementType();
                var len = MBApi.jsGetLength(es, v);
                var args = Array.CreateInstance(eType, len);
                for (int i = 0; i < len; i++)
                {
                    var argV = ConvertJSToObject(es, MBApi.jsGetAt(es, v, i), eType);
                    args.SetValue(argV, i);
                }
                return args;
            }
            else if (pt == jsType.OBJECT)
            {
                var base_obj = Activator.CreateInstance(objType);
                var pInfos = objType.GetProperties();
                var dicFun = MBApi.jsEvalW(es, SCRIPT_FUN);
                //将object转化成{name,value}的数组
                var dv = MBApi.jsCallGlobal(es, (long)dicFun, new long[] { v }, 1);
                var dLen = MBApi.jsGetLength(es, dv);
                for (int i = 0; i < dLen; i++)
                {
                    var item = MBApi.jsGetAt(es, dv, i);
                    var name = ExtApi.GetJsString(es, MBApi.jsGet(es, item, NAME_ATTR));
                    PropertyInfo pInfo = null;
                    if (pInfos.Length > 0)
                    {
                        foreach (var x in pInfos)
                        {
                            if (x.Name.ToLower() == name.ToLower())
                            {
                                pInfo = x;
                                break;
                            }
                        }
                    }
                    if (pInfo != null)
                    {
                        var js_obj = MBApi.jsGet(es, item, VALUE_ATTR);
                        var obj = ConvertJSToObject(es, js_obj, pInfo.PropertyType);
                        pInfo.SetValue(base_obj, obj, null);
                    }
                }
                return base_obj;
            }
            else
            {
                return null;
            }
        }

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
                return MBApi.jsNull();
            }
            long resIndex = MBApi.jsUndefined();
            var type = obj.GetType();
            if (type == typeof(byte[]))
            {
                var strData = Convert.ToBase64String((byte[])obj);
                resIndex = ExtApi.SetJsTempString(es, strData);
            }
            else if (type.IsSubclassOf(typeof(Array)))
            {
                var arr = obj as Array;
                resIndex = MBApi.jsEmptyArray(es);
                MBApi.jsSetLength(es, resIndex, arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    MBApi.jsSetAt(es, resIndex, i,
                        ConvertObjectToJS(es, arr.GetValue(i)));
                }
            }
            else if (type == typeof(string))
            {
                resIndex = ExtApi.SetJsTempString(es, obj.ToString());
            }
            else if (type == typeof(DateTime))
            {
                DateTime curTime = (DateTime)obj;
                resIndex = MBApi.jsDouble((curTime - DateTime.MinValue).TotalMilliseconds);
            }
            else if (type == typeof(int) ||
                type == typeof(uint) || type == typeof(short) ||
                type == typeof(ushort) || type == typeof(byte) ||
                type == typeof(sbyte))
            {
                var curValue = (int)obj;
                resIndex = MBApi.jsInt(curValue);
            }
            else if (type == typeof(double) ||
                type == typeof(decimal) ||
                type == typeof(long) ||
                type == typeof(ulong)
                )
            {
                var curValue = (double)obj;
                resIndex = MBApi.jsDouble(curValue);
            }
            else if (type == typeof(float))
            {
                var curValue = (float)obj;
                resIndex = MBApi.jsFloat(curValue);
            }
            else if (type == typeof(bool))
            {
                var curValue = (bool)obj;
                resIndex = MBApi.jsBoolean(curValue);
            }
            else
            {
                resIndex = MBApi.jsEmptyObject(es);
                var pInfos = type.GetProperties();
                //将实例中所有保护值类型属性赋值
                foreach (var pInfo in pInfos)
                {
                    var val = pInfo.GetValue(obj, null);
                    if (val != null && val.GetType().IsValueType)
                    {
                        MBApi.jsSet(es, resIndex, pInfo.Name, ConvertObjectToJS(es, val));
                    }
                }
            }
            return resIndex;
        }
    }
}
