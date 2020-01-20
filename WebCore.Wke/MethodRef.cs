using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebCore.Wke.JavaScript;

namespace WebCore.Wke
{
    public class MethodRef
    {
        private wkeJSCallAsFunctionCallback _jsMethod = null;

        private object _obj = null;

        private MethodInfo _method = null;

        public MethodRef(object obj, MethodInfo method)
        {
            _obj = obj;
            _method = method;
            _jsMethod = new wkeJSCallAsFunctionCallback(OnCallMethod);
            GC.SuppressFinalize(this);
        }

        public long ToValue(IntPtr es)
        {
            return JSApi.JsCreateFunction(es, _jsMethod);
        }

        private long OnCallMethod(IntPtr es,
            long obj,
            IntPtr args,
            int argCount)
        {
            try
            {
                object[] vl = new object[argCount];
                var paramters = _method.GetParameters();
                for (int i = 0; i < argCount; i++)
                {
                    var par = paramters[i];
                    var pType = par.ParameterType;
                    if (par.ParameterType.IsByRef)
                    {
                        pType = par.ParameterType.GetElementType();
                    }
                    vl[i] = JSConvert.ConvertJSToObject(es, JSApi.wkeJSParam(es, i), pType);
                }
                var res = _method.Invoke(_obj, vl);
                return JSConvert.ConvertObjectToJS(es, res);
            }
            finally
            {
                GC.KeepAlive(this);
                GC.ReRegisterForFinalize(this);
            }
        }
    }
}
