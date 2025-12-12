using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerGame
{
    public enum HandRank
    {
        HighCard,
        JacksOrBetter,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush,
        // Deuces Wild specific
        FiveOfAKind,
        WildRoyalFlush,
        FourDeuces
    }

    public class HandEvaluator
    {
        public static HandRank EvaluateHand(List<Card> hand)
        {
            if (hand == null || hand.Count != 5)
                throw new ArgumentException("Hand must contain exactly 5 cards.");

            // Sort hand by rank to make evaluation easier
            var sortedHand = hand.OrderBy(c => c.Rank).ToList();

            bool isFlush = IsFlush(sortedHand);
            bool isStraight = IsStraight(sortedHand);

            if (isFlush && isStraight)
            {
                // Royal Flush must be 10, J, Q, K, A
                // If it is A, 2, 3, 4, 5, it is a Straight Flush, not Royal
                if (sortedHand[0].Rank == Rank.Ten && sortedHand.Last().Rank == Rank.Ace)
                    return HandRank.RoyalFlush;
                return HandRank.StraightFlush;
            }

            if (IsFourOfAKind(sortedHand))
                return HandRank.FourOfAKind;

            if (IsFullHouse(sortedHand))
                return HandRank.FullHouse;

            if (isFlush)
                return HandRank.Flush;

            if (isStraight)
                return HandRank.Straight;

            if (IsThreeOfAKind(sortedHand))
                return HandRank.ThreeOfAKind;

            if (IsTwoPair(sortedHand))
                return HandRank.TwoPair;

            if (IsJacksOrBetter(sortedHand))
                return HandRank.JacksOrBetter;

            return HandRank.HighCard;
        }

        public static List<Card> GetWinningCards(List<Card> hand, HandRank rank)
        {
            if (hand == null || hand.Count != 5) return new List<Card>();
            
            // We need to work with the original card objects to return the correct instances
            // But logic often relies on sorted values. 
            // We'll find the ranks/suits that matter, then filter the original list.
            
            var sortedHand = hand.OrderBy(c => c.Rank).ToList();

            switch (rank)
            {
                case HandRank.RoyalFlush:
                case HandRank.StraightFlush:
                case HandRank.Flush:
                case HandRank.Straight:
                    // All 5 cards are part of these hands
                    return new List<Card>(hand);

                case HandRank.FourOfAKind:
                    var fourGroup = hand.GroupBy(c => c.Rank).FirstOrDefault(g => g.Count() == 4);
                    return fourGroup != null ? fourGroup.ToList() : new List<Card>();

                case HandRank.FullHouse:
                    // All 5 cards (3 + 2)
                    return new List<Card>(hand);

                case HandRank.ThreeOfAKind:
                    var threeGroup = hand.GroupBy(c => c.Rank).FirstOrDefault(g => g.Count() == 3);
                    return threeGroup != null ? threeGroup.ToList() : new List<Card>();

                case HandRank.TwoPair:
                    var twoPairGroups = hand.GroupBy(c => c.Rank).Where(g => g.Count() == 2).SelectMany(g => g).ToList();
                    return twoPairGroups;

                case HandRank.JacksOrBetter:
                    var jacksGroup = hand.GroupBy(c => c.Rank).FirstOrDefault(g => g.Count() == 2 && g.Key >= Rank.Jack);
                    return jacksGroup != null ? jacksGroup.ToList() : new List<Card>();

                default:
                    return new List<Card>();
            }
        }

        private static bool IsFlush(List<Card> hand)
        {
            return hand.All(c => c.Suit == hand[0].Suit);
        }

        private static bool IsStraight(List<Card> hand)
        {
            // Check for standard straight
            bool isStandardStraight = true;
            for (int i = 0; i < 4; i++)
            {
                if (hand[i + 1].Rank != hand[i].Rank + 1)
                {
                    isStandardStraight = false;
                    break;
                }
            }
            if (isStandardStraight) return true;

            // Check for Ace-low straight (A, 2, 3, 4, 5)
            // In our Rank enum, Ace is high (14). 
            // If we have 2, 3, 4, 5, Ace, the sorted order is 2, 3, 4, 5, Ace.
            if (hand[0].Rank == Rank.Two &&
                hand[1].Rank == Rank.Three &&
                hand[2].Rank == Rank.Four &&
                hand[3].Rank == Rank.Five &&
                hand[4].Rank == Rank.Ace)
            {
                return true;
            }

            return false;
        }

        private static bool IsFourOfAKind(List<Card> hand)
        {
            var groups = hand.GroupBy(c => c.Rank);
            return groups.Any(g => g.Count() == 4);
        }

        private static bool IsFullHouse(List<Card> hand)
        {
            var groups = hand.GroupBy(c => c.Rank).ToList();
            return groups.Count == 2 && groups.Any(g => g.Count() == 3);
        }

        private static bool IsThreeOfAKind(List<Card> hand)
        {
            var groups = hand.GroupBy(c => c.Rank);
            return groups.Any(g => g.Count() == 3);
        }

        private static bool IsTwoPair(List<Card> hand)
        {
            var groups = hand.GroupBy(c => c.Rank);
            return groups.Count(g => g.Count() == 2) == 2;
        }

        private static bool IsJacksOrBetter(List<Card> hand)
        {
            var groups = hand.GroupBy(c => c.Rank);
            // Check for any pair where the rank is Jack or higher
            return groups.Any(g => g.Count() == 2 && g.Key >= Rank.Jack);
        }
    }
}
