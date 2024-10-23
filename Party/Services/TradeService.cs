using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Party.Data;
using Party.Models;

namespace Party.Services
{
    public class TradeService
    {

        public static (bool result, string resultText) Buy(ulong id,decimal cost, decimal pointCost,string message)
        {
            using (ApplicationDbContext _context = new()) {
                var user = _context.UserAccount.Find(id);
                if (user == null) return (false, "沒有此ID");
                if (cost > user.Balance) return (false, "餘額不足");
                if (pointCost > user.Points) return (false, "點數不足");
                user.Balance -= cost;
                user.Points -= pointCost;
                //todo:歷史紀錄
                _context.SaveChanges();
            }
            return (true,"Success");
        }

        public static (bool result, string resultText) AddValue(ulong id, decimal cost, decimal pointCost, string message) {
            return Buy(id,cost*(-1),pointCost*(-1),message);
        }
    }

}
