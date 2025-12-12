using System.Collections.Generic;
using Xunit;

namespace PokerGame.Tests
{
    public class DeucesWildTests
    {
        private readonly DeucesWildVariant _variant;

        public DeucesWildTests()
        {
            _variant = new DeucesWildVariant();
        }

        [Fact]
        public void FourDeuces_ShouldReturnFourDeuces()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Two),
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Spades, Rank.Two),
                new Card(Suit.Hearts, Rank.Ace)
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourDeuces, rank);
        }

        [Fact]
        public void WildRoyalFlush_ShouldReturnWildRoyalFlush()
        {
            // 2, 10, J, Q, K (Hearts) -> Wild Royal
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Hearts, Rank.Ten),
                new Card(Suit.Hearts, Rank.Jack),
                new Card(Suit.Hearts, Rank.Queen),
                new Card(Suit.Hearts, Rank.King)
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.WildRoyalFlush, rank);
        }

        [Fact]
        public void FiveOfAKind_ShouldReturnFiveOfAKind()
        {
            // 2, 2, 2, A, A -> 5 Aces
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Two),
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Spades, Rank.Ace),
                new Card(Suit.Hearts, Rank.Ace)
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FiveOfAKind, rank);
        }

        [Fact]
        public void ThreeDeuces_ShouldMakeFourOfAKind_IfNoPair()
        {
            // 2, 2, 2, 5, 6 -> 4 of a kind (e.g. 6 6 6 6 5)
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Two),
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Spades, Rank.Five),
                new Card(Suit.Hearts, Rank.Six)
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourOfAKind, rank); // Or better if straight flush possible? 2 2 2 5 6 suited? 
            // If suited (wilds match suit), 2 3 4 5 6 -> Straight Flush.
            // But here 5 and 6 might be different suits.
            // Wait, wilds assume ANY suit.
            // If 5 and 6 are same suit, then Straight Flush is possible (2,3,4,5,6).
            // Let's ensure 5 and 6 are different suits for this test to force 4 of a kind.
        }

        [Fact]
        public void StraightFlush_WithWilds()
        {
            // 2, 3, 4, 5, 6 (Hearts) -> Straight Flush
            // Hand: 2H, 3H, 4H, 5H, 9D (Wait, 9D breaks flush).
            // Hand: 2H, 3H, 4H, 5H, 6H -> Natural Straight Flush (or Wild if 2 is used as 2).
            // Hand: 2H, 3H, 4H, 5H, 8H -> 2 fills 6H? 3,4,5,6,8? No.
            // Hand: 2H, 3H, 4H, 5H, 7H -> 2 as 6H -> 3,4,5,6,7.
            
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Hearts, Rank.Three),
                new Card(Suit.Hearts, Rank.Four),
                new Card(Suit.Hearts, Rank.Five),
                new Card(Suit.Hearts, Rank.Seven) // Gap filled by 2? No 2 is present.
                // Wait, 2 is the wild card.
                // 2H, 3H, 4H, 5H, 7H.
                // Non-deuces: 3H, 4H, 5H, 7H.
                // Gaps: 3,4,5,_,7. Gap is 6.
                // 1 Deuce available. Can fill 6.
                // So yes, Straight Flush.
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.StraightFlush, rank);
        }
    }
}
