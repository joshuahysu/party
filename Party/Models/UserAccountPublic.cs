using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Party.Models
{
    public class UserAccountPublic
    {
        /// <summary>
        /// UserID
        /// </summary>
        [Key]
        public required ulong Id { get; set; }

        /// <summary>
        /// 被追蹤次數
        /// </summary>
        public double TraceCount { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        public string PhotoAddress { get; set; }

        /// <summary>
        /// 影片位置
        /// </summary>
        public string VideoAddress { get; set; }

    }
}
