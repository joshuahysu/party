using Party.Services.TexasHoldThem;
using static Party.Services.PokerDeckService;

namespace Party.Models
{
    public class PokerUser
    {
        public ulong Id { get; set; }

        /// <summary>
        /// 玩家房間內籌碼
        /// </summary>
        public ulong RoomChips { get; set; }

        /// <summary>
        /// 當輪下注籌碼
        /// </summary>
        public ulong NowRoundChips { get; set; }

        /// <summary>
        /// 總下注籌碼
        /// </summary>
        public ulong AllBoardChips { get; set; }
        /// <summary>
        /// 座位
        /// </summary>
        public int Seat { get; set; }
        /// <summary>
        /// IsAllIn
        /// </summary>
        public bool IsAllIn { get; set; }

        /// <summary>
        /// 是否棄牌
        /// </summary>
        public bool IsFold { get; set; }
        /// <summary>
        /// 手牌
        /// </summary>
        public List<Card> Hand { get; set; }
        /// <summary>
        /// 最大牌型
        /// </summary>
        public HandCards BiggestCards { get; set; }
        
    }
}