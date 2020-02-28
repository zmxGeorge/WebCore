using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WebCore.Miniblink
{
    public class NativeControl:Control
    {
        public string DisposeFun = null;

        public IntPtr WebView { get; set; }

        public NativeControl()
        {
            this.SetStyle(ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Selectable ,true);
        }

        private const string RETURN_FUN = "return {0};";

        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(DisposeFun))
            {
                var es = MBApi.wkeGlobalExec(WebView);
                var fun = MBApi.jsEvalW(es, string.Format(RETURN_FUN, DisposeFun));
                MBApi.jsCallGlobal(es, fun, null, 0);
            }
            base.Dispose(disposing);
        }
    }
}
