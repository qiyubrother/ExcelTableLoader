using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class LogHelper
    {
        static Queue<string> queue = new Queue<string>();
        private static bool _enable = false;
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
        /// <summary>
        /// 启动日志服务
        /// </summary>
        public static void StartService()
        {
            _enable = true;
            LogWriter();
        }
        /// <summary>
        /// 停止日志服务
        /// </summary>
        public static void Stop()
        {
            _enable = false;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="s"></param>
        /// <param name="param"></param>
        public static void Trace(string s, params object[] param)
        {
            var p = param == null || param.Length == 0 ? new[] { "" } : param;
            var str = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}][ThreadId:{Thread.CurrentThread.ManagedThreadId}]";
            if (param == null || param.Length == 0)
            {
                str = str + s;
            }
            else
            {
                str = str + string.Format(s, p);
            }
            Console.WriteLine(str);
            queue.Enqueue(str);
            //Task.Run(() => qiyubrother.SysLogHelper.SendMessage(str)); // 发送日志到syslog服务器
        }

        /// <summary>
        /// 写日志到文件
        /// </summary>
        private static void LogWriter()
        {
            Task.Run(() =>
            {
                while (_enable)
                {
                    if (queue.Count > 0)
                    {
                        var item = queue.Dequeue();
                        var fn = Path.Combine(Environment.CurrentDirectory, $"Trace-{DateTime.Now.Year}{DateTime.Now.Month.ToString().PadLeft(2, '0')}{DateTime.Now.Day.ToString().PadLeft(2, '0')}.log");
                        var cnt = 0;
                        do
                        {
                            try
                            {
                                System.IO.File.AppendAllLines(fn, new[] { item });
                                OutputDebugString(item);
                                break;
                            }
                            catch
                            {
                                cnt++;
                                System.Threading.Thread.Sleep(200);
                            }
                            if (cnt > 3)
                            {
                                // 超过3次写入错误
                                var efn = $"Error-{DateTime.Now.Year}{DateTime.Now.Month.ToString().PadLeft(2, '0')}{DateTime.Now.Day.ToString().PadLeft(2, '0')}.log";
                                try
                                {
                                    System.IO.File.AppendAllLines(efn, new[] { $"[{DateTime.Now.ToString()}]日志系统错误。" });
                                }
                                catch { }
                                
                                break;
                            }
                        }while (true);
                    }
                    System.Threading.Thread.Sleep(10);
                }
            });
        }

    }
}
