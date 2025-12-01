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
        private List<CardControl> _cardControls;
        private bool _isAnimating = false;
        private SoundManager _soundManager;
        private PersistenceManager _persistenceManager;
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _persistenceManager = new PersistenceManager();
            var saveData = _persistenceManager.Load();

            _game = new VideoPokerGame(saveData.Credits);
            _soundManager = new SoundManager();
            _cardControls = new List<CardControl> { Card1, Card2, Card3, Card4, Card5 };

            _viewModel = new MainViewModel(
                BetOneAction,
                BetMaxAction,
                DealDrawAsync,
                DoubleAsync,
                CollectAction);

            DataContext = _viewModel;

            // Initialize cards face down
            foreach(var card in _cardControls) card.ShowBack();

            RefreshUI();

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _persistenceManager.Save(_game.Credits);
        }

        private void RefreshUI()
        {
            _viewModel.UpdateFromGame(_game, _isAnimating);

            if (_game.CurrentState == GameState.Dealt)
            {
                for (int i = 0; i < 5; i++)
                {
                    _cardControls[i].SetHeld(_game.HeldCards[i]);
                }
            }
        }

        private void BetOneAction()
        {
            if (_game.CurrentState == GameState.WaitingForBet && !_isAnimating)
            {
                _soundManager.PlayClick();
                _viewModel.SelectedBet++;
                if (_viewModel.SelectedBet > 5) _viewModel.SelectedBet = 1;
                if (_viewModel.SelectedBet > _game.Credits)
                {
                    _viewModel.SelectedBet = Math.Max(1, Math.Min(5, _game.Credits));
                }
                RefreshUI();
            }
        }

        private void BetMaxAction()
        {
            if (_game.CurrentState == GameState.WaitingForBet && !_isAnimating)
            {
                _soundManager.PlayClick();
                _viewModel.SelectedBet = Math.Min(5, _game.Credits);
                if (_viewModel.SelectedBet < 1) _viewModel.SelectedBet = 1;
                RefreshUI();
            }
        }

        private async Task DealDrawAsync()
        {
            if (_isAnimating) return;
            try
            {
                _isAnimating = true;
                RefreshUI(); // Disable buttons

                if (_game.CurrentState == GameState.WaitingForBet)
                {
                    var bet = Math.Max(1, Math.Min(_viewModel.SelectedBet, _game.Credits));
                    _viewModel.SelectedBet = bet;
                    _game.PlaceBet(bet);

                    // Animate Deal
                    _soundManager.PlayDeal();
                    _viewModel.SetStatus("Dealing...");
                    for (int i = 0; i < 5; i++)
                    {
                        await _cardControls[i].FlipTo(_game.CurrentHand[i]);
                        await Task.Delay(100); // Stagger effect
                    }
                }
                else if (_game.CurrentState == GameState.Dealt)
                {
                    // Capture old hand to know which ones changed
                    var oldHeld = new List<bool>(_game.HeldCards);
                    
                    _game.Draw();

                    // Animate Draw (only replaced cards)
                    _soundManager.PlayDeal();
                    _viewModel.SetStatus("Drawing...");
                    
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
                RefreshUI();
            }
        }

        private async Task DoubleAsync()
        {
            if (_isAnimating) return;
            try
            {
                _game.StartDoubleUp();
                RefreshUI();
                
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

        private void CollectAction()
        {
             if (_isAnimating) return;
             try
             {
                 _soundManager.PlayClick();
                 _game.Collect();
                 RefreshUI();
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
                        RefreshUI();
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
                                _viewModel.SetStatus($"YOU WIN! Total: {_game.LastWin}", Brushes.Yellow);
                                cardControl.SetWinning(true); // Highlight the winning card
                            }
                            else
                            {
                                _soundManager.PlayGameOver();
                                _viewModel.SetStatus("YOU LOSE");
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
                            RefreshUI();
                        }
                    }
                }
            }
        }
    }
}
