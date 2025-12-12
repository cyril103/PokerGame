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
        private readonly IGameVariant _variant;
        
        public List<Card> CurrentHand { get; private set; }
        public List<bool> HeldCards { get; private set; }
        public GameState CurrentState { get; private set; }
        public int CurrentBet { get; private set; }
        public int LastWin { get; private set; }
        public HandRank LastHandRank { get; private set; }

        public int Credits => _bankroll.Credits;
        public string VariantName => _variant.Name;

        public VideoPokerGame(IGameVariant variant, int initialCredits = 100)
        {
            _variant = variant ?? throw new ArgumentNullException(nameof(variant));
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
            
            // Evaluate using the variant
            LastHandRank = _variant.EvaluateHand(CurrentHand);
            LastWin = _variant.CalculatePayout(LastHandRank, CurrentBet);
            _bankroll.AddWin(LastWin);

            CurrentState = GameState.GameOver;
        }

        public void StartDoubleUp()
        {
            if (LastWin <= 0)
                throw new InvalidOperationException("Cannot double up now.");

            if (CurrentState != GameState.GameOver)
                throw new InvalidOperationException("Double up not available.");

            PrepareDoubleUpRound();
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

            // Compare ranks only for Double Up (suit should not decide outcome)
            int comparison = playerCard.Rank.CompareTo(dealerCard.Rank);
            
            if (comparison > 0) // Player wins
            {
                LastWin *= 2;
                _bankroll.AddWin(LastWin / 2); // Add the other half (original win was already added)
                CurrentState = GameState.GameOver; // Let UI offer Collect or Double again
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
                CurrentState = GameState.GameOver;
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

        public List<Card> GetWinningCards()
        {
            return _variant.GetWinningCards(CurrentHand, LastHandRank);
        }

        public bool IsCardWild(Card card)
        {
            return _variant.IsCardWild(card);
        }

        public List<PaytableRow> GetPaytable()
        {
            return _variant.GetPaytable();
        }

        public void Reset()
        {
            CurrentState = GameState.WaitingForBet;
            CurrentHand.Clear();
            HeldCards.Clear();
            
            // Auto top-up so player can keep playing after bust
            if (_bankroll.Credits <= 0)
            {
                _bankroll.Deposit(100);
            }
        }

        private void PrepareDoubleUpRound()
        {
            _deck.Reset();
            _deck.Shuffle();

            // Deal 5 cards: 1st is Dealer's (Face Up), others are Player's options (Face Down)
            CurrentHand = _deck.DealCards(5);
            HeldCards = new List<bool> { false, false, false, false, false };
        }
    }
}
