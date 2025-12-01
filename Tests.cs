using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerGame
{
    public class Tests
    {
        public static void RunTests()
        {
            Console.WriteLine("Running Tests...");
            TestShuffle();
            TestHandEvaluation();
            Console.WriteLine("All Tests Completed.");
        }

        private static void TestShuffle()
        {
            Console.WriteLine("Testing Shuffle...");
            Deck deck = new Deck();
            var originalOrder = deck.DealCards(52); // Get all cards to see order
            
            // Re-create deck and shuffle
            Deck deck2 = new Deck();
            deck2.Shuffle();
            var shuffledOrder = deck2.DealCards(52);

            bool isDifferent = false;
            for (int i = 0; i < 52; i++)
            {
                if (originalOrder[i].ToString() != shuffledOrder[i].ToString())
                {
                    isDifferent = true;
                    break;
                }
            }

            if (isDifferent)
                Console.WriteLine("PASS: Deck order changed after shuffle.");
            else
                Console.WriteLine("FAIL: Deck order did not change (extremely unlikely but possible).");
            
            // Verify all cards are present
            if (shuffledOrder.Count == 52)
                 Console.WriteLine("PASS: 52 Cards present.");
            else
                 Console.WriteLine($"FAIL: Count is {shuffledOrder.Count}");
        }

        private static void TestHandEvaluation()
        {
            Console.WriteLine("Testing Hand Evaluation...");

            // Royal Flush
            var royalFlush = new List<Card>
            {
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Jack),
                new Card(Suit.Spades, Rank.Queen),
                new Card(Suit.Spades, Rank.King),
                new Card(Suit.Spades, Rank.Ace)
            };
            AssertHand(royalFlush, HandRank.RoyalFlush, "Royal Flush");

            // Straight Flush
            var straightFlush = new List<Card>
            {
                new Card(Suit.Hearts, Rank.Nine),
                new Card(Suit.Hearts, Rank.Ten),
                new Card(Suit.Hearts, Rank.Jack),
                new Card(Suit.Hearts, Rank.Queen),
                new Card(Suit.Hearts, Rank.King)
            };
            AssertHand(straightFlush, HandRank.StraightFlush, "Straight Flush");

            // Four of a Kind
            var fourOfAKind = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Diamonds, Rank.Five),
                new Card(Suit.Hearts, Rank.Five),
                new Card(Suit.Spades, Rank.Five),
                new Card(Suit.Clubs, Rank.Two)
            };
            AssertHand(fourOfAKind, HandRank.FourOfAKind, "Four of a Kind");

            // Full House
            var fullHouse = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Diamonds, Rank.Three),
                new Card(Suit.Hearts, Rank.Three),
                new Card(Suit.Spades, Rank.King),
                new Card(Suit.Clubs, Rank.King)
            };
            AssertHand(fullHouse, HandRank.FullHouse, "Full House");

            // Flush
            var flush = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Clubs, Rank.Seven),
                new Card(Suit.Clubs, Rank.Jack),
                new Card(Suit.Clubs, Rank.King)
            };
            AssertHand(flush, HandRank.Flush, "Flush");

            // Straight
            var straight = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Diamonds, Rank.Four),
                new Card(Suit.Hearts, Rank.Five),
                new Card(Suit.Spades, Rank.Six),
                new Card(Suit.Clubs, Rank.Seven)
            };
            AssertHand(straight, HandRank.Straight, "Straight");

            // Three of a Kind
            var threeOfAKind = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Eight),
                new Card(Suit.Diamonds, Rank.Eight),
                new Card(Suit.Hearts, Rank.Eight),
                new Card(Suit.Spades, Rank.Nine),
                new Card(Suit.Clubs, Rank.Ten)
            };
            AssertHand(threeOfAKind, HandRank.ThreeOfAKind, "Three of a Kind");

            // Two Pair
            var twoPair = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Jack),
                new Card(Suit.Diamonds, Rank.Jack),
                new Card(Suit.Hearts, Rank.Three),
                new Card(Suit.Spades, Rank.Three),
                new Card(Suit.Clubs, Rank.Ace)
            };
            AssertHand(twoPair, HandRank.TwoPair, "Two Pair");

            // Jacks or Better
            var jacksOrBetter = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Queen),
                new Card(Suit.Diamonds, Rank.Queen),
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Spades, Rank.Three),
                new Card(Suit.Clubs, Rank.Four)
            };
            AssertHand(jacksOrBetter, HandRank.JacksOrBetter, "Jacks or Better (Queens)");

            // Low Pair (Not Jacks or Better) -> High Card
            var lowPair = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Diamonds, Rank.Ten),
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Spades, Rank.Three),
                new Card(Suit.Clubs, Rank.Four)
            };
            AssertHand(lowPair, HandRank.HighCard, "Low Pair (Tens) -> High Card");

            // High Card
            var highCard = new List<Card>
            {
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Diamonds, Rank.Four),
                new Card(Suit.Hearts, Rank.Six),
                new Card(Suit.Spades, Rank.Eight),
                new Card(Suit.Clubs, Rank.Ten)
            };
            AssertHand(highCard, HandRank.HighCard, "High Card");
        }

        private static void AssertHand(List<Card> hand, HandRank expected, string testName)
        {
            var result = HandEvaluator.EvaluateHand(hand);
            if (result == expected)
            {
                Console.WriteLine($"PASS: {testName}");
            }
            else
            {
                Console.WriteLine($"FAIL: {testName} - Expected {expected}, Got {result}");
            }
        }
    }
}
