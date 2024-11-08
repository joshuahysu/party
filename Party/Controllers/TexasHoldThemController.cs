using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Services;
using Party.Services.ChatRoom;
using Party.Services.TexasHoldThem;
using System.Security.Claims;

namespace Party.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TexasHoldThemController : ControllerBase
    {
        private readonly ILogger<TexasHoldThemController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<TexasHoldThemController> _localizer;
        private readonly ChatRoomService _chatRoomService;

        public TexasHoldThemController(ILogger<TexasHoldThemController> logger, ApplicationDbContext context, ChatRoomService chatRoomService)
        {
            _logger = logger;
            _context = context;
            _chatRoomService = chatRoomService;
        }
        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="chips">0是跟注</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostBet(string roomNumber, ulong chips)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room);
            if (room != null)
            {
                TexasHoldThemService.BetProcess(room, currentId, chips);
            }
            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
        /// <summary>
        /// 棄牌
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostFold(string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room);
            if (room != null)
            {
                TexasHoldThemService.FoldProcess(room, currentId);
            }
            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }

        /// <summary>
        /// AllIn
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="chips"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAllIn(string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room);
            if (room != null)
            {
                room.IDPokerUserDic.TryGetValue(currentId, out var player);
                TexasHoldThemService.BetProcess(room, currentId, player.RoomChips);
            }

            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
        // POST: api/BuyIn
        [HttpPost]
        public async Task<IActionResult> PostBuyIn(int chips, string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));

            TexasHoldThemService.BuyIn(currentId);

            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
        /// <summary>
        /// 加入遊戲房間
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostJoinRoom(string roomNumber)
        {
            //todo 一次一房
            //沒玩遊戲所以只能聊天
            _chatRoomService.JoinRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(new { message = "OK" });
        }
        /// <summary>
        /// 離開遊戲房間
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostLeaveRoom(string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //離開聊天室
            _chatRoomService.LeaveRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room))
            {
                TexasHoldThemService.Leave(room, currentId);
            }

            return Ok(new { message = "OK" });
        }

        /// <summary>
        /// 開房
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostCreateRoom(string roomNumber,ulong bigBlind, ulong smallBlind)
        {
            if (!TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room))
            {
                room = new TexasHoldThemModel(bigBlind, smallBlind);
                TexasHoldThemService.PokerRoomDic.TryAdd(roomNumber, room);
                //加入聊天室
                _chatRoomService.CreateRoom(roomNumber);
                _chatRoomService.JoinRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            return Ok(new { message = "OK" });
        }
        /// <summary>
        /// 帶籌碼入座
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="seat"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostSitDown(string roomNumber,int seat,ulong chips)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room))
            {
                TexasHoldThemService.SitDown(room, currentId, seat, chips);
            }
   
            // 返回 200 OK，不返回任何內容
            return Ok(); // 或者你可以返回 new { message = "OK" } 以便於客戶端確認
        }
    }
}
