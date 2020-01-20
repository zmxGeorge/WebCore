using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Reflection;
using WebCore.Wke.JavaScript;
using System.Runtime.CompilerServices;

namespace WebCore.Wke
{
   

    public class Browser
    {
        private static readonly Browser _browser = new Browser();

        /// <summary>
        /// 获取当前对象实例
        /// </summary>
        public static Browser Current { get { return _browser; } }

        public wkeJSCallAsFunctionCallback _loadLib = null;

        public wkeJSCallAsFunctionCallback _createObj = null;

        public wkeJSCallAsFunctionCallback _createComObj = null;

        public wkeJSCallAsFunctionCallback _createControl = null;

        public wkeJSCallAsFunctionCallback _changeControl = null;

        private wkeJSFinalizeCallback _disposed = null;

        public Browser()
        {
            _loadLib = new wkeJSCallAsFunctionCallback(OnLoadLib);
            _createObj = new wkeJSCallAsFunctionCallback(OnCreateObj);
            _createComObj = new wkeJSCallAsFunctionCallback(OnCreateComObj);
            _createControl = new wkeJSCallAsFunctionCallback(OnCreateControl);
            _changeControl = new wkeJSCallAsFunctionCallback(OnChangeControl);
            _disposed = new wkeJSFinalizeCallback(Disposed);
        }

        private long OnCreateComObj(IntPtr es, long obj, IntPtr args, int argCount)
        {
            if (argCount != 2)
            {
                return JSApi.wkeJSUndefined(es);
            }
            long typeName_Val = JSApi.wkeJSParam(es, 0);
            if (!JSApi.wkeJSIsString(es, typeName_Val))
            {
                return JSApi.wkeJSUndefined(es);
            }
            long interFace_Val = JSApi.wkeJSParam(es, 1);
            if (!JSApi.wkeJSIsString(es, interFace_Val))
            {
                return JSApi.wkeJSUndefined(es);
            }
            //实现类
            Type localType = GetComType(es, typeName_Val);
            //接口类
            Type interfaceType = GetComType(es, interFace_Val);
            if (localType == null || interfaceType == null)
            {
                return JSApi.wkeJSNull(es);
            }
            //创建Com对象
            var coObject =Convert.ChangeType(Activator.CreateInstance(localType),interfaceType);
            WkeObjectRef objRef = new WkeObjectRef(es, coObject, interfaceType, true);
            return objRef.JsValue;
        }

        private static Type GetComType(IntPtr es, long typeName_Val)
        {
            string typeName =JSHelper.GetJsString(es, typeName_Val);
            Guid comId = Guid.Empty;
            Type localType = null;
            if (Guid.TryParse(typeName, out comId))
            {
                localType = Type.GetTypeFromCLSID(comId);
            }
            else
            {
                localType = Type.GetTypeFromProgID(typeName);
            }
            return localType;
        }

        private long OnChangeControl(IntPtr es, long obj, IntPtr args, int argCount)
        {
            return 0;
        }

        private long OnCreateControl(IntPtr es, long obj, IntPtr args, int argCount)
        {
            return 0;
        }

        private long OnCreateObj(IntPtr es, long obj, IntPtr args, int argCount)
        {
            var typeName_Val = JSApi.wkeJSParam(es, 0);
            if (!JSApi.wkeJSIsString(es, typeName_Val))
            {
                return JSApi.wkeJSUndefined(es);
            }
            string typeName = JSHelper.GetJsString(es, typeName_Val);
            Type localType = AppDomain.CurrentDomain.
                GetAssemblies().SelectMany(t=>t.GetTypes()).FirstOrDefault(x=> {
                    return x.FullName == typeName;
                });
            if(localType==null)
            {
                return JSApi.wkeJSUndefined(es);
            }
            object[] obj_args = new object[argCount - 1];
            for (int i = 1; i < obj_args.Length; i++)
            {
                obj_args[i] = JSConvert.ConvertJSToObject(es, JSApi.wkeJSParam(es, i), typeof(object));
            }
            var localObj = Activator.CreateInstance(localType, obj_args);
            WkeObjectRef objRef = new WkeObjectRef(es, localObj, localType, false);
            return objRef.JsValue;
        }

        private long OnLoadLib(IntPtr es, long obj, IntPtr args, int argCount)
        {
            try
            {
                var p1 = JSApi.wkeJSParam(es, 0);
                if (!JSApi.wkeJSIsString(es, p1))
                {
                    return JSApi.wkeJSFalse(es);
                }
                string libFile = JSHelper.GetJsString(es, p1);
                if (string.IsNullOrEmpty(libFile))
                {
                    return JSApi.wkeJSFalse(es);
                }
                libFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), libFile);
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.Location == libFile))
                {
                    //若该程序集尚未加载，则执行加载
                    Assembly.LoadFile(libFile);
                    return JSApi.wkeJSTrue(es);
                }
                return JSApi.wkeJSTrue(es);
            }
            finally
            {
                JSGC.Current.Collect();
            }
        }

        private void Disposed(IntPtr data)
        {
            Marshal.FreeHGlobal(data);
        }

        public void Application_Init()
        {
            WkeApi.wkeInitialize();
            wkeSettings settings = new wkeSettings();
            settings.mask = 2;
            settings.cookieFilePath = new char[1024];
            "cookies.dat".ToCharArray().CopyTo(settings.cookieFilePath, 0);
            var size = Marshal.SizeOf(settings);
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(settings, ptr, true);
            WkeApi.wkeConfigure(ptr);
            Marshal.FreeHGlobal(ptr);
        }

        public void Application_Close()
        {
            WkeApi.wkeFinalize();
        }


    }
}
