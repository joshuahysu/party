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
        /// �b��l�B
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// �b���I��
        /// </summary>
        public decimal Points { get; set; }

        /// <summary>
        /// �n�ͦC��
        /// </summary>
        public ICollection<UserAccountPublic> Friends { get; set; }
        /// <summary>
        /// �n���ܽЦC��
        /// </summary>
        public ICollection<UserAccountPublic> FriendsInvite { get; set; }
        /// <summary>
        /// �l�ܦC��
        /// </summary>
        public ICollection<UserAccountPublic> Trace { get; set; }

        /// <summary>
        /// �I�O�l�ܦC��
        /// </summary>
        public ICollection<UserAccountPublic> PayTrace { get; set; }

        /// <summary>
        /// �w�I�O���ئC��
        /// </summary>
        public ICollection<UserAccountPublic> Purchased { get; set; }
    }
}
