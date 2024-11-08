using Party.Models;
using Party.Services;
using Party.Services.TexasHoldThem;
using static Party.Services.PokerDeckService;

namespace Party.Unit
{
    public static class CardExtensions
    {
        public static HandCards GetBiggestCards(this List<Card> hand, List<Card> board)
        {
            var handAndBoard = hand.Concat(board).ToList(); ;
            SortedDictionary<int, byte> countCards = new SortedDictionary<int, byte>();
            Dictionary<char, List<Card>> countColor = new Dictionary<char, List<Card>>();
            HandCards maxHandCards = new();
            (byte num, byte count) max1 = (0, 1);
            (byte num, byte count) max2 = (0, 1);
            (byte num, byte count) max3 = (0, 1);

            foreach (var card in handAndBoard)
            {
                if (countCards.TryGetValue(card.Num, out byte count))
                {
                    count += 1;
                    if (count > max1.count)
                    {
                        max1.count = count;
                        max1.num = card.Num;
                        if (max1.count == 4)
                        {
                            var handCards = new HandCards { Rank = Level.FourOfKind };//一定是4條,不可能是同花順
                            handCards.RankNumArr[0] = max1.num;
                            return handCards;
                        }
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

                //用於計算同花
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
                //3 3 1
                if (max2.num > max1.num)
                {
                    Swap(ref max2, ref max1);
                }
                var handCards=new HandCards { Rank = Level.Fullhouse};//一定是葫蘆
                handCards.RankNumArr[0] = max1.num;
                handCards.RankNumArr[1] = max2.num;
                return handCards;
            }

            if (max1.count == 3 && max3.count == 2)
            { 
                    //3 2 2                                
                    if (max3.num > max2.num)
                    {
                        Swap(ref max2, ref max3);
                    }
                    var handCards = new HandCards { Rank = Level.Fullhouse };//一定是葫蘆
                    handCards.RankNumArr[0] = max1.num;
                    handCards.RankNumArr[1] = max2.num;
                    return handCards;  
            }
            else if (max1.count == 3 && max2.count == 2)
            {
                //3 2 1 1
                maxHandCards = new HandCards { Rank = Level.Fullhouse};//一定是葫蘆

                maxHandCards.RankNumArr[0] = max1.num;
                maxHandCards.RankNumArr[1] = max2.num;
                return maxHandCards;
            }
          
            // 篩選出同花 List<Card> 的 Count > 4 並對其進行倒序
            Dictionary<char, List<Card>> filteredCountColor = countColor
                .Where(pair => pair.Value.Count > 4)
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.OrderByDescending(card => card.Num).ToList()
                );

            //代表有同花
            if (filteredCountColor.Any())
            {
                KeyValuePair<char, List<Card>> flushCards = filteredCountColor.First();
                //有同花
                for (int i = 0; i < filteredCountColor.Count - 4; i++)
                {
                    if (flushCards.Value[i].Num - 4 == flushCards.Value[i + 4].Num)
                    {
                        var tempHandCards = new HandCards { Rank = Level.StraightFlush };//同花順
                        tempHandCards.RankNumArr[0] = flushCards.Value[i].Num;
                        return tempHandCards;

                    }
                }
 
                var handCards = new HandCards { Rank = Level.Flush, Cards = flushCards.Value };//同花比大小
                for (int i = 0; i < 5; i++)
                {
                    handCards.RankNumArr[i] = flushCards.Value[i].Num;
                }
                return handCards;

            }


            var sortedCards = handAndBoard.OrderBy(card => card.Num).ToList();
            for (int i = 0; i < sortedCards.Count - 4; i++)
            {
                if (sortedCards[i].Num - 4 == sortedCards[i + 4].Num)
                {
                    var handCards = new HandCards { Rank = Level.Straight };//順
                    handCards.RankNumArr[0] = sortedCards[i].Num;
                    return handCards;
                }
            }

            if (max1.count == 3) //等同(max1.count == 3 && max2.count == 1)
            {
                //3 1 1 1 1
                maxHandCards = new HandCards { Rank = Level.ThressOfKind };
                maxHandCards.RankNumArr[0] = max1.num;
                countCards.Remove(max1.num);
                maxHandCards.RankNumArr[1] = countCards.Values.ElementAt(countCards.Count - 1);
                maxHandCards.RankNumArr[2] = countCards.Values.ElementAt(countCards.Count - 2);

            }
            else if (max1.count == 2 && max2.count == 2)
            {
                if (max3.count == 2)
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

                    maxHandCards = new HandCards { Rank = Level.TwoPair };

                    maxHandCards.RankNumArr[0] = max1.num;
                    maxHandCards.RankNumArr[1] = max2.num;
                    countCards.Remove(max1.num);
                    countCards.Remove(max2.num);
                    maxHandCards.RankNumArr[2] = countCards.Values.ElementAt(countCards.Count - 1);
                }
                else
                {
                    //2 2 1 1 1
                    if (max2.num > max1.num)
                    {
                        Swap(ref max2, ref max1);
                    }
                    maxHandCards = new HandCards { Rank = Level.TwoPair };

                    maxHandCards.RankNumArr[0] = max1.num;
                    maxHandCards.RankNumArr[1] = max2.num;
                    countCards.Remove(max1.num);
                    countCards.Remove(max2.num);
                    maxHandCards.RankNumArr[2] = countCards.Values.ElementAt(countCards.Count - 1);
                }
            }
            else if (max1.count == 2)
            {
                //2 1 1 1 1 1
                maxHandCards = new HandCards { Rank = Level.OnePair };

                maxHandCards.RankNumArr[0] = max1.num;
                countCards.Remove(max1.num);
                for (int i = 1; i < 5; i++)
                {
                    maxHandCards.RankNumArr[i] = countCards.Values.ElementAt(countCards.Count - i);
                }
            }
            else//等同 if (max1.count == 1)
            {
                maxHandCards = new HandCards { Rank = Level.One };
                for (int i = 0; i < 5; i++)
                {
                    maxHandCards.RankNumArr[i] = countCards.Values.ElementAt(countCards.Count - 1);
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
        /// <summary>
        /// 下一位玩家
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static PokerUser NextPlayer(this TexasHoldThemModel room)
        {
            var nextSeat = room.NowBetPlayerSeat;//next需要+1但arr又-1
            PokerUser player;
            if (nextSeat < room.PokerUserArr.Count())
            {
                player = room.PokerUserArr[nextSeat];
            }
            else
            {
                player = room.PokerUserArr[0];
            }
            room.NowBetPlayerSeat = nextSeat+1;
            return player;
        }

    }
}
