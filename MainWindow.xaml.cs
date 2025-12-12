using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private ShellViewModel _shellViewModel;
        private VideoPokerViewModel _gameViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _persistenceManager = new PersistenceManager();
            _soundManager = new SoundManager();

            // Start with Game Selection
            var selectionVM = new GameSelectionViewModel(
                () => StartGame(new JacksOrBetterVariant()),
                () => StartGame(new DeucesWildVariant()),
                () => this.Close()
            );

            _shellViewModel = new ShellViewModel(selectionVM, () => 
            {
                // On Exit Game (Return to Menu)
                _shellViewModel.NavigateTo(selectionVM);
            });

            DataContext = _shellViewModel;
            
            this.Closing += MainWindow_Closing;
        }

        private void StartGame(IGameVariant variant)
        {
            var saveData = _persistenceManager.Load();
            _game = new VideoPokerGame(variant, saveData.Credits);
            _cardControls = null; // Force re-discovery of controls for the new view
            
            // Re-initialize card controls list when the view is loaded?
            // The CardControls are inside the DataTemplate, so they are not accessible via 'this.Card1' anymore.
            // We need to find them when the view is loaded or passed via event args.
            // But Card_MouseDown handles interaction.
            // Animation needs reference to CardControls.
            
            _gameViewModel = new VideoPokerViewModel(
                BetOneAction,
                BetMaxAction,
                DealDrawAsync,
                DoubleAsync,
                CollectAction,
                () => _shellViewModel.ReturnToMenuCommand.Execute(null));
            
            _gameViewModel.UpdateFromGame(_game, _isAnimating);
            
            _shellViewModel.NavigateTo(_gameViewModel);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_game != null)
            {
                _persistenceManager.Save(_game.Credits);
            }
        }

        private void RefreshUI()
        {
            if (_gameViewModel != null && _game != null)
            {
                _gameViewModel.UpdateFromGame(_game, _isAnimating);

                if (_game.CurrentState == GameState.Dealt)
                {
                    // We need to update card held status.
                    // Since we don't have direct reference to controls easily (DataTemplate),
                    // we rely on bindings or we need to find them.
                    // For now, let's assume we can find them or we bind 'IsHeld' in the view model?
                    // But CardControl has visual state.
                    
                    // Quick fix: Find controls in the visual tree or use a helper.
                    // Since we are in CodeBehind of MainWindow, we can look into the ContentControl.
                    if (_cardControls != null)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            _cardControls[i].SetHeld(_game.HeldCards[i]);
                        }
                    }
                }
            }
        }

        // Helper to find CardControls. 
        // We will populate _cardControls when the Game View is loaded.
        // But how do we know when it's loaded?
        // We can hook into the Loaded event of the UserControl if we had one.
        // Or we can search the visual tree when needed.
        
        private void EnsureCardControls()
        {
             if (_cardControls == null || _cardControls.Count != 5)
             {
                 _cardControls = new List<CardControl>();
                 // Find Card1..Card5 in the visual tree
                 // This is a bit hacky but works for this refactor.
                 // Better: Use a UserControl for GameView and expose the cards.
                 
                 var contentPresenter = FindVisualChild<ContentPresenter>(this);
                 if (contentPresenter != null)
                 {
                     var card1 = FindChild<CardControl>(contentPresenter, "Card1");
                     var card2 = FindChild<CardControl>(contentPresenter, "Card2");
                     var card3 = FindChild<CardControl>(contentPresenter, "Card3");
                     var card4 = FindChild<CardControl>(contentPresenter, "Card4");
                     var card5 = FindChild<CardControl>(contentPresenter, "Card5");
                     
                     if (card1 != null) _cardControls.Add(card1);
                     if (card2 != null) _cardControls.Add(card2);
                     if (card3 != null) _cardControls.Add(card3);
                     if (card4 != null) _cardControls.Add(card4);
                     if (card5 != null) _cardControls.Add(card5);
                 }
             }
        }

        private void BetOneAction()
        {
            if (_game.CurrentState == GameState.WaitingForBet && !_isAnimating)
            {
                _soundManager.PlayClick();
                _gameViewModel.SelectedBet++;
                if (_gameViewModel.SelectedBet > 5) _gameViewModel.SelectedBet = 1;
                if (_gameViewModel.SelectedBet > _game.Credits)
                {
                    _gameViewModel.SelectedBet = Math.Max(1, Math.Min(5, _game.Credits));
                }
                RefreshUI();
            }
        }

        private void BetMaxAction()
        {
            if (_game.CurrentState == GameState.WaitingForBet && !_isAnimating)
            {
                _soundManager.PlayClick();
                _gameViewModel.SelectedBet = Math.Min(5, _game.Credits);
                if (_gameViewModel.SelectedBet < 1) _gameViewModel.SelectedBet = 1;
                RefreshUI();
            }
        }

        private async Task DealDrawAsync()
        {
            if (_isAnimating) return;
            EnsureCardControls();
            if (_cardControls.Count < 5) return; // View not ready?

            try
            {
                _isAnimating = true;
                RefreshUI(); // Disable buttons

                if (_game.CurrentState == GameState.WaitingForBet)
                {
                    var bet = Math.Max(1, Math.Min(_gameViewModel.SelectedBet, _game.Credits));
                    _gameViewModel.SelectedBet = bet;
                    _game.PlaceBet(bet);

                    // Animate Deal
                    _soundManager.PlayDeal();
                    _gameViewModel.SetStatus("Dealing...");
                    for (int i = 0; i < 5; i++)
                    {
                        var card = _game.CurrentHand[i];
                        await _cardControls[i].FlipTo(card, _game.IsCardWild(card));
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
                    _gameViewModel.SetStatus("Drawing...");
                    
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
                            var card = _game.CurrentHand[i];
                            await _cardControls[i].FlipTo(card, _game.IsCardWild(card));
                            await Task.Delay(100);
                        }
                    }

                    // Check for win and highlight
                    if (_game.LastWin > 0)
                    {
                        _soundManager.PlayWin(_game.LastHandRank);
                        var winningCards = _game.GetWinningCards();
                        
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
                         card.SetWinning(false);
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
            EnsureCardControls();
            try
            {
                _game.StartDoubleUp();
                RefreshUI();
                
                // Show Dealer Card (Index 0)
                var dealerCard = _game.CurrentHand[0];
                await _cardControls[0].FlipTo(dealerCard, _game.IsCardWild(dealerCard));
                
                // Show Backs for others
                for (int i = 1; i < 5; i++)
                {
                    _cardControls[i].ShowBack();
                    _cardControls[i].SetHeld(false);
                    _cardControls[i].SetWinning(false);
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
            EnsureCardControls();

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
                            var card = _game.CurrentHand[index];
                            await cardControl.FlipTo(card, _game.IsCardWild(card));
                            
                            if (win)
                            {
                                _soundManager.PlayWin(HandRank.HighCard); // Simple win sound
                                _gameViewModel.SetStatus($"YOU WIN! Total: {_game.LastWin}", Brushes.Yellow);
                                cardControl.SetWinning(true); // Highlight the winning card
                            }
                            else
                            {
                                _soundManager.PlayGameOver();
                                _gameViewModel.SetStatus("YOU LOSE");
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

        // Helper methods to find children in Visual Tree
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild) return typedChild;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    if (!string.IsNullOrEmpty(childName))
                    {
                        var frameworkElement = child as FrameworkElement;
                        if (frameworkElement != null && frameworkElement.Name == childName)
                        {
                            foundChild = typedChild;
                            break;
                        }
                    }
                    else
                    {
                        foundChild = typedChild;
                        break;
                    }
                }
                foundChild = FindChild<T>(child, childName);
                if (foundChild != null) break;
            }
            return foundChild;
        }
    }
}
