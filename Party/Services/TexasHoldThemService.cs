using Microsoft.Extensions.Localization;
using Party.Data;
using Party.Models;
using static Party.Services.PokerDeckService;

namespace Party.Services
{
    public class TexasHoldThemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<TexasHoldThemService> _localizer;
        private int _starter;
        private int _allInChips;
        private int _addBetPlayer;
        private Deck _deck;
        private List<Card> _boardCards;
        Dictionary<int, PokerUser> PokerUserDic = new();
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
            foreach( var item in PokerUserDic.Values)
            {
                item.Hand = _deck.Deal(2);
            }
        }

        public List<Card> Round(int round)
        {
            if (round == 1)
            {
                _boardCards = _deck.Deal(3);
            }
            else {
                _boardCards.AddRange(_deck.Deal(1));                
            }
            return _boardCards;
        }


        public void BuyIn(ulong id,int seat, decimal cost, decimal pointCost, int chips)
        {
            TradeService.Buy(id,cost,pointCost,"Poker Buy In");
            PokerUserDic.Add(seat, new PokerUser {Id=id,Chips=chips });
        }
   
        public string  Bet(ulong id, int seat,int chips,bool isAllIn=false)
        {
            PokerUserDic.TryGetValue(seat, out var player);
            if (player.Id != id) {
                //驗證失敗
                return"位置不同人";
            }

            var tempChips = player.BoardChips + chips;
            if (tempChips > player.Chips) {
                return "超出籌碼上限";//error
            }

            //all in
            if (isAllIn) {
                tempChips=player.Chips;
            }
            //加注
            if (tempChips > _allInChips)
            {
                //todo驗證數量
                player.BoardChips = tempChips;
                _allInChips = tempChips;
                _addBetPlayer =seat;
            }
            //跟住
            else if (tempChips == _allInChips)
            {
                player.BoardChips = tempChips;
            }
            else if (tempChips <_allInChips)
            {
                //error
                return"下注數不夠";
            }
            return"";
        }

        public HandCards Big(List<Card> boardAndHand) {
            Dictionary<int, byte> countCards = new Dictionary<int,byte>();
            Dictionary<char, List<Card>> countColor = new Dictionary<char, List<Card>>();
            (byte num, byte count) max1 =(0,1);
            (byte num, byte count) max2 = (0, 1);
            (byte num, byte count) max3 = (0, 1);

            foreach (var card in boardAndHand) {
                if (countCards.TryGetValue(card.Num, out byte count))
                {
                    count += 1;
                    if (count > max1.count) {
                        max1.count = count;
                        max1.num = card.Num;
                        if (max1.count == 4)
                            return new HandCards{ Rank = Level.FourOfKind,RankNum1= max1.num };//一定是4條
                    }
                    else if (count > max2.count)
                    {
                        //3 2 2
                        max2.count = count;
                        max2.num = card.Num;     
                    }
                    else if (count > max3.count)
                    {
                        max3.count = count;
                        max3.num = card.Num;
              
                    }
                }
                else {
                    countCards[card.Num] = 1;
                }
                if (countColor.TryGetValue(card.Suit, out List<Card> listCard))
                {
                    listCard.Add(card);
                }
                else
                {
                    countColor[card.Suit] = new List<Card>();
                }
            }
            if (max1.count == 3 && max2.count == 3)
            {
                if (max2.num > max1.num)
                {
                    Swap(ref max2, ref max1);
                }
                return new HandCards { Rank = Level.Fullhouse, RankNum1 = max1.num, RankNum2 = max2.num };//一定是葫蘆
            }

            if (max3.count == 2)
            {
                if (max1.count == 3)
                {
                    //3 2 2                                
                    if (max3.num > max2.num)
                    {
                        Swap(ref max2, ref max3);
                    }
                    new HandCards { Rank = Level.Fullhouse,RankNum1 = max1.num, RankNum2 = max2.num };
                }
                else
                {
                    //2 2 2
                    if (max3.num > max2.num)
                    {
                        Swap(ref max2, ref max3);
                    }
                    if (max2.num > max1.num)
                    {
                        Swap(ref max2, ref max1);
                    }
                    if (max3.num > max2.num)
                    {
                        Swap(ref max2, ref max3);
                    }
                    //,RankNum3= max3.num todo 
                    new HandCards { Rank = Level.TwoPair, RankNum1 = max1.num, RankNum2 = max2.num };
                }
            }
            else if (max1.count == 3 && max2.count == 2) {
                new HandCards { Rank = Level.Fullhouse, RankNum1 = max1.num, RankNum2 = max2.num };
            }
            else if (max1.count == 3 && max2.count == 1)
            {
                //boardAndHand
                //todo
                new HandCards { Rank = Level.ThressOfKind };
            }
            // 篩選出 List<Card> 的 Count > 4 並對其進行倒序
            Dictionary<char, List<Card>> filteredCountColor = countColor
                .Where(pair => pair.Value.Count > 4)
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.OrderByDescending(card => card.Num).ToList()
                );

                  
            if (filteredCountColor.Any()) {
                var ssg = filteredCountColor.First();
                //有同花
                for (int i = 0; i < filteredCountColor.Count-4; i++)
                {
                    if (ssg.Value[i].Num - 4 == ssg.Value[i+4].Num)
                    {
                        return new HandCards { Rank = Level.StraightFlush, RankNum1 = ssg.Value[i].Num };//同花順
                    }
                }
            }
            var sortedCards = boardAndHand.OrderBy(card => card.Num).ToList();
            for (int i = 0; i < sortedCards.Count - 4; i++)
            {
                if (sortedCards[i].Num - 4 == sortedCards[i + 4].Num)
                {
                    return new HandCards { Rank = Level.Straight,RankNum1= sortedCards[i].Num };//順
                }
            }
            return new HandCards();
        }
        // 交換方法，使用 ref 傳遞元組參數
        public static void Swap(ref (byte num, byte count) a, ref (byte num, byte count) b)
        {
            var temp = a;
            a = b;
            b = temp;
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
