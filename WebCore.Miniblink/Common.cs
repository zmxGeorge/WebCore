using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlinkPinvokeVIP.Core
{
    public class Common
    {
        public static IntPtr Utf8StringToIntptr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return IntPtr.Zero;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, intPtr, bytes.Length);
            Marshal.WriteByte(intPtr, bytes.Length, 0);
            return intPtr;
        }

    }
}
