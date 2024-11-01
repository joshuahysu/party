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
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            UserAccount? user =_context.UserAccount.Find(currentId);

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
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        /// <summary>
        /// 發送好友邀請
        /// </summary>
        /// <param name="inviteId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostFriendsInvite(ulong inviteId, string action)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.UserAccount.Find(currentId);
            var inviteUser = _context.UserAccount.Find(inviteId);
            if (action == "add")
            {
                //添加在自己的列表
                user.FriendsInvite.Add(new UserAccountPublic() { Id = inviteId });
                //添加在對方的列表
                inviteUser.FriendsBeInvite.Add(new UserAccountPublic() { Id = currentId });
            }
            else
            {
                user.FriendsInvite.Remove(new UserAccountPublic() { Id = inviteId });
                inviteUser.FriendsBeInvite.Remove(new UserAccountPublic() { Id = currentId });
            }
            await _context.SaveChangesAsync();

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
        /// <summary>
        /// 是否答應好友邀請或刪除好友
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostFriends(ulong requestId, string action)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.UserAccount.Find(currentId);

            if (action == "add")
            {
                //添加好友
                user.Friends.Add(new UserAccountPublic() { Id = requestId });
            }
            else if (action == "remove")
            {
                //刪除好友
                user.Friends.Remove(new UserAccountPublic() { Id = requestId });
            }
            else
            {   //刪除好友邀請
                user.FriendsBeInvite.Remove(new UserAccountPublic() { Id = requestId });
            }
            await _context.SaveChangesAsync();

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
    }
}
