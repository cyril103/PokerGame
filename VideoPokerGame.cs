using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerGame
{
    public enum GameState
    {
        WaitingForBet,
        Dealt,
        DoubleUp,
        GameOver
    }

    public class VideoPokerGame
    {
        private readonly Deck _deck;
        private readonly Bankroll _bankroll;
        
        public List<Card> CurrentHand { get; private set; }
        public List<bool> HeldCards { get; private set; }
        public GameState CurrentState { get; private set; }
        public int CurrentBet { get; private set; }
        public int LastWin { get; private set; }
        public HandRank LastHandRank { get; private set; }

        public int Credits => _bankroll.Credits;

        public VideoPokerGame(int initialCredits = 100)
        {
            _bankroll = new Bankroll(initialCredits);
            _deck = new Deck();
            CurrentState = GameState.WaitingForBet;
            CurrentHand = new List<Card>();
            HeldCards = new List<bool>();
        }

        public void PlaceBet(int amount)
        {
            if (CurrentState != GameState.WaitingForBet && CurrentState != GameState.GameOver)
                throw new InvalidOperationException("Cannot place bet now.");

            if (!_bankroll.CanBet(amount))
                throw new InvalidOperationException("Insufficient credits.");

            _bankroll.Bet(amount);
            CurrentBet = amount;
            DealInitialHand();
        }

        private void DealInitialHand()
        {
            _deck.Reset(); // Ensure full deck for every hand
            _deck.Shuffle(); // Always shuffle before a new hand
            CurrentHand = _deck.DealCards(5);
            HeldCards = new List<bool> { false, false, false, false, false };
            CurrentState = GameState.Dealt;
            LastWin = 0;
            LastHandRank = HandRank.HighCard;
        }

        public void ToggleHold(int cardIndex)
        {
            if (CurrentState != GameState.Dealt)
                throw new InvalidOperationException("Cannot hold cards now.");
            
            if (cardIndex < 0 || cardIndex >= 5)
                throw new ArgumentOutOfRangeException(nameof(cardIndex));

            HeldCards[cardIndex] = !HeldCards[cardIndex];
        }

        public void Draw()
        {
            if (CurrentState != GameState.Dealt)
                throw new InvalidOperationException("Cannot draw now.");

            List<Card> cardsToReplace = new List<Card>();
            for (int i = 0; i < 5; i++)
            {
                if (!HeldCards[i])
                {
                    cardsToReplace.Add(CurrentHand[i]);
                }
            }

            _deck.ReplaceCards(CurrentHand, cardsToReplace);
            
            // Evaluate
            LastHandRank = HandEvaluator.EvaluateHand(CurrentHand);
            LastWin = CalculatePayout(LastHandRank, CurrentBet);
            _bankroll.AddWin(LastWin);

            CurrentState = GameState.GameOver;
        }

        public void StartDoubleUp()
        {
            if (CurrentState != GameState.GameOver || LastWin <= 0)
                throw new InvalidOperationException("Cannot double up now.");

            _deck.Reset();
            _deck.Shuffle();
            
            // Deal 5 cards: 1st is Dealer's (Face Up), others are Player's options (Face Down)
            CurrentHand = _deck.DealCards(5);
            
            // In UI, we will show Card 0 face up, others face down.
            // Logic doesn't need to know about face up/down, just that we are in DoubleUp state.
            
            CurrentState = GameState.DoubleUp;
        }

        public bool PlayDoubleUp(int cardIndex)
        {
            if (CurrentState != GameState.DoubleUp)
                throw new InvalidOperationException("Not in double up phase.");

            if (cardIndex <= 0 || cardIndex >= 5)
                throw new ArgumentOutOfRangeException(nameof(cardIndex), "Must pick one of the 4 player cards (index 1-4).");

            Card dealerCard = CurrentHand[0];
            Card playerCard = CurrentHand[cardIndex];

            // Compare
            int comparison = playerCard.CompareTo(dealerCard);
            
            if (comparison > 0) // Player wins
            {
                LastWin *= 2;
                _bankroll.AddWin(LastWin / 2); // Add the other half (original win was already added)
                // Wait for player to Collect or Double again. 
                // We stay in DoubleUp state effectively, but UI needs to show result.
                // Actually, let's keep state as DoubleUp so they can choose to double again or collect.
                // But we need to reset the "round" for next double. 
                // For simplicity, let's say one double attempt per "StartDoubleUp" call? 
                // No, standard is recursive.
                // Let's return true for Win.
                return true;
            }
            else if (comparison < 0) // Player loses
            {
                _bankroll.Bet(LastWin); // Deduct the win we just thought we had (it was already added to bankroll)
                LastWin = 0;
                CurrentState = GameState.GameOver;
                return false;
            }
            else // Tie (Push)
            {
                // Push: Player keeps money, can try again or collect.
                return true; // Treated as "not lost"
            }
        }

        public void Collect()
        {
            if (CurrentState != GameState.DoubleUp && CurrentState != GameState.GameOver)
                 throw new InvalidOperationException("Nothing to collect.");

            CurrentState = GameState.WaitingForBet;
            // Money is already in bankroll from Draw() or PlayDoubleUp() updates.
        }

        private int CalculatePayout(HandRank rank, int bet)
        {
            // Standard 9/6 Jacks or Better Pay Table
            int multiplier = 0;
            switch (rank)
            {
                case HandRank.RoyalFlush:
                    // Bonus for max bet (5 coins) is usually 4000, but we'll keep it linear 800x or 250x depending on rules.
                    // Standard machine: 250x for 1-4 coins, 800x for 5 coins.
                    // Let's implement the big bonus for 5 coins.
                    if (bet == 5) return 4000; 
                    multiplier = 250;
                    break;
                case HandRank.StraightFlush:
                    multiplier = 50;
                    break;
                case HandRank.FourOfAKind:
                    multiplier = 25;
                    break;
                case HandRank.FullHouse:
                    multiplier = 9;
                    break;
                case HandRank.Flush:
                    multiplier = 6;
                    break;
                case HandRank.Straight:
                    multiplier = 4;
                    break;
                case HandRank.ThreeOfAKind:
                    multiplier = 3;
                    break;
                case HandRank.TwoPair:
                    multiplier = 2;
                    break;
                case HandRank.JacksOrBetter:
                    multiplier = 1;
                    break;
                default:
                    multiplier = 0;
                    break;
            }
            return bet * multiplier;
        }

        public void Reset()
        {
            CurrentState = GameState.WaitingForBet;
            CurrentHand.Clear();
            HeldCards.Clear();
        }
    }
}
