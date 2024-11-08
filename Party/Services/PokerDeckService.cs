using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Party.Services
{
    public class PokerDeckService
    {
        public class Card
        {
            public char Suit { get; set; }
            public byte Num { get; set; }

            public Card(char suit, byte num)
            {
                Suit = suit;
                Num = num;
            }

            public override string ToString()
            {
                return $"{Num} of {Suit}";
            }
        }

        public class Deck
        {
            private List<Card> cards = new List<Card>();
            private Random random = new Random();
            private static bool first = true;
            private static List<Card>? _staticCards;
            public Deck()
            {
                if (first)
                {
                    char[] suits = { 'H', 'D', 'C', 'S' };
                    byte[] ranks = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

                    foreach (var suit in suits)
                    {
                        foreach (var rank in ranks)
                        {
                            cards.Add(new Card(suit, rank));
                        }
                    }
                    _staticCards = cards.ToList();
                    first = false;
                }
                else {
                    cards = _staticCards.ToList();
                }
            }
            /// <summary>
            /// 洗牌
            /// </summary>
            public void Shuffle()
            {
                int n = cards.Count;
                while (n > 1)
                {
                    int k = random.Next(n--);
                    var temp = cards[n];
                    cards[n] = cards[k];
                    cards[k] = temp;
                }
            }
            /// <summary>
            /// 發牌(x)張
            /// </summary>
            /// <param name="numberOfCards"></param>
            /// <returns></returns>
            public List<Card> Deal(int numberOfCards)
            {
                List<Card> hand = new List<Card>();

                for (int i = 0; i < numberOfCards && cards.Count > 0; i++)
                {
                    hand.Add(cards[0]);
                    cards.RemoveAt(0);
                }

                return hand;
            }
        }

        public enum Level : byte
        {
            /// <summary>
            /// 同花大順
            /// </summary>
            RoyalFlush = 11,
            /// <summary>
            /// 同花順
            /// </summary>
            StraightFlush=10,            
            /// <summary>
            /// 4條
            /// </summary>
            FourOfKind=9,
            /// <summary>
            /// 葫蘆
            /// </summary>
            Fullhouse = 8,
            /// <summary>
            /// 同花
            /// </summary>
            Flush = 7,
            /// <summary>
            /// 順子
            /// </summary>
            Straight=6,
            /// <summary>
            /// 三條
            /// </summary>
            ThressOfKind=5,
            /// <summary>
            /// 兩對
            /// </summary>
            TwoPair=4,
            /// <summary>
            /// 一對
            /// </summary>
            OnePair = 3,
            /// <summary>
            /// 單張
            /// </summary>
            One =2
        }

    }

}
