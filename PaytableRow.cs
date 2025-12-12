using System;

namespace PokerGame
{
    public class PaytableRow
    {
        public string DisplayName { get; set; }
        public HandRank Rank { get; set; }
        public int[] Payouts { get; set; } = new int[5]; // Payouts for 1-5 coins

        public PaytableRow(string name, HandRank rank, int[] payouts)
        {
            DisplayName = name;
            Rank = rank;
            Payouts = payouts;
        }
    }
}
