using static Party.Services.PokerDeckService;

namespace Party.Models
{
    internal class PokerUser
    {
        public ulong Id { get; set; }

        /// <summary>
        /// 遊戲內籌碼
        /// </summary>
        public int Chips { get; set; }

        /// <summary>
        /// 下注籌碼
        /// </summary>
        public int BoardChips { get; set; }
        /// <summary>
        /// 座位
        /// </summary>
        public int Seat { get; set; }
        /// <summary>
        /// 手牌
        /// </summary>
        public List<Card> Hand { get; set; }


    }
}