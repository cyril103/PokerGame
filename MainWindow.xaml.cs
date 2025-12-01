using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PokerGame
{
    public partial class MainWindow : Window
    {
        private VideoPokerGame _game;
        private int _selectedBet = 1;
        private List<CardControl> _cardControls;
        private bool _isAnimating = false;
        private SoundManager _soundManager;
        private PersistenceManager _persistenceManager;

        public MainWindow()
        {
            InitializeComponent();
            _persistenceManager = new PersistenceManager();
            var saveData = _persistenceManager.Load();
            
            _game = new VideoPokerGame(saveData.Credits);
            _soundManager = new SoundManager();
            _cardControls = new List<CardControl> { Card1, Card2, Card3, Card4, Card5 };
            
            // Initialize cards face down
            foreach(var card in _cardControls) card.ShowBack();
            
            UpdateUI();
            
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _persistenceManager.Save(_game.Credits);
        }

        private void UpdateUI()
        {
            CreditsText.Text = _game.Credits.ToString();
            BetText.Text = _selectedBet.ToString();

            if (_game.CurrentState == GameState.WaitingForBet)
            {
                BtnDealDraw.Content = "DEAL";
                StatusText.Text = "Place your bet!";
                BtnBetOne.IsEnabled = !_isAnimating;
                BtnBetMax.IsEnabled = !_isAnimating;
                BtnDealDraw.IsEnabled = !_isAnimating;
                
                BtnDouble.Visibility = Visibility.Collapsed;
                BtnCollect.Visibility = Visibility.Collapsed;
                BtnBetOne.Visibility = Visibility.Visible;
                BtnBetMax.Visibility = Visibility.Visible;
                BtnDealDraw.Visibility = Visibility.Visible;
            }
            else if (_game.CurrentState == GameState.DoubleUp)
            {
                BtnDealDraw.Visibility = Visibility.Collapsed;
                BtnBetOne.Visibility = Visibility.Collapsed;
                BtnBetMax.Visibility = Visibility.Collapsed;
                
                BtnDouble.Visibility = Visibility.Collapsed; 
                BtnCollect.Visibility = Visibility.Collapsed; 
                
                StatusText.Text = "Pick a card higher than the Dealer's (Left)";
            }
            else if (_game.CurrentState == GameState.Dealt)
            {
                BtnDealDraw.Content = "DRAW";
                StatusText.Text = "Select cards to hold...";
                BtnBetOne.IsEnabled = false;
                BtnBetMax.IsEnabled = false;
                BtnDealDraw.IsEnabled = !_isAnimating;
                
                // Update hold status only (cards are updated via animation)
                for (int i = 0; i < 5; i++)
                {
                    _cardControls[i].SetHeld(_game.HeldCards[i]);
                }
            }
            else if (_game.CurrentState == GameState.GameOver)
            {
                BtnDealDraw.Content = "NEW GAME";
                BtnBetOne.IsEnabled = false;
                BtnBetMax.IsEnabled = false;
                BtnDealDraw.IsEnabled = !_isAnimating;
                
                BtnBetOne.Visibility = Visibility.Visible;
                BtnBetMax.Visibility = Visibility.Visible;
                BtnDealDraw.Visibility = Visibility.Visible;

                if (_game.LastWin > 0)
                {
                    StatusText.Text = $"{_game.LastHandRank} - YOU WIN {_game.LastWin}!";
                    StatusText.Foreground = Brushes.Yellow;
                    
                    // Show Double/Collect options if we won
                    BtnDouble.Visibility = Visibility.Visible;
                    BtnCollect.Visibility = Visibility.Visible;
                    BtnDealDraw.Visibility = Visibility.Collapsed;
                    BtnBetOne.Visibility = Visibility.Collapsed;
                    BtnBetMax.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusText.Text = "Game Over";
                    StatusText.Foreground = Brushes.White;
                    BtnDouble.Visibility = Visibility.Collapsed;
                    BtnCollect.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnBetOne_Click(object sender, RoutedEventArgs e)
        {
            if (_game.CurrentState == GameState.WaitingForBet && !_isAnimating)
            {
                _soundManager.PlayClick();
                _selectedBet++;
                if (_selectedBet > 5) _selectedBet = 1;
                UpdateUI();
            }
        }

        private void BtnBetMax_Click(object sender, RoutedEventArgs e)
        {
            if (_game.CurrentState == GameState.WaitingForBet && !_isAnimating)
            {
                _soundManager.PlayClick();
                _selectedBet = 5;
                UpdateUI();
            }
        }

        private async void BtnDealDraw_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnimating) return;

            try
            {
                _isAnimating = true;
                UpdateUI(); // Disable buttons

                if (_game.CurrentState == GameState.WaitingForBet)
                {
                    _game.PlaceBet(_selectedBet);
                    
                    // Animate Deal
                    _soundManager.PlayDeal();
                    StatusText.Text = "Dealing...";
                    for (int i = 0; i < 5; i++)
                    {
                        await _cardControls[i].FlipTo(_game.CurrentHand[i]);
                        await Task.Delay(100); // Stagger effect
                    }
                }
                else if (_game.CurrentState == GameState.Dealt)
                {
                    // Capture old hand to know which ones changed
                    var oldHand = new List<Card>(_game.CurrentHand);
                    var oldHeld = new List<bool>(_game.HeldCards);
                    
                    _game.Draw();

                    // Animate Draw (only replaced cards)
                    _soundManager.PlayDeal();
                    StatusText.Text = "Drawing...";
                    
                    // Clear "HELD" overlay immediately for held cards so they are visible ("en clair")
                    for (int i = 0; i < 5; i++)
                    {
                        if (oldHeld[i])
                        {
                             _cardControls[i].SetHeld(false); 
                        }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        if (!oldHeld[i])
                        {
                            await _cardControls[i].FlipTo(_game.CurrentHand[i]);
                            await Task.Delay(100);
                        }
                    }

                    // Check for win and highlight
                    if (_game.LastWin > 0)
                    {
                        _soundManager.PlayWin(_game.LastHandRank);
                        var winningCards = HandEvaluator.GetWinningCards(_game.CurrentHand, _game.LastHandRank);
                        
                        // Highlight matching cards
                        for (int i = 0; i < 5; i++)
                        {
                            if (winningCards.Contains(_game.CurrentHand[i]))
                            {
                                _cardControls[i].SetWinning(true);
                            }
                        }
                    }
                    else
                    {
                         // _soundManager.PlayGameOver(); // Optional
                    }
                }
                else if (_game.CurrentState == GameState.GameOver)
                {
                    _game.Reset();
                    // Flip all back to face down for new game feel?
                    // Or just leave them and flip new ones on deal.
                    // Let's flip them face down for effect.
                    foreach(var card in _cardControls)
                    {
                         card.ShowBack();
                         card.SetHeld(false);
                    }
                    await Task.Delay(200);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _isAnimating = false;
                UpdateUI();
            }
        }

        private async void BtnDouble_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnimating) return;
            try
            {
                _game.StartDoubleUp();
                UpdateUI();
                
                // Show Dealer Card (Index 0)
                await _cardControls[0].FlipTo(_game.CurrentHand[0]);
                
                // Show Backs for others
                for (int i = 1; i < 5; i++)
                {
                    _cardControls[i].ShowBack();
                    _cardControls[i].SetHeld(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnCollect_Click(object sender, RoutedEventArgs e)
        {
             if (_isAnimating) return;
             try
             {
                 _soundManager.PlayClick();
                 _game.Collect();
                 UpdateUI();
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
        }
        private async void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isAnimating) return;

            if (_game.CurrentState == GameState.Dealt)
            {
                if (sender is CardControl cardControl)
                {
                    int index = _cardControls.IndexOf(cardControl);
                    if (index >= 0)
                    {
                        _soundManager.PlayClick();
                        _game.ToggleHold(index);
                        UpdateUI();
                    }
                }
            }
            else if (_game.CurrentState == GameState.DoubleUp)
            {
                if (sender is CardControl cardControl)
                {
                    int index = _cardControls.IndexOf(cardControl);
                    // Must pick index 1-4
                    if (index >= 1 && index <= 4)
                    {
                        _isAnimating = true;
                        try 
                        {
                            bool win = _game.PlayDoubleUp(index);
                            
                            // Reveal card
                            await cardControl.FlipTo(_game.CurrentHand[index]);
                            
                            if (win)
                            {
                                _soundManager.PlayWin(HandRank.HighCard); // Simple win sound
                                StatusText.Text = $"YOU WIN! Total: {_game.LastWin}";
                                BtnDouble.Visibility = Visibility.Visible;
                                BtnCollect.Visibility = Visibility.Visible;
                                cardControl.SetWinning(true); // Highlight the winning card
                            }
                            else
                            {
                                _soundManager.PlayGameOver();
                                StatusText.Text = "YOU LOSE";
                                await Task.Delay(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            _isAnimating = false;
                            if (_game.CurrentState == GameState.GameOver)
                            {
                                UpdateUI();
                            }
                        }
                    }
                }
            }
        }
    }
}
