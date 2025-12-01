using System;

namespace PokerGame
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card : IComparable<Card>
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int CompareTo(Card? other)
        {
            if (other == null) return 1;

            int rankComparison = Rank.CompareTo(other.Rank);
            if (rankComparison != 0)
            {
                return rankComparison;
            }

            return Suit.CompareTo(other.Suit);
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}
