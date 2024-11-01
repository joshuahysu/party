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
    public class TexasHoldThemService
    {
        /// <summary>
        /// 房間對遊戲
        /// </summary>
        public static ConcurrentDictionary<String, TexasHoldThemService> PokerRoomDic = new();

        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<TexasHoldThemService> _localizer;
        /// <summary>
        /// 起始玩家
        /// </summary>
        private int _starter;

        /// <summary>
        /// 目前最大下注
        /// </summary>
        private ulong _allInChips;
        /// <summary>
        /// 檯面總數
        /// </summary>
        private ulong _allBoardChips;
        /// <summary>
        /// 最後加注玩家
        /// </summary>
        private int _addBetPlayer;
        private Deck _deck;
        private List<Card> _boardCards;
        private int _nowRound;
        /// <summary>
        /// 座位對玩家
        /// </summary>
        private Dictionary<int, PokerUser> PokerUserDic = new();
        /// <summary>
        /// ID對玩家
        /// </summary>
        private Dictionary<ulong, PokerUser> IDPokerUserDic = new();

        public TexasHoldThemService(ApplicationDbContext context, IStringLocalizer<TexasHoldThemService> localizer)
        {    
            _localizer = localizer;
            _context = context;
            var localizerFactory = Program.ServiceProvider.GetService<IStringLocalizerFactory>();
            localizerFactory?.Create(typeof(TexasHoldThemService).FullName, typeof(TexasHoldThemService).Assembly.GetName().Name);         
        }

        public void Start() {
            _deck = new Deck();
            _deck.Shuffle();
            _nowRound = 0;
            foreach( var item in PokerUserDic.Values)
            {
                item.Hand = _deck.Deal(2);
            }
        }

        /// <summary>
        /// 買籌碼
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cost"></param>
        /// <param name="pointCost"></param>
        /// <param name="chips"></param>
        public void BuyIn(ulong id,decimal cost, decimal pointCost, ulong chips)
        {            
            TradeService.BuyIn(id,cost,pointCost,"Poker Buy In",chips);            
        }
        public void SitDown(ulong id, int seat, ulong chips)
        {
            using (ApplicationDbContext _context = new())
            {
                var user = _context.UserAccount.Find(id);
            
            var newUser = new PokerUser { Id = id, Seat = seat,Chips=user.Chips };
            PokerUserDic.Add(seat, newUser);
            IDPokerUserDic.Add(id, newUser);
            }
        }
        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="chips"></param>
        /// <returns></returns>
        public string Bet(ulong id,ulong chips)
        {
            IDPokerUserDic.TryGetValue(id, out var player);

            var tempChips = player.BoardChips + chips;
            if (chips > player.Chips) {
                return "超出籌碼上限";//error
            }

            if (chips == player.Chips) {
                AllIn(id);
                return "";
            }

            //加注
            if (tempChips > _allInChips)
            {
                //todo驗證數量
                player.BoardChips = tempChips;
                _allInChips = tempChips;
                _addBetPlayer = player.Seat;
            }
            //跟住
            else if (tempChips == _allInChips)
            {
                player.BoardChips = tempChips;
                //todo 最後一個跟注，要進下一輪
                _boardCards.Round(ref _nowRound, _deck);
            }
            else if (tempChips <_allInChips)
            {
                //error
                return"下注數不夠";
            }

            player.Chips -= chips;
            _allBoardChips += chips;

            return "";
        }

        public void AllIn(ulong id)
        {
            IDPokerUserDic.TryGetValue(id, out var player);
            player.IsAllIn = true;
            player.BoardChips += player.Chips;
            _allBoardChips += player.Chips;
            player.Chips = 0;
            if (player.BoardChips > _allInChips)
            {
                _allInChips = player.BoardChips;
            }

        }
        public static int compare()
        {

            return 1;
        }
        public int End(ulong winUser)
        {
            foreach (var user in PokerUserDic.Values) {
                if (user.Id == winUser)
                {
                    user.Chips += _allBoardChips;
                }
                else {
                
                }
            }
            return 1;
        }
        public int Leave(ulong id)
        {
            IDPokerUserDic.TryGetValue (id, out var player);
            using (ApplicationDbContext _context = new())
            {
                //var user = _context.UserAccount.Find(id);
                var tempChips=player.Chips;
                player.Chips = 0;  
                
                _context.Database.ExecuteSqlInterpolated($"UPDATE Users SET Chips = Chips + {tempChips} WHERE Id = {id}");
                //user.Chips += tempChips;
                //_context.SaveChanges();                  
            }

            IDPokerUserDic.Remove (id);
            return 1;
        }
    }

    public class CardCount() {
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
