using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using Party.Unit;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
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

        /// <summary>
        /// 開始遊戲
        /// </summary>
        /// <param name="room"></param>
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
        public static void SitDown(TexasHoldThemModel room,ulong id, int seat, ulong chips)
        {
            using (ApplicationDbContext _context = new())
            {
                var user = _context.UserAccount.Find(id);

                var newUser = new PokerUser { Id = id, Seat = seat, RoomChips = user.Chips };
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
            PokerUser player =room.IDPokerUserDic[id];

            var tempChips = player.NowRoundChips + chips;
            if (chips > player.RoomChips)
            {
                return "超出籌碼上限";//error
            }

            //All in
            if (chips == player.RoomChips)
            {
                player.IsAllIn = true;        

                //有加注
                if (tempChips > room.NowRoundBetChips)
                {
                    //修改最小加注
                    room._nowMinAddChips = tempChips - room.NowRoundBetChips;

                    room.NowRoundBetChips = tempChips;

                    room.AddBetPlayerSeat = player.Seat;
                }

            }
            //加注
            else if (tempChips > room.NowRoundBetChips + room._nowMinAddChips)
            {
                //修改最小加注
                room._nowMinAddChips = tempChips- room.NowRoundBetChips;

                room.NowRoundBetChips = tempChips;
                room.AddBetPlayerSeat = player.Seat;
            }

            //跟住(不用額外做事)
            //else if (tempChips == room.NowRoundBetChips)
            //{        
            //}

            else if (tempChips < room.NowRoundBetChips)
            {
                //error
                return "下注數不夠";
            }
            player.NowRoundChips = tempChips;
            player.RoomChips -= chips;
            player.AllBoardChips += chips;
            room._allBoardChips += chips;          
            return "下注成功";
        }
        /// <summary>
        /// 回傳牌比較大的玩家
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        public static PokerUser Compare(PokerUser player1, PokerUser player2)
        {
            if (player1.BiggestCards.Rank > player2.BiggestCards.Rank) {
                return player1;
            }
            else if (player1.BiggestCards.Rank < player2.BiggestCards.Rank)
            {
                return player2;
            }
            else// if (player1.Rank == player2.Rank)
            {
                int num = 0;
                return GetHigherRankPlayer(player1, player2, ref num);
            }           
        }

        public static PokerUser NextPlayer(TexasHoldThemModel room)
        {      
            PokerUser player;

            while (true)
            {
                player = room.NextPlayer();

                if (player.Seat == room.AddBetPlayerSeat)//如果下一round
                {
                    return NextRound(room);
                }

                if (player.IsFold) continue;


                if (!player.IsAllIn)
                {
                    return player;
                }               
            }
        }
        /// <summary>
        /// 玩家牌比大小
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="num">比較第幾個</param>
        /// <returns></returns>
        static PokerUser GetHigherRankPlayer(PokerUser player1, PokerUser player2,ref int num)
        {
            if (player1.BiggestCards.RankNumArr[num] > player2.BiggestCards.RankNumArr[num])
            {
                return player1;  // player1 排名較高
            }
            else if (player1.BiggestCards.RankNumArr[num] < player2.BiggestCards.RankNumArr[num])
            {
                return player2;  // player2 排名較高
            }
            else if (num==4)
            {
                return null;//平手
            }
            else
            {
                num++;
                return GetHigherRankPlayer(player1, player2,ref num);//平手繼續比較
            }

        }

        public static PokerUser NextRound(TexasHoldThemModel room)
        {
            if (room._nowRound == 3) {
                //當輪結束，開始比大小
                PokerUser? winPlayer =null;
                for (int i = 0; i < room.PokerUserArr.Count(); i++)
                {
                    var pokerUser=room.PokerUserArr[i];
                    if (!pokerUser.IsFold)
                    {
                        if (winPlayer != null) {
                            winPlayer=Compare(winPlayer, pokerUser);
                        }
                        else
                        {
                            winPlayer = pokerUser;
                            winPlayer.BiggestCards = winPlayer.Hand.GetBiggestCards(room.BoardCards);
                        }
                    }
                }
                End(room, winPlayer);
                return null;
            }

            room._nowRound++;
            //下一輪 發一張牌            
            room.BoardCards.AddRange(room._deck.Deal(1));

            //新一輪回到起始玩家
            room.NowBetPlayerSeat = room._starter;
            var player = room.PokerUserArr[room.NowBetPlayerSeat - 1];

            while (true)
            {
                if (!player.IsFold) break;
                player = room.NextPlayer();
            }
            room.AddBetPlayerSeat = player.Seat;
            room.NowBetPlayerSeat = player.Seat;
            return player;
        }
        public static int BetProcess(TexasHoldThemModel room, ulong id, ulong chips)
        {
            Bet(room, id, chips);
            //回傳下一位玩家
            NextPlayer(room);
            return 1;
        }

        public static int FoldProcess(TexasHoldThemModel room, ulong id)
        {
            //棄牌
            room.IDPokerUserDic[id].IsFold = true;

            //都棄完牌剩一人直接勝利
            if (CountAlivePlayer(room) == 1)
            {
                PokerUser play = NextNotFoldPlayer(room);
                End(room, play);
            }

            //回傳下一位玩家
            NextPlayer(room);
            return 1;
        }

        public static void End(TexasHoldThemModel room, PokerUser winPlayer)
        {
            winPlayer.RoomChips += room._allBoardChips;
        }
        public static int Leave(TexasHoldThemModel room, ulong id)
        {
            room.IDPokerUserDic.TryGetValue(id, out var player);
            using (ApplicationDbContext _context = new())
            {
                //var user = _context.UserAccount.Find(id);
                var tempChips = player.RoomChips;
                player.RoomChips = 0;

                _context.Database.ExecuteSqlInterpolated($"UPDATE Users SET Chips = Chips + {tempChips} WHERE Id = {id}");
                //user.Chips += tempChips;
                //_context.SaveChanges();                  
            }

            room.IDPokerUserDic.Remove(id);
            return 1;
        }

        public static int CountAlivePlayer(TexasHoldThemModel room) {
            int count = 0;
            foreach (var player in room.PokerUserArr) {
                if (player.IsFold == false) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 下一位非棄牌玩家
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static PokerUser NextNotFoldPlayer(TexasHoldThemModel room)
        {
            PokerUser player;
            while (true)
            {
                player = room.NextPlayer();
                if (!player.IsFold)
                {
                    break;
                }
            }
            return player;
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
        public byte[] RankNumArr { get; set; }= new byte[5];

    }
}
