using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using Party.Unit;
using System.Collections.Concurrent;
using System.Numerics;
using static Party.Services.PokerDeckService;

namespace Party.Services.TexasHoldThem
{
    public class TexasHoldThemService
    {
        /// <summary>
        /// 房間對遊戲
        /// </summary>
        public static ConcurrentDictionary<string, TexasHoldThemModel> PokerRoomDic = new();

        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<TexasHoldThemService> _localizer;
 
      

        public TexasHoldThemService(ApplicationDbContext context, IStringLocalizer<TexasHoldThemService> localizer)
        {
            _localizer = localizer;
            _context = context;
            var localizerFactory = Program.ServiceProvider.GetService<IStringLocalizerFactory>();
            localizerFactory?.Create(typeof(TexasHoldThemService).FullName, typeof(TexasHoldThemService).Assembly.GetName().Name);
        }

        public static void Start(TexasHoldThemModel room)
        {
            //重製
            room._deck = new Deck();
            room._deck.Shuffle();          
            room._nowMinAddChips = room._minAddChips;
            room._nowRound = 1;
            foreach (var item in room.PokerUserArr)
            {
                item.Hand = room._deck.Deal(2);
            }

        }

        /// <summary>
        /// 買籌碼
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cost"></param>
        /// <param name="pointCost"></param>
        /// <param name="chips"></param>
        public static void BuyIn(ulong id, decimal cost, decimal pointCost, ulong chips)
        {
            TradeService.BuyIn(id, cost, pointCost, "Poker Buy In", chips);
        }
        /// <summary>
        /// 進入位子
        /// </summary>
        /// <param name="room"></param>
        /// <param name="id"></param>
        /// <param name="seat"></param>
        /// <param name="chips"></param>
        public void SitDown(TexasHoldThemModel room,ulong id, int seat, ulong chips)
        {
            using (ApplicationDbContext _context = new())
            {
                var user = _context.UserAccount.Find(id);

                var newUser = new PokerUser { Id = id, Seat = seat, Chips = user.Chips };
                room.PokerUserArr[seat]=newUser;
                room.IDPokerUserDic.Add(id, newUser);
            }
        }
        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="chips"></param>
        /// <returns></returns>
        public static string Bet(TexasHoldThemModel room,ulong id, ulong chips)
        {
            room.IDPokerUserDic.TryGetValue(id, out var player);

            var tempChips = player.NowRoundChips + chips;
            if (chips > player.Chips)
            {
                return "超出籌碼上限";//error
            }

            //All in
            if (chips == player.Chips)
            {
                player.IsAllIn = true;        

                //有加注
                if (tempChips > room.NowRoundBetChips)
                {
                    //修改最小加注
                    room._nowMinAddChips = tempChips - room.NowRoundBetChips;

                    room.NowRoundBetChips = tempChips;

                    room._addBetPlayerSeat = player.Seat;
                }

            }
            //加注
            else if (tempChips > room.NowRoundBetChips + room._nowMinAddChips)
            {
                //修改最小加注
                room._nowMinAddChips = tempChips- room.NowRoundBetChips;

                room.NowRoundBetChips = tempChips;
                room._addBetPlayerSeat = player.Seat;
            }
            //跟住
            else if (tempChips == room.NowRoundBetChips)
            {
                //todo 最後一個跟注，要進下一輪
        
            }
            else if (tempChips < room.NowRoundBetChips)
            {
                //error
                return "下注數不夠";
            }
            player.NowRoundChips = tempChips;
            player.Chips -= chips;
            player.AllBoardChips += chips;
            room._allBoardChips += chips;          
            return "下注成功";
        }

        public static int compare()
        {

            return 1;
        }

        public static void NextPlayer(TexasHoldThemModel room, ulong id)
        {      
            PokerUser player;
            while (true)
            {
                player = room.NextPlayer();
                if (player.IsFold) continue;

                if (player.Seat == room._addBetPlayerSeat)
                {
                    //下一round
                    break;
                }

                if (!player.IsAllIn)
                {
                    break;
                }
            }
        }

        public static int NextRound(TexasHoldThemModel room)
        {
            if (room._nowRound == 3) {
                //結束開始比大小
            }
            else if (room._nowBetPlayerSeat == room._addBetPlayerSeat) {
                //....todo
                room._nowRound++;
                //下一輪
                // 将新发的牌添加到现有的 List<Card>
                room._boardCards.AddRange(room._deck.Deal(1));
            }
            return 1;
        }
        public static int BetProcess(TexasHoldThemModel room, ulong id, ulong chips)
        {
            Bet(room, id, chips);
            NextRound(room);
            return 1;
        }

        public int End(TexasHoldThemModel room, ulong winUser)
        {
            foreach (var user in room.PokerUserArr)
            {
                if (user.Id == winUser)
                {
                    user.Chips += room._allBoardChips;
                }
                else
                {

                }
            }
            return 1;
        }
        public int Leave(TexasHoldThemModel room, ulong id)
        {
            room.IDPokerUserDic.TryGetValue(id, out var player);
            using (ApplicationDbContext _context = new())
            {
                //var user = _context.UserAccount.Find(id);
                var tempChips = player.Chips;
                player.Chips = 0;

                _context.Database.ExecuteSqlInterpolated($"UPDATE Users SET Chips = Chips + {tempChips} WHERE Id = {id}");
                //user.Chips += tempChips;
                //_context.SaveChanges();                  
            }

            room.IDPokerUserDic.Remove(id);
            return 1;
        }
    }

    public class CardCount()
    {
        public int Count { get; private set; }
        public int Rank { get; private set; }
        public char Color { get; private set; }
    }
    public class HandCards()
    {
        public Level Rank { get; set; }
        public List<Card> Cards { get; set; }

        public byte RankNum1 { get; set; }

        public byte RankNum2 { get; set; }

        public byte RankNum3 { get; set; }

    }
}
