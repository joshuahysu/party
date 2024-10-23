using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Party.Models
{
    public class UserAccount
    {
        /// <summary>
        /// UserID
        /// </summary>
        [Key]
        public required ulong Id { get; set; }

        /// <summary>
        /// 帳戶餘額
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 帳戶點數
        /// </summary>
        public decimal Points { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        public ICollection<UserAccountPublic> Friends { get; set; }
        /// <summary>
        /// 好友邀請列表
        /// </summary>
        public ICollection<UserAccountPublic> FriendsInvite { get; set; }
        /// <summary>
        /// 追蹤列表
        /// </summary>
        public ICollection<UserAccountPublic> Trace { get; set; }

        /// <summary>
        /// 付費追蹤列表
        /// </summary>
        public ICollection<UserAccountPublic> PayTrace { get; set; }

        /// <summary>
        /// 已付費項目列表
        /// </summary>
        public ICollection<UserAccountPublic> Purchased { get; set; }
    }
}
