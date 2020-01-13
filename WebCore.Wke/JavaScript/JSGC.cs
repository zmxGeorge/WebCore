using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebCore.Wke.JavaScript
{
    /// <summary>
    /// JS对象垃圾回收器
    /// </summary>
    public class JSGC
    {
        private static readonly JSGC _gc = new JSGC();

        /// <summary>
        /// 获取当前垃圾回收器
        /// </summary>
        public static JSGC Current { get { return _gc; } }

        private readonly Semaphore _lock = new Semaphore(1, 1);

        /// <summary>
        /// 调用GC回收
        /// </summary>
        public void Collect()
        {
            try
            {
                _lock.WaitOne();
                WkeApi.wkeJSCollectGarbge();
            }
            finally
            {
                _lock.Release();
            }
        }

    }
}
