using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PokerGame
{
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public void SetCard(Card card, bool faceUp = true)
        {
            UpdateCardVisuals(card);
            if (faceUp)
            {
                CardFront.Visibility = Visibility.Visible;
                CardBack.Visibility = Visibility.Collapsed;
            }
            else
            {
                CardFront.Visibility = Visibility.Collapsed;
                CardBack.Visibility = Visibility.Visible;
            }
        }

        public async Task FlipTo(Card card)
        {
            Storyboard close = (Storyboard)Resources["FlipClose"];
            Storyboard open = (Storyboard)Resources["FlipOpen"];

            // Animate Close
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = null;
            handler = (s, e) => 
            {
                close.Completed -= handler;
                tcs.SetResult(true);
            };
            close.Completed += handler;
            close.Begin();
            await tcs.Task;

            // Change Content
            UpdateCardVisuals(card);
            CardFront.Visibility = Visibility.Visible;
            CardBack.Visibility = Visibility.Collapsed;
            HeldOverlay.Visibility = Visibility.Collapsed; // Reset held status on flip
            SetWinning(false); // Reset win status on flip

            // Animate Open
            open.Begin();
        }

        private void UpdateCardVisuals(Card card)
        {
            if (card == null) return;

            string suitSymbol = "";
            Brush color = Brushes.Black;

            switch (card.Suit)
            {
                case Suit.Clubs: suitSymbol = "♣"; color = Brushes.Black; break;
                case Suit.Diamonds: suitSymbol = "♦"; color = Brushes.Red; break;
                case Suit.Hearts: suitSymbol = "♥"; color = Brushes.Red; break;
                case Suit.Spades: suitSymbol = "♠"; color = Brushes.Black; break;
            }

            string rankText = "";
            if (card.Rank >= Rank.Two && card.Rank <= Rank.Nine)
                rankText = ((int)card.Rank).ToString();
            else
                rankText = card.Rank.ToString().Substring(0, 1);

            if (card.Rank == Rank.Ten) rankText = "10";

            TopRank.Text = rankText;
            BottomRank.Text = rankText;
            TopSuit.Text = suitSymbol;
            BottomSuit.Text = suitSymbol;
            CenterSuit.Text = suitSymbol;

            TopRank.Foreground = color;
            BottomRank.Foreground = color;
            TopSuit.Foreground = color;
            BottomSuit.Foreground = color;
            CenterSuit.Foreground = color;
        }

        public void SetHeld(bool isHeld)
        {
            HeldOverlay.Visibility = isHeld ? Visibility.Visible : Visibility.Collapsed;
        }
        
        public void ShowBack()
        {
            CardFront.Visibility = Visibility.Collapsed;
            CardBack.Visibility = Visibility.Visible;
            SetWinning(false);
        }

        public void SetWinning(bool isWinning)
        {
            if (isWinning)
            {
                WinBorder.Visibility = Visibility.Visible;
                Storyboard winAnim = (Storyboard)Resources["WinAnimation"];
                winAnim.Begin();
            }
            else
            {
                WinBorder.Visibility = Visibility.Collapsed;
                Storyboard winAnim = (Storyboard)Resources["WinAnimation"];
                winAnim.Stop();
            }
        }
    }
}
