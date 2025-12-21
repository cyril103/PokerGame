using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerGame
{
    public class DoubleDoubleBonusVariant : IGameVariant
    {
        public string Name => "Double Double Bonus";

        public HandRank EvaluateHand(List<Card> hand)
        {
            if (hand == null || hand.Count != 5)
                throw new ArgumentException("Hand must contain exactly 5 cards.");

            // Base evaluation
            HandRank baseRank = HandEvaluator.EvaluateHand(hand);

            if (baseRank == HandRank.FourOfAKind)
            {
                return RefineFourOfAKind(hand);
            }

            return baseRank;
        }

        private HandRank RefineFourOfAKind(List<Card> hand)
        {
            var groups = hand.GroupBy(c => c.Rank).ToList();
            var quadGroup = groups.First(g => g.Count() == 4);
            var kickerGroup = groups.First(g => g.Count() == 1);

            Rank quadRank = quadGroup.Key;
            Rank kickerRank = kickerGroup.Key;

            if (quadRank == Rank.Ace)
            {
                if (kickerRank == Rank.Two || kickerRank == Rank.Three || kickerRank == Rank.Four)
                    return HandRank.FourAcesWithKicker;
                return HandRank.FourAces;
            }
            
            if (quadRank == Rank.Two || quadRank == Rank.Three || quadRank == Rank.Four)
            {
                if (kickerRank == Rank.Ace || kickerRank == Rank.Two || kickerRank == Rank.Three || kickerRank == Rank.Four)
                    return HandRank.FourTwosThreesFoursWithKicker;
                return HandRank.FourTwosThreesFours;
            }

            return HandRank.FourFivesThroughKings;
        }

        public int CalculatePayout(HandRank rank, int bet)
        {
            // Standard 9/6 Double Double Bonus Pay Table
            int multiplier = 0;
            switch (rank)
            {
                case HandRank.RoyalFlush:
                    if (bet == 5) return 4000;
                    multiplier = 250;
                    break;
                case HandRank.StraightFlush:
                    multiplier = 50;
                    break;
                case HandRank.FourAcesWithKicker:
                    multiplier = 400;
                    break;
                case HandRank.FourAces:
                    multiplier = 160;
                    break;
                case HandRank.FourTwosThreesFoursWithKicker:
                    multiplier = 160;
                    break;
                case HandRank.FourTwosThreesFours:
                    multiplier = 80;
                    break;
                case HandRank.FourFivesThroughKings:
                    multiplier = 50;
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
                    multiplier = 1;
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

        public List<Card> GetWinningCards(List<Card> hand, HandRank rank)
        {
            switch (rank)
            {
                case HandRank.FourAcesWithKicker:
                case HandRank.FourTwosThreesFoursWithKicker:
                    // In DDB, the kicker is part of the win for these specific hands
                    return new List<Card>(hand);

                case HandRank.FourAces:
                case HandRank.FourTwosThreesFours:
                case HandRank.FourFivesThroughKings:
                    var quadGroup = hand.GroupBy(c => c.Rank).FirstOrDefault(g => g.Count() == 4);
                    return quadGroup != null ? quadGroup.ToList() : new List<Card>();

                default:
                    return HandEvaluator.GetWinningCards(hand, rank);
            }
        }

        public bool IsCardWild(Card card)
        {
            return false;
        }

        public List<PaytableRow> GetPaytable()
        {
            return new List<PaytableRow>
            {
                new PaytableRow("Royal Flush", HandRank.RoyalFlush, new[] { 250, 500, 750, 1000, 4000 }),
                new PaytableRow("Straight Flush", HandRank.StraightFlush, new[] { 50, 100, 150, 200, 250 }),
                new PaytableRow("4 Aces w/ 2,3,4", HandRank.FourAcesWithKicker, new[] { 400, 800, 1200, 1600, 2000 }),
                new PaytableRow("4 Aces", HandRank.FourAces, new[] { 160, 320, 480, 640, 800 }),
                new PaytableRow("4 2s,3s,4s w/ A-4", HandRank.FourTwosThreesFoursWithKicker, new[] { 160, 320, 480, 640, 800 }),
                new PaytableRow("4 2s,3s,4s", HandRank.FourTwosThreesFours, new[] { 80, 160, 240, 320, 400 }),
                new PaytableRow("4 5s thru Kings", HandRank.FourFivesThroughKings, new[] { 50, 100, 150, 200, 250 }),
                new PaytableRow("Full House", HandRank.FullHouse, new[] { 9, 18, 27, 36, 45 }),
                new PaytableRow("Flush", HandRank.Flush, new[] { 6, 12, 18, 24, 30 }),
                new PaytableRow("Straight", HandRank.Straight, new[] { 4, 8, 12, 16, 20 }),
                new PaytableRow("3 of a Kind", HandRank.ThreeOfAKind, new[] { 3, 6, 9, 12, 15 }),
                new PaytableRow("Two Pair", HandRank.TwoPair, new[] { 1, 2, 3, 4, 5 }),
                new PaytableRow("Jacks or Better", HandRank.JacksOrBetter, new[] { 1, 2, 3, 4, 5 })
            };
        }
    }
}
