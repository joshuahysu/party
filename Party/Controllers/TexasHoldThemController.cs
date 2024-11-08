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
        /// �U�`
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="chips">0�O��`</param>
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
            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
        /// <summary>
        /// ��P
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
            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
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

            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
        // POST: api/BuyIn
        [HttpPost]
        public async Task<IActionResult> PostBuyIn(int chips, string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));

            TexasHoldThemService.BuyIn(currentId);

            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
        /// <summary>
        /// �[�J�C���ж�
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostJoinRoom(string roomNumber)
        {
            //todo �@���@��
            //�S���C���ҥH�u����
            _chatRoomService.JoinRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(new { message = "OK" });
        }
        /// <summary>
        /// ���}�C���ж�
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostLeaveRoom(string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //���}��ѫ�
            _chatRoomService.LeaveRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room))
            {
                TexasHoldThemService.Leave(room, currentId);
            }

            return Ok(new { message = "OK" });
        }

        /// <summary>
        /// �}��
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
                //�[�J��ѫ�
                _chatRoomService.CreateRoom(roomNumber);
                _chatRoomService.JoinRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            return Ok(new { message = "OK" });
        }
        /// <summary>
        /// �a�w�X�J�y
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
   
            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
    }
}
