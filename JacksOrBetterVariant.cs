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
    }
}
