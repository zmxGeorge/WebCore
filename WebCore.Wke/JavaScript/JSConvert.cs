using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebCore.Wke.JavaScript
{
    public static class JSConvert
    {
        /// <summary>
        /// 将JS对象转换为C#对象
        /// </summary>
        /// <param name="es"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static object ConvertJSToObject(IntPtr es, long v)
        {
            return null;
        }

        /// <summary>
        /// 将Object对象转换为JS对象
        /// </summary>
        /// <param name="es"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ConvertObjectToJS(IntPtr es, object obj)
        {
            return 0;
        }
    }
}
