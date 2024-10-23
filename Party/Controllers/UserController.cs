using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using System;
using System.Diagnostics;
using System.Security.Claims;

namespace Party.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<UserController> _localizer;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // POST: api/Trace
        [HttpPost]
        public async Task<IActionResult> PostTrace(ulong traceId,string action)
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user=_context.UserAccount.Find(currentId);

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
        public async Task<IActionResult> PostPayTrace(ulong traceId, string action)
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.UserAccount.Find(currentId);

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

        [HttpPost]
        public async Task<IActionResult> PostFriendsInvite(ulong inviteId, string action)
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.UserAccount.Find(currentId);

            if (action == "add")
            {
                user.FriendsInvite.Add(new UserAccountPublic() { Id = inviteId });
            }
            else
            {
                user.FriendsInvite.Remove(new UserAccountPublic() { Id = inviteId });
            }
            await _context.SaveChangesAsync();

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }

        [HttpPost]
        public async Task<IActionResult> PostFriends(ulong requestId, string action)
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.UserAccount.Find(currentId);

            if (action == "add")
            {
                user.Friends.Add(new UserAccountPublic() { Id = requestId });
            }
            else if (action == "remove")
            {
                user.Friends.Add(new UserAccountPublic() { Id = requestId });
            }
            else
            {
                user.FriendsInvite.Remove(new UserAccountPublic() { Id = requestId });
            }
            await _context.SaveChangesAsync();

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
    }
}
