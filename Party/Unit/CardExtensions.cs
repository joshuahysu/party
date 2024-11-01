using Party.Services;
using static Party.Services.PokerDeckService;

namespace Party.Unit
{
    public static class CardExtensions
    {
        public static HandCards Big(this List<Card> boardAndHand)
        {
            Dictionary<int, byte> countCards = new Dictionary<int, byte>();
            Dictionary<char, List<Card>> countColor = new Dictionary<char, List<Card>>();
            HandCards maxHandCards = new();
            (byte num, byte count) max1 = (0, 1);
            (byte num, byte count) max2 = (0, 1);
            (byte num, byte count) max3 = (0, 1);

            foreach (var card in boardAndHand)
            {
                if (countCards.TryGetValue(card.Num, out byte count))
                {
                    count += 1;
                    if (count > max1.count)
                    {
                        max1.count = count;
                        max1.num = card.Num;
                        if (max1.count == 4)
                            return new HandCards { Rank = Level.FourOfKind, RankNum1 = max1.num };//一定是4條
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
                else
                {
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
                    new HandCards { Rank = Level.Fullhouse, RankNum1 = max1.num, RankNum2 = max2.num };//一定是葫蘆
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
                    maxHandCards = new HandCards { Rank = Level.TwoPair, RankNum1 = max1.num, RankNum2 = max2.num };
                }
            }
            else if (max1.count == 3 && max2.count == 2)
            {
                maxHandCards = new HandCards { Rank = Level.Fullhouse, RankNum1 = max1.num, RankNum2 = max2.num };
            }
            else if (max1.count == 3 && max2.count == 1)
            {
                //boardAndHand
                //todo
                maxHandCards = new HandCards { Rank = Level.ThressOfKind };
            }
            // 篩選出 List<Card> 的 Count > 4 並對其進行倒序
            Dictionary<char, List<Card>> filteredCountColor = countColor
                .Where(pair => pair.Value.Count > 4)
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.OrderByDescending(card => card.Num).ToList()
                );


            if (filteredCountColor.Any())
            {
                KeyValuePair<char, List<Card>> flushCards = filteredCountColor.First();
                //有同花
                for (int i = 0; i < filteredCountColor.Count - 4; i++)
                {
                    if (flushCards.Value[i].Num - 4 == flushCards.Value[i + 4].Num)
                    {
                        return new HandCards { Rank = Level.StraightFlush, RankNum1 = flushCards.Value[i].Num };//同花順
                    }
                }
                return new HandCards { Rank = Level.Flush, Cards = flushCards.Value };//同花順todo 
            }
            var sortedCards = boardAndHand.OrderBy(card => card.Num).ToList();
            for (int i = 0; i < sortedCards.Count - 4; i++)
            {
                if (sortedCards[i].Num - 4 == sortedCards[i + 4].Num)
                {
                    return new HandCards { Rank = Level.Straight, RankNum1 = sortedCards[i].Num };//順
                }
            }
            return maxHandCards;
        }
        // 交換方法，使用 ref 傳遞元組參數
        public static void Swap(ref (byte num, byte count) a, ref (byte num, byte count) b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static List<Card> Round(this List<Card> boardCards,ref int round, Deck deck)
        {
            if (round == 1)
            {
                // 使用 deck.Deal(3) 创建一个新的 List<Card>
                boardCards = deck.Deal(3);
            }
            else
            {
                // 将新发的牌添加到现有的 List<Card>
                boardCards.AddRange(deck.Deal(1));
            }
            round++;
            return boardCards;
        }
    }

}
