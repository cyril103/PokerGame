using System.Collections.Generic;
using Xunit;

namespace PokerGame.Tests
{
    public class DoubleDoubleBonusTests
    {
        private readonly DoubleDoubleBonusVariant _variant;

        public DoubleDoubleBonusTests()
        {
            _variant = new DoubleDoubleBonusVariant();
        }

        [Fact]
        public void FourAcesWithKicker_ShouldReturnFourAcesWithKicker()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Diamonds, Rank.Ace),
                new Card(Suit.Clubs, Rank.Ace),
                new Card(Suit.Spades, Rank.Ace),
                new Card(Suit.Hearts, Rank.Two) // Kicker 2
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourAcesWithKicker, rank);
            Assert.Equal(2000, _variant.CalculatePayout(rank, 5));
        }

        [Fact]
        public void FourAces_ShouldReturnFourAces()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Diamonds, Rank.Ace),
                new Card(Suit.Clubs, Rank.Ace),
                new Card(Suit.Spades, Rank.Ace),
                new Card(Suit.Hearts, Rank.Five) // Kicker 5
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourAces, rank);
            Assert.Equal(800, _variant.CalculatePayout(rank, 5));
        }

        [Fact]
        public void FourTwosWithAceKicker_ShouldReturnFourTwosThreesFoursWithKicker()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Two),
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Spades, Rank.Two),
                new Card(Suit.Hearts, Rank.Ace) // Kicker Ace
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourTwosThreesFoursWithKicker, rank);
            Assert.Equal(800, _variant.CalculatePayout(rank, 5));
        }

        [Fact]
        public void FourTwosWithFiveKicker_ShouldReturnFourTwosThreesFours()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Two),
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Spades, Rank.Two),
                new Card(Suit.Hearts, Rank.Five) // Kicker 5
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourTwosThreesFours, rank);
            Assert.Equal(400, _variant.CalculatePayout(rank, 5));
        }

        [Fact]
        public void FourFives_ShouldReturnFourFivesThroughKings()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Five),
                new Card(Suit.Diamonds, Rank.Five),
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Spades, Rank.Five),
                new Card(Suit.Hearts, Rank.Ace)
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.FourFivesThroughKings, rank);
            Assert.Equal(250, _variant.CalculatePayout(rank, 5));
        }

        [Fact]
        public void TwoPair_ShouldReturnTwoPair()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Jack),
                new Card(Suit.Diamonds, Rank.Jack),
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Spades, Rank.Three),
                new Card(Suit.Hearts, Rank.Ace)
            };

            var rank = _variant.EvaluateHand(hand);
            Assert.Equal(HandRank.TwoPair, rank);
            Assert.Equal(5, _variant.CalculatePayout(rank, 5)); // 1x payout
        }
    }
}
