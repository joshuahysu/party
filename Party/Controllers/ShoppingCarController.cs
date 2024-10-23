using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using System;
using System.Diagnostics;

namespace Party.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCarController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<UserController> _localizer;

        public ShoppingCarController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        //1.儲值
        //2.信用卡扣款
        //3.定期扣款
        //4.增加餘額


        // POST: api/Trace
        [HttpPost]
        public async Task<IActionResult> PostTrace(ulong id, ulong traceId,string action)
        {
            var user=_context.UserAccount.Find(id);

            if (action == "add")
            {
                user.Trace.Add(new UserAccountPublic() { Id = traceId });
            }
            else
            {
                user.Trace.Remove(new UserAccountPublic() { Id = traceId });
            }
            await _context.SaveChangesAsync();

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
        // POST: api/PostPayTrace
        [HttpPost]
        public async Task<IActionResult> PostPayTrace(string id, ulong traceId, string action)
        {
            var user = _context.UserAccount.Find(id);

            if (action == "add")
            {
                user.PayTrace.Add(new UserAccountPublic() { Id = traceId });
            }
            else
            {
                user.PayTrace.Remove(new UserAccountPublic() { Id = traceId });
            }
            await _context.SaveChangesAsync();

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
    }
}
