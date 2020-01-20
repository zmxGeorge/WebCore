using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using WebCore.Wke.JavaScript;

namespace WebCore.Wke
{
    public class WkeObjectRef
    {
        public long JsValue { get { return _jsValue; } }

        public object Object { get { return _obj; } }

        public bool IsComObject { get { return _isComObject; } }

        private long _jsValue = 0;

        private bool _isComObject = false;

        private object _obj = null;

        private Type _callType = null;

        private wkeJSGetPropertyCallback _getter = null;

        private wkeJSSetPropertyCallback _setter = null;

        private wkeJSCallAsFunctionCallback _callBack = null;

        private wkeJSFinalizeCallback _finalizeCallBack = null;



        public WkeObjectRef(
            IntPtr es,
            object obj, 
            Type callType,
            bool isComObject)
        {
            _obj = obj;
            _callType = callType;
            _isComObject = isComObject;
            _getter = new wkeJSGetPropertyCallback(ObjectGetter);
            _setter = new wkeJSSetPropertyCallback(ObjectSetter);
            _callBack = new wkeJSCallAsFunctionCallback(OnFunctionCallBack);
            _finalizeCallBack = new wkeJSFinalizeCallback(OnDisposed);
            _jsValue=JSApi.JsCreateObject(es, _getter, _setter, _callBack, _finalizeCallBack);
            JSGC.Current.AddRef(es, this);

        }

        private void OnDisposed(IntPtr data)
        {
            if (_obj != null)
            {
                if (_isComObject)
                {
                    Marshal.FinalReleaseComObject(_obj);
                }
            }
            Marshal.FreeHGlobal(data);
        }

        private long OnFunctionCallBack(IntPtr es, long obj, IntPtr args, int argCount)
        {
            return JSApi.wkeJSUndefined(es);
        }

        private bool ObjectSetter(IntPtr es, long obj, string propertyName, long value)
        {
            var cType = _callType;
            if (cType == null)
            {
                cType = _obj.GetType();
            }
            var pInfo = cType.GetProperty(propertyName, BindingFlags.Instance |
    BindingFlags.Public | BindingFlags.IgnoreCase|BindingFlags.SetProperty);
            if (pInfo != null)
            {
                var v = JSConvert.ConvertJSToObject(es, value, pInfo.PropertyType);
                pInfo.SetValue(_obj, v, null);
                return true;
            }
            return false;
        }

        private long ObjectGetter(IntPtr es, long obj, string propertyName)
        {
            var cType = _callType;
            if (cType == null)
            {
                cType = _obj.GetType();
            }
            var members = cType.GetMember(propertyName, BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.IgnoreCase);
            if (members != null&&members.Length>0)
            {
                var member = members.FirstOrDefault();
                if (member.MemberType == MemberTypes.Method)
                {
                    var method = member as MethodInfo;
                    MethodRef m = new MethodRef(_obj, method);
                    return m.ToValue(es);
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    var property = member as PropertyInfo;
                    var v = property.GetValue(_obj, null);
                    return JSConvert.ConvertObjectToJS(es, v);
                }
            }
            return JSApi.wkeJSUndefined(es);
        }


    }
}
