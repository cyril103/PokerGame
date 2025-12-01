using System.Reflection;
using PokerGame;
using Xunit;

namespace PokerGame.Tests
{
    public class VideoPokerGameTests
    {
        [Fact]
        public void Reset_TopsUpBankrollWhenEmpty()
        {
            var game = new VideoPokerGame(0);
            game.Reset();

            Assert.Equal(100, game.Credits);
        }

        [Fact]
        public void Reset_DoesNotChangePositiveBankroll()
        {
            var game = new VideoPokerGame(50);
            game.Reset();

            Assert.Equal(50, game.Credits);
        }

        [Fact]
        public void CalculatePayout_UsesExpectedMultipliers()
        {
            Assert.Equal(4000, InvokePayout(HandRank.RoyalFlush, 5)); // Max bet bonus
            Assert.Equal(250, InvokePayout(HandRank.RoyalFlush, 1));
            Assert.Equal(150, InvokePayout(HandRank.StraightFlush, 3));
            Assert.Equal(50, InvokePayout(HandRank.JacksOrBetter, 50)); // 1:1 on pair J+
            Assert.Equal(0, InvokePayout(HandRank.HighCard, 5));
        }

        private static int InvokePayout(HandRank rank, int bet)
        {
            var game = new VideoPokerGame();
            var method = typeof(VideoPokerGame).GetMethod("CalculatePayout", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(method);

            var result = method!.Invoke(game, new object[] { rank, bet });
            return (int)result!;
        }
    }
}
