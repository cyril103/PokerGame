using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PokerGame
{
    public class Deck
    {
        private List<Card> _cards = new List<Card>();

        public Deck()
        {
            InitializeDeck();
        }

        public void Reset()
        {
            InitializeDeck();
        }

        private void InitializeDeck()
        {
            _cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
        }

        /// <summary>
        /// Shuffles the deck using the Fisher-Yates algorithm and a CSPRNG.
        /// </summary>
        public void Shuffle()
        {
            // Use CSPRNG for a fair Fisher-Yates shuffle
            int n = _cards.Count;
            while (n > 1)
            {
                int k = RandomNumberGenerator.GetInt32(n); // Uniform 0..n-1
                n--;

                Card value = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = value;
            }
        }

        /// <summary>
        /// Deals a specified number of cards from the top of the deck.
        /// </summary>
        public List<Card> DealCards(int count)
        {
            if (count > _cards.Count)
            {
                throw new InvalidOperationException("Not enough cards in the deck.");
            }

            List<Card> dealtCards = _cards.GetRange(0, count);
            _cards.RemoveRange(0, count);
            return dealtCards;
        }

        /// <summary>
        /// Replaces specified cards in a hand with new ones from the deck.
        /// </summary>
        public void ReplaceCards(List<Card> hand, List<Card> cardsToReplace)
        {
            foreach (var cardToReplace in cardsToReplace)
            {
                int index = hand.IndexOf(cardToReplace);
                if (index != -1)
                {
                    hand[index] = DealCards(1)[0];
                }
            }
        }
        
        // Helper for testing to see remaining cards count
        public int RemainingCards => _cards.Count;
    }
}
