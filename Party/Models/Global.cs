
using System.Diagnostics;

namespace Party.Models
{
    //fix: bad design，當架構變複雜時耦合度會太高，未來可能需要拆分成基於singleton pattern的manager class
    /// <summary>
    /// 全域管理者，相當於各項功能的Manager
    /// </summary>
    public static class Global
    {
        #region Constants

        /// <summary>
        /// 定義目前程序
        /// </summary>
        private static readonly Process PROC = Process.GetCurrentProcess();



        #endregion

        #region Properties


        #endregion

        #region Initialize



        #endregion

        #region Others


        /// <summary>
        /// 取得系統資源使用狀況
        /// </summary>
        /// <returns>系統資源使用狀況</returns>

        public static ResourceUsage GetResourceUsage()
        {
            PROC.Refresh();
            TimeSpan cpu = PROC.TotalProcessorTime;
            Stopwatch sw = Stopwatch.StartNew();
            Task waiter = Task.Delay(50);
            ResourceUsage usage = new()
            {
                Memory = PROC.WorkingSet64,
                Threads = PROC.Threads.Count,

            };
            waiter.Wait();
            sw.Stop();
            PROC.Refresh();
            usage.Cpu = (PROC.TotalProcessorTime - cpu).TotalMilliseconds / sw.ElapsedMilliseconds / Environment.ProcessorCount;
            return usage;
        }

        #endregion
    }
}
