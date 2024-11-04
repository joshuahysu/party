using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using Party.Services;
using Party.Services.ChatRoom;
using Party.Services.TexasHoldThem;
using System;
using System.Diagnostics;
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

        // POST: api/Trace
        [HttpPost]
        public async Task<IActionResult> PostBet(int chips,string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room);
            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
        // POST: api/BuyIn
        [HttpPost]
        public async Task<IActionResult> PostBuyIn(int chips, string roomNumber)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room)) {
                room = new TexasHoldThemModel();
                TexasHoldThemService.PokerRoomDic.TryAdd(roomNumber, room);
            }
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
            //�S���C���ҥH�u����
            _chatRoomService.JoinRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(new { message = "OK" });
        }
        /// <summary>
        /// �}��
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostCreateRoom(string roomNumber)
        {
            if (!TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room))
            {
                room = new TexasHoldThemModel();
                TexasHoldThemService.PokerRoomDic.TryAdd(roomNumber, room);
                //�[�J��ѫ�
                _chatRoomService.CreateRoom(roomNumber);
                _chatRoomService.JoinRoom(roomNumber, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            return Ok(new { message = "OK" });
        }
        /// <summary>
        /// �J�y
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="seat"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostSitDown(string roomNumber,int seat)
        {
            var currentId = Convert.ToUInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!TexasHoldThemService.PokerRoomDic.TryGetValue(roomNumber, out var room))
            {
                room = new TexasHoldThemModel();
                TexasHoldThemService.PokerRoomDic.TryAdd(roomNumber, room);
            }
   
            // ��^ 200 OK�A����^���󤺮e
            return Ok(); // �Ϊ̧A�i�H��^ new { message = "OK" } �H�K��Ȥ�ݽT�{
        }
    }
}
