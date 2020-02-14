using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace WebCore
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LoggerLevel
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info = 1000,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 异常
        /// </summary>
        Exception
    }


    public class DCLogger
    {
        private static readonly DCLogger _logger = new DCLogger();

        private const string LogDir = "Logs";

        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.ffffff";

        private const string FILENAME_DATE_FORMAT = "yyyyMMddHH";

        private const string LAST_MONTH_FORMAT = "yyyyMM";

        private readonly Semaphore _lock = new Semaphore(1, 1);

        private DateTime _clearTime = DateTime.Now.Date.AddDays(3);

        private DateTime _checkTime = DateTime.MinValue;

        public static DCLogger Current { get { return _logger; } }

        public void Dispose()
        {

        }

        public bool Init()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(LogDir);
            dirInfo.Refresh();
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            _checkTime = DateTime.Now;
            return true;
        }

        public void WriteLog(LoggerLevel level,
            string message)
        {
            _lock.WaitOne();
            var nTime = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[时间]:{0}\r\n", nTime.ToString(DATE_TIME_FORMAT));
            sb.AppendFormat("[级别]:{0}\r\n", level.ToString());
            sb.AppendFormat("[消息]:{0}\r\n", message);
            sb.Append("\r\n");
            string timeStamp = DateTime.Now.ToString(FILENAME_DATE_FORMAT);
            string fullPath = Path.Combine(LogDir, timeStamp + ".log");
            File.AppendAllText(fullPath, sb.ToString());
            if (_checkTime == DateTime.MinValue)
            {
                _checkTime = nTime;
            }
            if ((nTime - _checkTime).TotalHours >= 1)
            {
                //每隔一小时检查日志文件
                ThreadPool.QueueUserWorkItem(CheckLogs);
                _checkTime = nTime;
            }
            _lock.Release();
        }


        public void WriteLog(string title, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\r\n[来源]:{0}\r\n", title);
            if (ex is IOException)
            {
                var IoEx = (ex as IOException);
                sb.AppendFormat("[异常标题]：{0}\r\n", "IO异常");
                sb.AppendFormat("[异常消息]：{0}\r\n", IoEx.Message);
            }
            else if (ex is WebException)
            {
                var webEx = (ex as WebException);
                sb.AppendFormat("[异常标题]：{0}\r\n", "Http请求异常");
                sb.AppendFormat("[异常类型]：{0}\r\n", webEx.Status);
                sb.AppendFormat("[异常消息]：{0}\r\n", webEx.Message);
                var res = webEx.Response;
                if (res != null)
                {
                    sb.AppendFormat("[响应Url]：{0}\r\n", res.ResponseUri.AbsoluteUri);
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        var stream = res.GetResponseStream();
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            sb.AppendFormat("[响应内容]:{0}\r\n", reader.ReadToEnd());
                        }
                        res.Close();
                    }
                }
            }
            else if (ex is FileNotFoundException)
            {
                var fileEx = (ex as FileNotFoundException);
                sb.AppendFormat("[异常标题]：{0}\r\n", "文件未找到");
                sb.AppendFormat("[文件路径]：{0}\r\n", fileEx.FileName);
                sb.AppendFormat("[异常消息]：{0}\r\n", ex.Message);
                sb.AppendFormat("[堆栈信息]：{0}\r\n", ex.ToString());
            }
            else if (ex is FileLoadException)
            {
                var fileEx = (ex as FileLoadException);
                sb.AppendFormat("[异常标题]：{0}\r\n", "程序集加载失败");
                sb.AppendFormat("[文件路径]：{0}\r\n", fileEx.FileName);
                sb.AppendFormat("[日志文件]：{0}\r\n", fileEx.FusionLog);
                sb.AppendFormat("[异常消息]：{0}\r\n", ex.Message);
            }
            else if (ex is ArgumentNullException)
            {
                var arg = (ex as ArgumentNullException);
                sb.AppendFormat("[异常标题]：{0}\r\n", "参数为空");
                sb.AppendFormat("[异常消息]：{0}\r\n", arg.Message);
                sb.AppendFormat("[参数名称]：{0}\r\n", arg.ParamName);
            }
            else
            {
                sb.AppendFormat("[异常标题]：{0}\r\n", ex.GetType().Name);
                sb.AppendFormat("[异常消息]：{0}\r\n", ex.Message);
            }
            sb.AppendFormat("[堆栈信息]：{0}\r\n", ex.StackTrace);
            sb.Append("\r\n");
            WriteLog(LoggerLevel.Exception, sb.ToString());
        }

        private void CheckLogs(object state)
        {
            DateTime nTimeDate = DateTime.Now.Date.AddMonths(-3);
            DirectoryInfo dirInfo = new DirectoryInfo(LogDir);
            dirInfo.Refresh();
            var files = dirInfo.GetFiles();
            foreach (var file in files)
            {
                if (file.LastWriteTime < nTimeDate)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

    }
}
