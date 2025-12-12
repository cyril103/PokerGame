using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerGame
{
    public class DeucesWildVariant : IGameVariant
    {
        public string Name => "Deuces Wild";

        public HandRank EvaluateHand(List<Card> hand)
        {
            if (hand == null || hand.Count != 5)
                throw new ArgumentException("Hand must contain exactly 5 cards.");

            var deuces = hand.Count(c => c.Rank == Rank.Two);
            var nonDeuces = hand.Where(c => c.Rank != Rank.Two).OrderBy(c => c.Rank).ToList();

            if (deuces == 4) return HandRank.FourDeuces;

            // Natural Royal Flush (0 deuces) is handled by standard logic, but we need to distinguish Wild Royal
            if (deuces == 0)
            {
                var standardRank = HandEvaluator.EvaluateHand(hand);
                // Deuces Wild usually doesn't pay for Two Pair or High Card, and JacksOrBetter is not a thing.
                // But we can map standard ranks.
                // Note: Standard Evaluator returns JacksOrBetter. We need to filter that out or ignore it?
                // In Deuces Wild, lowest pay is usually 3 of a Kind.
                if (standardRank == HandRank.JacksOrBetter || standardRank == HandRank.TwoPair || standardRank == HandRank.HighCard)
                    return HandRank.HighCard; // Losing hand in Deuces Wild
                return standardRank;
            }

            // With Deuces:
            
            // 1. Check for Five of a Kind
            // Possible if nonDeuces are all same rank
            if (nonDeuces.Count > 0 && nonDeuces.All(c => c.Rank == nonDeuces[0].Rank))
            {
                return HandRank.FiveOfAKind;
            }

            // 2. Check for Royal Flush (Wild)
            // Needs to be same suit (wilds assume suit) and ranks must fit in 10,J,Q,K,A
            // Group non-deuces by suit. If more than 1 suit present, impossible (unless we have 5 deuces? impossible with 1 deck).
            // Actually, if we have 1 deuce, we have 4 non-deuces. They must be same suit.
            if (IsWildRoyalFlush(nonDeuces))
            {
                return HandRank.WildRoyalFlush;
            }

            // 3. Check for Straight Flush
            if (IsWildStraightFlush(nonDeuces))
            {
                return HandRank.StraightFlush;
            }

            // 4. Four of a Kind
            // If we have 3 deuces, we have 2 non-deuces. If they are pair -> 5 of a kind (handled). If not pair -> 4 of a kind.
            // If 2 deuces, 3 non-deuces. If pair -> 4 of a kind. If 3 of a kind -> 5 of a kind (handled).
            // If 1 deuce, 4 non-deuces. If 3 of a kind -> 4 of a kind.
            if (IsWildFourOfAKind(nonDeuces, deuces))
            {
                return HandRank.FourOfAKind;
            }

            // 5. Full House
            // 1 Deuce: 2 pair -> Full House.
            // 0 Deuces: Standard check (handled above).
            // With wilds, Full House is less common than 4 of a kind usually?
            // Wait, with 1 deuce and 2 pair (A A K K 2), you make Full House (A A A K K).
            if (IsWildFullHouse(nonDeuces, deuces))
            {
                return HandRank.FullHouse;
            }

            // 6. Flush
            // All non-deuces same suit.
            if (IsWildFlush(nonDeuces))
            {
                return HandRank.Flush;
            }

            // 7. Straight
            if (IsWildStraight(nonDeuces))
            {
                return HandRank.Straight;
            }

            // 8. Three of a Kind
            if (IsWildThreeOfAKind(nonDeuces, deuces))
            {
                return HandRank.ThreeOfAKind;
            }

            return HandRank.HighCard;
        }

        private bool IsWildRoyalFlush(List<Card> nonDeuces)
        {
            // All non-deuces must be same suit
            if (nonDeuces.Select(c => c.Suit).Distinct().Count() > 1) return false;

            // All non-deuces must be >= 10 (Ten, Jack, Queen, King, Ace)
            if (nonDeuces.Any(c => c.Rank < Rank.Ten)) return false;

            // Must be distinct ranks (no pairs)
            if (nonDeuces.Select(c => c.Rank).Distinct().Count() != nonDeuces.Count) return false;

            return true;
        }

        private bool IsWildStraightFlush(List<Card> nonDeuces)
        {
            // Must be same suit
            if (nonDeuces.Select(c => c.Suit).Distinct().Count() > 1) return false;

            // Must be able to form a straight
            return IsWildStraight(nonDeuces);
        }

        private bool IsWildStraight(List<Card> nonDeuces)
        {
            // Distinct ranks only. If pair, cannot be straight.
            if (nonDeuces.Select(c => c.Rank).Distinct().Count() != nonDeuces.Count) return false;

            // Get ranks as integers. Ace can be 14 or 1.
            // We need to check if there is a window of length 5 that covers all non-deuces.
            // And the number of gaps <= number of deuces.
            
            var ranks = nonDeuces.Select(c => (int)c.Rank).OrderBy(r => r).ToList();
            
            // Try with Ace as High (14)
            if (CheckStraightWindow(ranks, 5 - nonDeuces.Count)) return true;

            // Try with Ace as Low (1) if Ace is present
            if (ranks.Contains(14))
            {
                var lowAceRanks = ranks.Select(r => r == 14 ? 1 : r).OrderBy(r => r).ToList();
                if (CheckStraightWindow(lowAceRanks, 5 - nonDeuces.Count)) return true;
            }

            return false;
        }

        private bool CheckStraightWindow(List<int> ranks, int numDeuces)
        {
            // Max - Min must be < 5
            // And duplicates already checked (no duplicates allowed for straight)
            if (ranks.Max() - ranks.Min() >= 5) return false;
            return true;
        }

        private bool IsWildFourOfAKind(List<Card> nonDeuces, int deuces)
        {
            // Max frequency + deuces >= 4
            var maxFreq = nonDeuces.GroupBy(c => c.Rank).Max(g => g.Count());
            return maxFreq + deuces >= 4;
        }

        private bool IsWildFullHouse(List<Card> nonDeuces, int deuces)
        {
            // Typically with 1 deuce: Two Pair -> Full House.
            // With 2 deuces: Any card -> 3 of a kind + 2 deuces = 5 of a kind (better).
            // So Full House is only relevant if we don't have 4/5 of a kind.
            // 1 Deuce, 2 Pair (A A K K 2) -> Full House.
            if (deuces == 1)
            {
                var groups = nonDeuces.GroupBy(c => c.Rank).ToList();
                // If 2 groups of 2 (Two Pair)
                if (groups.Count == 2 && groups.All(g => g.Count() == 2)) return true;
            }
            return false;
        }

        private bool IsWildFlush(List<Card> nonDeuces)
        {
            return nonDeuces.Select(c => c.Suit).Distinct().Count() == 1;
        }

        private bool IsWildThreeOfAKind(List<Card> nonDeuces, int deuces)
        {
            var maxFreq = nonDeuces.GroupBy(c => c.Rank).Max(g => g.Count());
            return maxFreq + deuces >= 3;
        }

        public int CalculatePayout(HandRank rank, int bet)
        {
            // "Not So Ugly Ducks" (NSUD) Pay Table - More generous than standard
            // Natural Royal: 4000 (max bet) / 250
            // Four Deuces: 200
            // Wild Royal: 25
            // 5 of a Kind: 16
            // Straight Flush: 10
            // 4 of a Kind: 4
            // Full House: 4
            // Flush: 3
            // Straight: 2
            // 3 of a Kind: 1
            
            int multiplier = 0;
            switch (rank)
            {
                case HandRank.RoyalFlush: // Natural
                    if (bet == 5) return 4000;
                    multiplier = 250;
                    break;
                case HandRank.FourDeuces:
                    multiplier = 200;
                    break;
                case HandRank.WildRoyalFlush:
                    multiplier = 25;
                    break;
                case HandRank.FiveOfAKind:
                    multiplier = 16;
                    break;
                case HandRank.StraightFlush:
                    multiplier = 10;
                    break;
                case HandRank.FourOfAKind:
                    multiplier = 4;
                    break;
                case HandRank.FullHouse:
                    multiplier = 4;
                    break;
                case HandRank.Flush:
                    multiplier = 3;
                    break;
                case HandRank.Straight:
                    multiplier = 2;
                    break;
                case HandRank.ThreeOfAKind:
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
            if (rank == HandRank.HighCard) return new List<Card>();

            // For Straight, Flush, Full House, Straight Flush, Royal Flush, Five of a Kind
            // All 5 cards are involved.
            if (rank == HandRank.Straight || 
                rank == HandRank.Flush || 
                rank == HandRank.FullHouse || 
                rank == HandRank.StraightFlush || 
                rank == HandRank.WildRoyalFlush || 
                rank == HandRank.RoyalFlush || 
                rank == HandRank.FiveOfAKind ||
                rank == HandRank.FourDeuces) // 4 Deuces + Kicker is usually considered 5 cards winning or just 4? 
                                             // Usually the kicker doesn't matter, but highlighting 4 is cleaner.
            {
                if (rank == HandRank.FourDeuces)
                {
                    return hand.Where(c => c.Rank == Rank.Two).ToList();
                }
                return hand;
            }

            var deuces = hand.Where(c => c.Rank == Rank.Two).ToList();
            var nonDeuces = hand.Where(c => c.Rank != Rank.Two).ToList();

            if (rank == HandRank.FourOfAKind)
            {
                // 4 of a Kind: Deuces + matching non-deuces.
                // We need to find which rank forms the quad.
                // It's the rank with the most count among non-deuces.
                var group = nonDeuces.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).FirstOrDefault();
                if (group != null)
                {
                    var winning = new List<Card>(deuces);
                    winning.AddRange(group);
                    return winning;
                }
            }

            if (rank == HandRank.ThreeOfAKind)
            {
                // 3 of a Kind: Deuces + matching non-deuces.
                var group = nonDeuces.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).FirstOrDefault();
                if (group != null)
                {
                    var winning = new List<Card>(deuces);
                    // We only need enough cards to make 3.
                    // If we have 2 deuces, we need 1 from group.
                    // If we have 1 deuce, we need 2 from group.
                    // If we have 0 deuces, we need 3 from group.
                    
                    int needed = 3 - deuces.Count;
                    winning.AddRange(group.Take(needed));
                    return winning;
                }
            }

            return hand; // Fallback
        }

        public bool IsCardWild(Card card)
        {
            return card.Rank == Rank.Two;
        }

        public List<PaytableRow> GetPaytable()
        {
            // NSUD Pay Table
            return new List<PaytableRow>
            {
                new PaytableRow("Natural Royal Flush", HandRank.RoyalFlush, new[] { 250, 500, 750, 1000, 4000 }),
                new PaytableRow("Four Deuces", HandRank.FourDeuces, new[] { 200, 400, 600, 800, 1000 }),
                new PaytableRow("Wild Royal Flush", HandRank.WildRoyalFlush, new[] { 25, 50, 75, 100, 125 }),
                new PaytableRow("Five of a Kind", HandRank.FiveOfAKind, new[] { 16, 32, 48, 64, 80 }),
                new PaytableRow("Straight Flush", HandRank.StraightFlush, new[] { 10, 20, 30, 40, 50 }),
                new PaytableRow("Four of a Kind", HandRank.FourOfAKind, new[] { 4, 8, 12, 16, 20 }),
                new PaytableRow("Full House", HandRank.FullHouse, new[] { 4, 8, 12, 16, 20 }),
                new PaytableRow("Flush", HandRank.Flush, new[] { 3, 6, 9, 12, 15 }),
                new PaytableRow("Straight", HandRank.Straight, new[] { 2, 4, 6, 8, 10 }),
                new PaytableRow("Three of a Kind", HandRank.ThreeOfAKind, new[] { 1, 2, 3, 4, 5 })
            };
        }
    }
}
