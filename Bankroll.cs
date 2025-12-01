using System;

namespace PokerGame
{
    public class Bankroll
    {
        public int Credits { get; private set; }

        public Bankroll(int initialCredits)
        {
            if (initialCredits < 0)
                throw new ArgumentException("Initial credits cannot be negative.");
            Credits = initialCredits;
        }

        public bool CanBet(int amount)
        {
            return Credits >= amount && amount > 0;
        }

        public void Bet(int amount)
        {
            if (!CanBet(amount))
                throw new InvalidOperationException("Insufficient credits or invalid bet amount.");
            Credits -= amount;
        }

        public void AddWin(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Win amount cannot be negative.");
            Credits += amount;
        }

        public void Deposit(int amount)
        {
             if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");
            Credits += amount;
        }
    }
}
