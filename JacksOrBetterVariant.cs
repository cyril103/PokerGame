using System.Collections.Generic;

namespace PokerGame
{
    public class JacksOrBetterVariant : IGameVariant
    {
        public string Name => "Jacks or Better";

        public HandRank EvaluateHand(List<Card> hand)
        {
            return HandEvaluator.EvaluateHand(hand);
        }

        public List<Card> GetWinningCards(List<Card> hand, HandRank rank)
        {
            return HandEvaluator.GetWinningCards(hand, rank);
        }

        public int CalculatePayout(HandRank rank, int bet)
        {
            // Standard 9/6 Jacks or Better Pay Table
            int multiplier = 0;
            switch (rank)
            {
                case HandRank.RoyalFlush:
                    // Standard machine: 250x for 1-4 coins, 800x for 5 coins.
                    if (bet == 5) return 4000; 
                    multiplier = 250;
                    break;
                case HandRank.StraightFlush:
                    multiplier = 50;
                    break;
                case HandRank.FourOfAKind:
                    multiplier = 25;
                    break;
                case HandRank.FullHouse:
                    multiplier = 9;
                    break;
                case HandRank.Flush:
                    multiplier = 6;
                    break;
                case HandRank.Straight:
                    multiplier = 4;
                    break;
                case HandRank.ThreeOfAKind:
                    multiplier = 3;
                    break;
                case HandRank.TwoPair:
                    multiplier = 2;
                    break;
                case HandRank.JacksOrBetter:
                    multiplier = 1;
                    break;
                default:
                    multiplier = 0;
                    break;
            }
            return bet * multiplier;
        }

        public bool IsCardWild(Card card)
        {
            return false;
        }

        public List<PaytableRow> GetPaytable()
        {
            // Standard 9/6 Jacks or Better
            return new List<PaytableRow>
            {
                new PaytableRow("Royal Flush", HandRank.RoyalFlush, new[] { 250, 500, 750, 1000, 4000 }),
                new PaytableRow("Straight Flush", HandRank.StraightFlush, new[] { 50, 100, 150, 200, 250 }),
                new PaytableRow("4 of a Kind", HandRank.FourOfAKind, new[] { 25, 50, 75, 100, 125 }),
                new PaytableRow("Full House", HandRank.FullHouse, new[] { 9, 18, 27, 36, 45 }),
                new PaytableRow("Flush", HandRank.Flush, new[] { 6, 12, 18, 24, 30 }),
                new PaytableRow("Straight", HandRank.Straight, new[] { 4, 8, 12, 16, 20 }),
                new PaytableRow("3 of a Kind", HandRank.ThreeOfAKind, new[] { 3, 6, 9, 12, 15 }),
                new PaytableRow("Two Pair", HandRank.TwoPair, new[] { 2, 4, 6, 8, 10 }),
                new PaytableRow("Jacks or Better", HandRank.JacksOrBetter, new[] { 1, 2, 3, 4, 5 })
            };
        }
    }
}
