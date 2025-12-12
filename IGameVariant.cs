using System.Collections.Generic;

namespace PokerGame
{
    public interface IGameVariant
    {
        string Name { get; }
        HandRank EvaluateHand(List<Card> hand);
        int CalculatePayout(HandRank rank, int bet);
        List<Card> GetWinningCards(List<Card> hand, HandRank rank);
    }
}
