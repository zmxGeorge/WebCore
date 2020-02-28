using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WebCore
{
    public class WebMessageFilter : IMessageFilter
    {
        private const int WM_TIMER = 0x113;

        public bool PreFilterMessage(ref Message m)
        {
            //Console.WriteLine(m);
            return false;
        }
    }
}
