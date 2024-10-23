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
        /// �Q�l�ܦ���
        /// </summary>
        public double TraceCount { get; set; }

        /// <summary>
        /// �Ӥ�
        /// </summary>
        public string PhotoAddress { get; set; }

        /// <summary>
        /// �v����m
        /// </summary>
        public string VideoAddress { get; set; }

    }
}
