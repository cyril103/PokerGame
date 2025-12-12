using System.Collections.Generic;
using System.Reflection;
using PokerGame;
using Xunit;

namespace PokerGame.Tests
{
    public class VideoPokerGameTests
    {
        private static void SetupDoubleUpState(VideoPokerGame game, List<Card> hand, int lastWin)
        {
            // Simulate a completed winning hand so bankroll already includes the last win
            var bankrollField = typeof(VideoPokerGame).GetField("_bankroll", BindingFlags.NonPublic | BindingFlags.Instance);
            var bankroll = (Bankroll)bankrollField.GetValue(game);
            bankroll.AddWin(lastWin);

            var currentHandProp = typeof(VideoPokerGame).GetProperty(nameof(VideoPokerGame.CurrentHand));
            currentHandProp!.GetSetMethod(true)!.Invoke(game, new object[] { hand });

            var currentStateProp = typeof(VideoPokerGame).GetProperty(nameof(VideoPokerGame.CurrentState));
            currentStateProp!.GetSetMethod(true)!.Invoke(game, new object[] { GameState.DoubleUp });

            var lastWinProp = typeof(VideoPokerGame).GetProperty(nameof(VideoPokerGame.LastWin));
            lastWinProp!.GetSetMethod(true)!.Invoke(game, new object[] { lastWin });
        }

        [Fact]
        public void Reset_TopsUpBankrollWhenEmpty()
        {
            var game = new VideoPokerGame(new JacksOrBetterVariant(), 0);
            game.Reset();

            Assert.Equal(100, game.Credits);
        }

        [Fact]
        public void Reset_DoesNotChangePositiveBankroll()
        {
            var game = new VideoPokerGame(new JacksOrBetterVariant(), 50);
            game.Reset();

            Assert.Equal(50, game.Credits);
        }

        [Fact]
        public void JacksOrBetter_CalculatePayout_UsesExpectedMultipliers()
        {
            var variant = new JacksOrBetterVariant();
            
            Assert.Equal(4000, variant.CalculatePayout(HandRank.RoyalFlush, 5)); // Max bet bonus
            Assert.Equal(250, variant.CalculatePayout(HandRank.RoyalFlush, 1));
            Assert.Equal(150, variant.CalculatePayout(HandRank.StraightFlush, 3));
            Assert.Equal(50, variant.CalculatePayout(HandRank.JacksOrBetter, 50)); // 1:1 on pair J+
            Assert.Equal(0, variant.CalculatePayout(HandRank.HighCard, 5));
        }

        [Fact]
        public void DoubleUp_TieDoesNotChangeCreditsOrWin()
        {
            var game = new VideoPokerGame(new JacksOrBetterVariant(), 100);
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Seven),    // Dealer
                new Card(Suit.Spades, Rank.Seven),   // Player pick (tie by rank, different suit)
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Three),
                new Card(Suit.Hearts, Rank.Four)
            };

            SetupDoubleUpState(game, hand, 10);
            var initialCredits = game.Credits;

            bool result = game.PlayDoubleUp(1);

            Assert.True(result); // Treated as push
            Assert.Equal(GameState.GameOver, game.CurrentState);
            Assert.Equal(10, game.LastWin);
            Assert.Equal(initialCredits, game.Credits);
        }

        [Fact]
        public void DoubleUp_Win_DoublesLastWinAndAddsToCredits()
        {
            var game = new VideoPokerGame(new JacksOrBetterVariant(), 100);
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Six),      // Dealer
                new Card(Suit.Spades, Rank.Eight),   // Player pick (higher rank)
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Three),
                new Card(Suit.Hearts, Rank.Four)
            };

            SetupDoubleUpState(game, hand, 10);

            bool result = game.PlayDoubleUp(1);

            Assert.True(result);
            Assert.Equal(20, game.LastWin);
            Assert.Equal(120, game.Credits); // 100 + 10 (initial win) + 10 (double)
            Assert.Equal(GameState.GameOver, game.CurrentState);
        }

        [Fact]
        public void DoubleUp_Loss_RemovesLastWinFromCredits()
        {
            var game = new VideoPokerGame(new JacksOrBetterVariant(), 100);
            var hand = new List<Card>
            {
                new Card(Suit.Clubs, Rank.King),     // Dealer
                new Card(Suit.Spades, Rank.Jack),    // Player pick (lower rank)
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Diamonds, Rank.Three),
                new Card(Suit.Hearts, Rank.Four)
            };

            SetupDoubleUpState(game, hand, 10);

            bool result = game.PlayDoubleUp(1);

            Assert.False(result);
            Assert.Equal(0, game.LastWin);
            Assert.Equal(100, game.Credits); // 100 + 10 (initial) - 10 (lost)
            Assert.Equal(GameState.GameOver, game.CurrentState);
        }

        [Fact]
        public void DoubleUp_CannotRestartWhileInDoubleUp()
        {
            var game = new VideoPokerGame(new JacksOrBetterVariant(), 100);

            // Seed a winning state and enter Double Up once
            typeof(VideoPokerGame).GetProperty(nameof(VideoPokerGame.LastWin))!
                .GetSetMethod(true)!.Invoke(game, new object[] { 10 });
            typeof(VideoPokerGame).GetProperty(nameof(VideoPokerGame.CurrentState))!
                .GetSetMethod(true)!.Invoke(game, new object[] { GameState.GameOver });

            game.StartDoubleUp(); // first time OK

            Assert.Throws<InvalidOperationException>(() => game.StartDoubleUp()); // should not be allowed during round
        }
    }
}
