using System.Collections.Generic;
using PokerGame;
using Xunit;

namespace PokerGame.Tests
{
    public class HandEvaluatorTests
    {
        [Fact]
        public void EvaluateHand_RoyalFlush_ReturnsRoyalFlush()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Jack),
                new Card(Suit.Spades, Rank.Queen),
                new Card(Suit.Spades, Rank.King),
                new Card(Suit.Spades, Rank.Ace)
            };

            var result = HandEvaluator.EvaluateHand(hand);
            Assert.Equal(HandRank.RoyalFlush, result);
        }

        [Fact]
        public void EvaluateHand_AceLowStraight_ReturnsStraight()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Diamonds, Rank.Three),
                new Card(Suit.Hearts, Rank.Four),
                new Card(Suit.Spades, Rank.Five),
                new Card(Suit.Clubs, Rank.Ace)
            };

            var result = HandEvaluator.EvaluateHand(hand);
            Assert.Equal(HandRank.Straight, result);
        }

        [Fact]
        public void EvaluateHand_JacksOrBetter_ReturnsJacksOrBetter()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Jack),
                new Card(Suit.Diamonds, Rank.Jack),
                new Card(Suit.Hearts, Rank.Three),
                new Card(Suit.Spades, Rank.Four),
                new Card(Suit.Clubs, Rank.Nine)
            };

            var result = HandEvaluator.EvaluateHand(hand);
            Assert.Equal(HandRank.JacksOrBetter, result);
        }

        [Fact]
        public void EvaluateHand_LowPair_ReturnsHighCard()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Diamonds, Rank.Ten),
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Spades, Rank.Five),
                new Card(Suit.Clubs, Rank.Seven)
            };

            var result = HandEvaluator.EvaluateHand(hand);
            Assert.Equal(HandRank.HighCard, result);
        }

        [Fact]
        public void GetWinningCards_FourOfAKind_ReturnsFourCards()
        {
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Diamonds, Rank.Five),
                new Card(Suit.Hearts, Rank.Five),
                new Card(Suit.Spades, Rank.Five),
                new Card(Suit.Clubs, Rank.Two)
            };

            var rank = HandEvaluator.EvaluateHand(hand);
            var winners = HandEvaluator.GetWinningCards(hand, rank);

            Assert.Equal(HandRank.FourOfAKind, rank);
            Assert.Equal(4, winners.Count);
        }
    }
}
