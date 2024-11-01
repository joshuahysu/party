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

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
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

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
        /// <summary>
        /// �o�e�n���ܽ�
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
                //�K�[�b�ۤv���C��
                user.FriendsInvite.Add(new UserAccountPublic() { Id = inviteId });
                //�K�[�b��誺�C��
                inviteUser.FriendsBeInvite.Add(new UserAccountPublic() { Id = currentId });
            }
            else
            {
                user.FriendsInvite.Remove(new UserAccountPublic() { Id = inviteId });
                inviteUser.FriendsBeInvite.Remove(new UserAccountPublic() { Id = currentId });
            }
            await _context.SaveChangesAsync();

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
        /// <summary>
        /// �O�_�����n���ܽЩΧR���n��
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
                //�K�[�n��
                user.Friends.Add(new UserAccountPublic() { Id = requestId });
            }
            else if (action == "remove")
            {
                //�R���n��
                user.Friends.Remove(new UserAccountPublic() { Id = requestId });
            }
            else
            {   //�R���n���ܽ�
                user.FriendsBeInvite.Remove(new UserAccountPublic() { Id = requestId });
            }
            await _context.SaveChangesAsync();

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
    }
}
