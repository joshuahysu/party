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
        /// <summary>
        /// 起始玩家
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
        /// 起始注
        /// </summary>
        public readonly ulong _startChips;

        /// <summary>
        /// 最小加注
        /// </summary>
        public readonly ulong _minAddChips;

        /// <summary>
        /// 檯面總數
        /// </summary>
        public ulong _allBoardChips;
        /// <summary>
        /// 最後加注玩家座位
        /// </summary>
        public int _addBetPlayerSeat;

        /// <summary>
        /// 當前玩家
        /// </summary>
        public int _nowBetPlayerSeat;
        public Deck _deck;
        /// <summary>
        /// 檯面上的牌
        /// </summary>
        public List<Card> _boardCards;
        /// <summary>
        /// 當前回合
        /// </summary>
        public int _nowRound;

        /// <summary>
        /// 座位對玩家
        /// </summary>
        public PokerUser[] PokerUserArr;
        /// <summary>
        /// ID對玩家
        /// </summary>
        public Dictionary<ulong, PokerUser> IDPokerUserDic = new();
    }
}
