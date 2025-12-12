using PokerGame;
using Xunit;

namespace PokerGame.Tests
{
    public class VideoPokerGameTests
    {
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
    }
}
