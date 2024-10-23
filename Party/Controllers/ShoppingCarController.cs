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
        //1.�x��
        //2.�H�Υd����
        //3.�w������
        //4.�W�[�l�B


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

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
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

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
    }
}
