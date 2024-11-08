using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using Party.Unit;
using System.Collections.Concurrent;
using static Party.Services.PokerDeckService;

namespace Party.Services
{
    public class TexasHoldThemModel
    {
        public TexasHoldThemModel(ulong bigBlind, ulong smallBlind) {
            BigBlind = bigBlind;
            SmallBlind = smallBlind;
        }
        /// <summary>
        /// 起始玩家(小盲玩家)
        /// </summary>
        public int _starter;

        /// <summary>
        /// 當輪最大下注
        /// </summary>
        public ulong NowRoundBetChips;

        /// <summary>
        /// 目前最小加注
        /// </summary>
        public ulong _nowMinAddChips;

        /// <summary>
        /// 最小加注
        /// </summary>
        public readonly ulong _minAddChips;
        /// <summary>
        /// 小盲
        /// </summary>
        public readonly ulong SmallBlind;
        /// <summary>
        /// 大盲
        /// </summary>
        public readonly ulong BigBlind;
        /// <summary>
        /// 檯面總數
        /// </summary>
        public ulong _allBoardChips;
        /// <summary>
        /// 最後加注玩家座位
        /// </summary>
        public int AddBetPlayerSeat;

        /// <summary>
        /// 當前玩家
        /// </summary>
        public int NowBetPlayerSeat;
        /// <summary>
        /// 牌堆
        /// </summary>
        public Deck? _deck;
        /// <summary>
        /// 檯面上的牌
        /// </summary>
        public List<Card> BoardCards { get; set; } = new();
        /// <summary>
        /// 當前回合
        /// </summary>
        public int _nowRound;

        /// <summary>
        /// 座位對玩家
        /// </summary>
        public PokerUser[] PokerUserArr { get; set; } =new PokerUser[12];
        /// <summary>
        /// ID對玩家
        /// </summary>
        public Dictionary<ulong, PokerUser> IDPokerUserDic = new();
    }
}
