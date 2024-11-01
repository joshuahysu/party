using static Party.Services.PokerDeckService;

namespace Party.Models
{
    internal class PokerUser
    {
        public ulong Id { get; set; }

        /// <summary>
        /// 遊戲內籌碼
        /// </summary>
        public ulong Chips { get; set; }

        /// <summary>
        /// 下注籌碼
        /// </summary>
        public ulong BoardChips { get; set; }
        /// <summary>
        /// 座位
        /// </summary>
        public int Seat { get; set; }
        /// <summary>
        /// IsAllIn
        /// </summary>
        public bool IsAllIn { get; set; }
        /// <summary>
        /// 手牌
        /// </summary>
        public List<Card> Hand { get; set; }


    }
}