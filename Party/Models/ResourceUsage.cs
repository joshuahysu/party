namespace Party.Models
{ 
    /// <summary>
    /// 系統資源使用量
    /// </summary>

    public class ResourceUsage
    {
        /// <summary>
        /// 定義常用的數值單位
        /// </summary>
        private static readonly string[] UNIT = { "", "k", "M", "G", "T", "P", "E" };

        #region Properties

        /// <summary>
        /// CPU使用率
        /// </summary>
        public double Cpu { get; set; }

        /// <summary>
        /// 記憶體使用量，單位為byte
        /// </summary>
        public long Memory { get; set; }

        /// <summary>
        /// 執行緒數量
        /// </summary>
        public int Threads { get; set; }

        #endregion

        /// <summary>
        /// 快取資源使用量
        /// </summary>
        //public Dictionary<string, CacheUsage> Caches { get; set; } = new();

        #region Convert to String


        /// <summary>
        /// 將較大的數值轉成人類易讀的文字
        /// </summary>
        /// <param name="x">數值</param>
        /// <returns>易讀文字</returns>
        private static string Nice(long x)
        {
            int sign = Math.Sign(x);
            x = Math.Abs(x);
            int level = (int)Math.Floor(Math.Log(x + 1, 10) / 3);
            double result = x * Math.Pow(10, -level * 3);
            return $"{sign * result:0.##}{UNIT[level]}";
        }

        #endregion
    }
}
