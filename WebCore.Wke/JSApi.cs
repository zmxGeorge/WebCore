using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WebCore.Wke
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long wkeJSGetPropertyCallback(IntPtr es, long obj, string propertyName);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool wkeJSSetPropertyCallback(IntPtr es, long obj, string propertyName, long value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long wkeJSCallAsFunctionCallback(IntPtr es, long obj, IntPtr args, int argCount);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeJSFinalizeCallback(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long wkeJSNativeFunction(IntPtr es);

    public enum wkeJSType
    {
        JSTYPE_NUMBER,
        JSTYPE_STRING,
        JSTYPE_BOOLEAN,
        JSTYPE_OBJECT,
        JSTYPE_FUNCTION,
        JSTYPE_UNDEFINED,
    }

    [StructLayout(LayoutKind.Sequential,Pack =1)]
    public struct wkeJSData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public char[] typeName;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public wkeJSGetPropertyCallback propertyGet;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public wkeJSSetPropertyCallback propertySet;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public wkeJSFinalizeCallback finalize;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public wkeJSCallAsFunctionCallback callAsFunction;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JS_StringInfo
    {
        public int strLen;

        public IntPtr strPtr;
    }

    public static class JSApi
    {
        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  void  wkeJSBindFunction(string name, wkeJSNativeFunction fn, uint argCount);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  void  wkeJSBindGetter(string name, wkeJSNativeFunction fn); /*get property*/

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  void  wkeJSBindSetter(string name, wkeJSNativeFunction fn); /*set property*/

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  int   wkeJSParamCount(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  wkeJSType  wkeJSParamType(IntPtr es, int index);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSParam(IntPtr es, int index);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  wkeJSType  wkeJSTypeOf(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsNumber(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsString(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsBool(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsObject(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsFunction(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsUndefined(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsNull(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsArray(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsTrue(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSIsFalse(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  int   wkeJSToInt(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  float wkeJSToFloat(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  double wkeJSToDouble(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  bool  wkeJSToBool(IntPtr es, long v);

        [DllImport("core/wke.dll", CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern  IntPtr wkeJSToTempStringW(IntPtr es, long v);

        [DllImport("core/wke.dll", EntryPoint = "wkeJSToTempString", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkeJSToTempStringPtr(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSInt(IntPtr es, int n);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSFloat(IntPtr es, float f);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSDouble(IntPtr es, double d);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSBool(IntPtr es, bool b);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSUndefined(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSNull(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSTrue(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSFalse(IntPtr es);

        [DllImport("core/wke.dll", CharSet =CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSString(IntPtr es, string str);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSEmptyObject(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern  long wkeJSEmptyArray(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long  wkeJSObject(IntPtr es, IntPtr obj);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64  wkeJSFunction(IntPtr es, IntPtr jdData);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr   wkeJSGetData(IntPtr es, long obj);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long   wkeJSGet(IntPtr es, long obj, string prop);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void  wkeJSSet(IntPtr es, long obj, string prop, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long   wkeJSGetAt(IntPtr es, long obj, int index);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void  wkeJSSetAt(IntPtr es, long obj, int index, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int  wkeJSGetLength(IntPtr es, long obj);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void  wkeJSSetLength(IntPtr es, long obj, int length);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long   wkeJSGlobalObject(IntPtr es);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr  wkeJSGetWebView(IntPtr es);

        [DllImport("core/wke.dll", CharSet =CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern long wkeJSEval(IntPtr es, string str);

        [DllImport("core/wke.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern long wkeJSEvalW(IntPtr es, string str);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long  wkeJSCall(IntPtr es, long func, long thisObj,long[] args, int argCount);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long  wkeJSCallGlobal(IntPtr es, long func, long[] args, int argCount);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long  wkeJSGetGlobal(IntPtr es, string prop);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void  wkeJSSetGlobal(IntPtr es, string prop, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void   wkeJSAddRef(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void   wkeJSReleaseRef(IntPtr es, long v);

        [DllImport("core/wke.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeJSCollectGarbge();

        [DllImport("core/wkeJavaScript.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void Test();

        [DllImport("core/wkeJavaScript.dll",CallingConvention = CallingConvention.StdCall)]
        public static extern JS_StringInfo GetTempJavaScriptString(IntPtr es,long v);

        [DllImport("core/wkeJavaScript.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern long JsCreateFunction(IntPtr es,wkeJSCallAsFunctionCallback callBack);

        [DllImport("core/wkeJavaScript.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern long JsCreateObject(IntPtr es, wkeJSGetPropertyCallback getter,
            wkeJSSetPropertyCallback setter,
            wkeJSCallAsFunctionCallback callBack,
            wkeJSFinalizeCallback finalCallBack);
    }

    public static class JSHelper
    {
        public static string GetJsString(IntPtr es, long v)
        {
            var strInfo = JSApi.GetTempJavaScriptString(es, v);
            byte[] dat = new byte[strInfo.strLen];
            Marshal.Copy(strInfo.strPtr, dat, 0, dat.Length);
            return Encoding.UTF8.GetString(dat);
        }

    }
}
