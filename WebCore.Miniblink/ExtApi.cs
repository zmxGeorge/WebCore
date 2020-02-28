using MiniBlinkPinvokeVIP.Core;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WebCore.Miniblink
{
    public struct STRINGINFO
    {
	    public int strLen;
	    public IntPtr strPtr;
    }

    public static class ExtApi
    {
        /// <summary>
        /// 字符串编码转换
        /// </summary>
        /// <param name="strPtr"></param>
        /// <returns></returns>
        [DllImport("core/ex.dll",CallingConvention =CallingConvention.StdCall)]
        public static extern STRINGINFO GetStringInfo(IntPtr strPtr);

        public static string GetJsString(IntPtr es,long v)
        {
            var strPtr= MBApi.jsToTempStringW(es, v);
            string str = Marshal.PtrToStringUni(strPtr);
            return str;
        }

        public static string GetWkeString(IntPtr wkePtr)
        {
            var strPtr = MBApi.wkeGetString(wkePtr);
            var strInfo = ExtApi.GetStringInfo(strPtr);
            byte[] data = new byte[strInfo.strLen];
            Marshal.Copy(strInfo.strPtr, data, 0, data.Length);
            return Encoding.UTF8.GetString(data);
        }

        public static IntPtr SetWkeString(string str)
        {
            return MBApi.wkeCreateStringW(str, Encoding.UTF8.GetByteCount(str));
        }

        public static long SetJsTempString(IntPtr es, string str)
        {
            var ptr = Common.Utf8StringToIntptr(str);
            var v = MBApi.jsString(es, ptr);
            Marshal.FreeHGlobal(ptr);
            return v;
        }
    }
}
