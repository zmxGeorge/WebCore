using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WebCore.Miniblink
{
    public class Browser
    {
        private static readonly Browser _browser = new Browser();

        public static Browser Current { get { return _browser; } }


        private jsCallAsFunctionCallback _loadLib = null;

        private jsCallAsFunctionCallback _createObj = null;

        private jsCallAsFunctionCallback _createComObj = null;

        private jsCallAsFunctionCallback _applicationExit = null;

        private jsCallAsFunctionCallback _getCurrentProcessId = null;

        private jsCallAsFunctionCallback _createControl = null;

        private jsCallAsFunctionCallback _changeControl = null;

        private wkeJsNativeFunction e_loadLib = null;

        private wkeJsNativeFunction e_createObj = null;

        private wkeJsNativeFunction e_createComObj = null;

        private wkeJsNativeFunction e_applicationExit = null;

        private wkeJsNativeFunction e_getCurrentProcessId = null;

        private wkeJsNativeFunction e_createControl = null;

        private wkeJsNativeFunction e_changeControl = null;

        private jsFinalizeCallback _disposed = null;

        public void Init()
        {
            if (MBApi.wkeIsInitialize() == 0)
            {
                MBApi.wkeInitialize();
            }
            _disposed = new jsFinalizeCallback(OnDispose);
            _loadLib = new jsCallAsFunctionCallback(OnLoadLib);
            _createObj = new jsCallAsFunctionCallback(OnCreateObject);
            _createComObj = new jsCallAsFunctionCallback(OnCreateComObj);
            _applicationExit = new jsCallAsFunctionCallback(OnApplicationExit);
            _getCurrentProcessId = new jsCallAsFunctionCallback(OnGetProcessId);
            _createControl = new jsCallAsFunctionCallback(OnCreateControl);
            _changeControl = new jsCallAsFunctionCallback(OnChangeControl);

            e_loadLib = new wkeJsNativeFunction(GetOnLoadLib);
            e_createObj = new wkeJsNativeFunction(GetOnCreateObject);
            e_createComObj = new wkeJsNativeFunction(GetOnCreateComObj);
            e_applicationExit = new wkeJsNativeFunction(GetOnApplicationExit);
            e_getCurrentProcessId = new wkeJsNativeFunction(GetOnGetProcessId);
            e_createControl = new wkeJsNativeFunction(GetOnCreateControl);
            e_changeControl = new wkeJsNativeFunction(GetOnChangeControl);
            MBApi.wkeJsBindGetter("_loadAssembly", e_loadLib, IntPtr.Zero);
            MBApi.wkeJsBindGetter("_createObject", e_createObj, IntPtr.Zero);
            MBApi.wkeJsBindGetter("_createComObject", e_createComObj, IntPtr.Zero);
            MBApi.wkeJsBindGetter("_createControl", e_createControl, IntPtr.Zero);
            MBApi.wkeJsBindGetter("_changeControl", e_changeControl, IntPtr.Zero);
            MBApi.wkeJsBindGetter("_getCurrentProcessId", e_getCurrentProcessId, IntPtr.Zero);
            MBApi.wkeJsBindGetter("_appExit", e_applicationExit, IntPtr.Zero);
        }

        private void OnDispose(IntPtr data)
        {
            Marshal.FreeHGlobal(data);
        }

        public long CreateFunction(IntPtr es, jsCallAsFunctionCallback fun)
        {
            jsData data = new jsData();
            data.callAsFunction = fun;
            data.propertyGet = null;
            data.propertySet = null;
            data.finalize = _disposed;
            var size = Marshal.SizeOf(typeof(jsData));
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, ptr, true);
            var v = MBApi.jsFunction(es, ptr);
            return v;
        }

        private long GetOnChangeControl(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _changeControl);
        }

        private long GetOnCreateControl(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _createControl);
        }

        private long GetOnGetProcessId(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _getCurrentProcessId);
        }

        private long GetOnApplicationExit(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _applicationExit);
        }

        private long GetOnCreateComObj(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _createComObj);
        }

        private long GetOnCreateObject(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _createObj);
        }

        private long GetOnLoadLib(IntPtr es, IntPtr param)
        {
            return CreateFunction(es, _loadLib);
        }

        #region 原生函数封装
        private long OnChangeControl(IntPtr es, long obj,
            IntPtr args, int argCount)
        {
            if (argCount != 5)
            {
                return MBApi.jsUndefined();
            }
            var vPtr = MBApi.jsArg(es, 0);
            var vX = MBApi.jsArg(es, 1);
            var vY = MBApi.jsArg(es, 2);
            var vWidth = MBApi.jsArg(es, 3);
            var vHeight = MBApi.jsArg(es, 4);
            IntPtr controlPtr = IntPtr.Zero;
            if (MBApi.jsIsString(vPtr))
            {
                var strPtr = ExtApi.GetJsString(es, vPtr);
                int n = 0;
                if (!int.TryParse(strPtr, out n))
                {
                    return MBApi.jsUndefined();
                }
                controlPtr = new IntPtr(n);
            }
            else if (MBApi.jsIsNumber(vPtr))
            {
                var ptr = MBApi.jsToInt(es, vPtr);
                controlPtr = new IntPtr(ptr);
            }
            else
            {
                return MBApi.jsUndefined();
            }
            NativeControl control = NativeControl.FromHandle(controlPtr) as NativeControl;
            if (control == null)
            {
                return MBApi.jsUndefined();
            }
            int x, y, width, height = 0;
            if (!MBApi.jsIsNumber(vX) ||
               !MBApi.jsIsNumber(vY) ||
               !MBApi.jsIsNumber(vWidth) ||
               !MBApi.jsIsNumber(vHeight))
            {
                return MBApi.jsUndefined();
            }
            x = MBApi.jsToInt(es, vX);
            y = MBApi.jsToInt(es, vY);
            width = MBApi.jsToInt(es, vWidth);
            height = MBApi.jsToInt(es, vHeight);
            control.SetBounds(x, y, width, height);
            return MBApi.jsTrue();
        }

        private long OnCreateControl(IntPtr es, long obj, 
            IntPtr args, int argCount)
        {
            if (argCount < 4)
            {
                return MBApi.jsUndefined();
            }
            var vX = MBApi.jsArg(es, 0);
            var vY = MBApi.jsArg(es, 1);
            var vWidth = MBApi.jsArg(es, 2);
            var vHeight = MBApi.jsArg(es, 3);
            var disposeFun = MBApi.jsArg(es, 4);
            int x, y, width, height = 0;
            if (!MBApi.jsIsNumber(vX) ||
               !MBApi.jsIsNumber(vY) ||
               !MBApi.jsIsNumber(vWidth) ||
               !MBApi.jsIsNumber(vHeight))
            {
                return MBApi.jsUndefined();
            }
            x = MBApi.jsToInt(es, vX);
            y = MBApi.jsToInt(es, vY);
            width = MBApi.jsToInt(es, vWidth);
            height = MBApi.jsToInt(es, vHeight);
            string jsData = null;
            if (!MBApi.jsIsUndefined(disposeFun))
            {
                jsData = ExtApi.GetJsString(es, disposeFun);
            }
            var mHandle = MBApi.jsGetWebView(es);
            NativeControl control = new NativeControl();
            control.WebView = mHandle;
            control.Location = new Point(x, y);
            control.Width = width;
            control.Height = height;
            control.DisposeFun = jsData;
            WebView parentView = GetParent(mHandle);
            if (parentView == null)
            {
                return MBApi.jsUndefined();
            }
            parentView.Controls.Add(control);
            control.BringToFront();
            var handle = control.Handle.ToInt32();
            return MBApi.jsToInt(es, handle);
        }

        private static WebView GetParent(IntPtr mHandle)
        {
            foreach (Form form in Application.OpenForms)
            {
                foreach (var c in form.Controls)
                {
                    if (c is WebView)
                    {
                        var bView = c as WebView;
                        if (bView.ViewHandle == mHandle)
                        {
                            return bView;
                        }
                    }
                }
            }
            return null;
        }

        private long OnGetProcessId(IntPtr es, long obj, IntPtr args, int argCount)
        {
            using (Process pro = Process.GetCurrentProcess())
            {
                return MBApi.jsInt(pro.Id);
            }
        }

        private long OnApplicationExit(IntPtr es, long obj, IntPtr args, int argCount)
        {
            Application.Exit();
            return MBApi.jsTrue();
        }

        private readonly Dictionary<IntPtr,List<CSharpObject>> sharpObjects = new Dictionary<IntPtr, List<CSharpObject>>();

        public void ClearViewObject(IntPtr webView)
        {
            lock (sharpObjects)
            {
                if (sharpObjects.ContainsKey(webView))
                {
                    var list = sharpObjects[webView];
                    foreach (var item in list)
                    {
                        item.Dispose();
                    }
                    list.Clear();
                }
            }
            MBApi.jsGC();
        }

        public void ClearAll()
        {
            lock (sharpObjects)
            {
                foreach (var iKv in sharpObjects)
                {
                    var list = iKv.Value;
                    foreach (var item in list)
                    {
                        item.Dispose();
                    }
                    list.Clear();
                }
            }
            MBApi.jsGC();
        }

        private long OnCreateComObj(IntPtr es, long obj, IntPtr args, int argCount)
        {
            var typeName_ID = MBApi.jsArg(es, 0);
            if (!MBApi.jsIsString(typeName_ID))
            {
                return MBApi.jsUndefined();
            }
            string typeID = ExtApi.GetJsString(es, typeName_ID);
            Guid clsId = new Guid(typeID);
            Type localType = Type.GetTypeFromCLSID(clsId);
            if (localType == null)
            {
                return MBApi.jsUndefined();
            }
            object[] obj_args = new object[argCount - 1];
            for (int i = 1; i < obj_args.Length; i++)
            {
                obj_args[i] = JsConvert.ConvertJSToObject(es, MBApi.jsArg(es, i), typeof(object));
            }
            var localObj = Activator.CreateInstance(localType, obj_args);
            var webView = MBApi.jsGetWebView(es);
            CSharpObject local_obj = new CSharpObject(webView, localObj);
            lock (sharpObjects)
            {
                List<CSharpObject> list = null;
                if (!sharpObjects.ContainsKey(webView))
                {
                    list = new List<CSharpObject>();
                }
                else
                {
                    list = sharpObjects[webView];
                }
                list.Add(local_obj);
            }
            return local_obj.ScriptValue;
        }

        private long OnCreateObject(IntPtr es, long obj, IntPtr args, int argCount)
        {
            try
            {
                var typeName_Val = MBApi.jsArg(es, 0);
                if (!MBApi.jsIsString(typeName_Val))
                {
                    return MBApi.jsUndefined();
                }
                string typeName = ExtApi.GetJsString(es, typeName_Val);
                Type localType = null;
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in ass.GetTypes())
                    {
                        if (type.FullName == typeName)
                        {
                            localType = type;
                            break;
                        }
                    }
                }
                if (localType == null)
                {
                    return MBApi.jsUndefined();
                }
                object[] obj_args = new object[argCount - 1];
                for (int i = 1; i < obj_args.Length; i++)
                {
                    obj_args[i] = JsConvert.ConvertJSToObject(es, MBApi.jsArg(es, i), typeof(object));
                }
                var localObj = Activator.CreateInstance(localType, obj_args);
                var webView = MBApi.jsGetWebView(es);
                CSharpObject local_obj = new CSharpObject(webView, localObj);
                lock (sharpObjects)
                {
                    List<CSharpObject> list = null;
                    if (!sharpObjects.ContainsKey(webView))
                    {
                        list = new List<CSharpObject>();
                    }
                    else
                    {
                        list = sharpObjects[webView];
                    }
                    list.Add(local_obj);
                }
                return local_obj.ScriptValue;
            }
            finally
            {
                
            }
        }

        private readonly List<string> _libSet = new List<string>();

        private long OnLoadLib(IntPtr es, long obj, IntPtr args, int argCount)
        {
            try
            {
                var p1 = MBApi.jsArg(es, 0);
                if (!MBApi.jsIsString(p1))
                {
                    return MBApi.jsFalse();
                }
                string libFile = ExtApi.GetJsString(es, p1);
                lock (_libSet)
                {
                    if (_libSet.Contains(libFile))
                    {
                        return MBApi.jsTrue();
                    }
                }
                if (string.IsNullOrEmpty(libFile))
                {
                    return MBApi.jsFalse();
                }
                libFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), libFile);
                bool find = false;
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ass.Location == libFile)
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    //若该程序集尚未加载，则执行加载
                    Assembly.LoadFile(libFile);
                    lock (_libSet)
                    {
                        _libSet.Add(libFile);
                    }
                    return MBApi.jsTrue();
                }
                return MBApi.jsTrue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return MBApi.jsUndefined();
            }
            finally
            {
              
            }
        }

        #endregion

        public void Close()
        {
            CSharpStore.Current.Close();
            ClearAll();
            MBApi.wkeFinalize();
        }
    }
}
