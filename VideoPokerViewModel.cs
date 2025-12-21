using System.Threading.Tasks;
using System.Windows.Media;

namespace PokerGame
{
    public class VideoPokerViewModel : ViewModelBase
    {
        private readonly RelayCommand _betOneCommand;
        private readonly RelayCommand _betMaxCommand;
        private readonly AsyncRelayCommand _dealDrawCommand;
        private readonly AsyncRelayCommand _doubleCommand;
        private readonly RelayCommand _collectCommand;
        private readonly RelayCommand _returnToMenuCommand;

        private int _credits;
        private int _selectedBet = 1;
        private string _statusText = "Place your bet!";
        private Brush _statusBrush = Brushes.White;
        private string _dealDrawLabel = "DEAL";
        private bool _showBetControls = true;
        private bool _showDealDrawButton = true;
        private bool _showDoubleButton;
        private bool _showCollectButton;
        private bool _isBetEnabled = true;
        private bool _isDealDrawEnabled = true;
        private System.Collections.ObjectModel.ObservableCollection<PaytableRow>? _currentPaytable;

        public System.Collections.ObjectModel.ObservableCollection<PaytableRow>? CurrentPaytable
        {
            get => _currentPaytable;
            set => SetField(ref _currentPaytable, value);
        }

        public VideoPokerViewModel(
            System.Action betOneAction,
            System.Action betMaxAction,
            System.Func<Task> dealDrawAction,
            System.Func<Task> doubleAction,
            System.Action collectAction,
            System.Action returnToMenuAction)
        {
            _betOneCommand = new RelayCommand(_ => betOneAction(), _ => IsBetEnabled);
            _betMaxCommand = new RelayCommand(_ => betMaxAction(), _ => IsBetEnabled);
            _dealDrawCommand = new AsyncRelayCommand(dealDrawAction, () => IsDealDrawEnabled && ShowDealDrawButton);
            _doubleCommand = new AsyncRelayCommand(doubleAction, () => ShowDoubleButton);
            _collectCommand = new RelayCommand(_ => collectAction(), _ => ShowCollectButton);
            _returnToMenuCommand = new RelayCommand(_ => returnToMenuAction());
        }

        public int Credits
        {
            get => _credits;
            set => SetField(ref _credits, value);
        }

        public int SelectedBet
        {
            get => _selectedBet;
            set => SetField(ref _selectedBet, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetField(ref _statusText, value);
        }

        public Brush StatusBrush
        {
            get => _statusBrush;
            set => SetField(ref _statusBrush, value);
        }

        public string DealDrawLabel
        {
            get => _dealDrawLabel;
            set => SetField(ref _dealDrawLabel, value);
        }

        public bool ShowBetControls
        {
            get => _showBetControls;
            set => SetField(ref _showBetControls, value);
        }

        public bool ShowDealDrawButton
        {
            get => _showDealDrawButton;
            set => SetField(ref _showDealDrawButton, value);
        }

        public bool ShowDoubleButton
        {
            get => _showDoubleButton;
            set => SetField(ref _showDoubleButton, value);
        }

        public bool ShowCollectButton
        {
            get => _showCollectButton;
            set => SetField(ref _showCollectButton, value);
        }

        public bool IsBetEnabled
        {
            get => _isBetEnabled;
            set => SetField(ref _isBetEnabled, value);
        }

        public bool IsDealDrawEnabled
        {
            get => _isDealDrawEnabled;
            set => SetField(ref _isDealDrawEnabled, value);
        }

        public System.Windows.Input.ICommand BetOneCommand => _betOneCommand;
        public System.Windows.Input.ICommand BetMaxCommand => _betMaxCommand;
        public System.Windows.Input.ICommand DealDrawCommand => _dealDrawCommand;
        public System.Windows.Input.ICommand DoubleCommand => _doubleCommand;
        public System.Windows.Input.ICommand CollectCommand => _collectCommand;
        public System.Windows.Input.ICommand ReturnToMenuCommand => _returnToMenuCommand;

        public void UpdateFromGame(VideoPokerGame game, bool isAnimating)
        {
            if (CurrentPaytable == null)
            {
                CurrentPaytable = new System.Collections.ObjectModel.ObservableCollection<PaytableRow>(game.GetPaytable());
            }

            Credits = game.Credits;

            switch (game.CurrentState)
            {
                case GameState.WaitingForBet:
                    DealDrawLabel = "DEAL";
                    ShowBetControls = true;
                    ShowDealDrawButton = true;
                    ShowDoubleButton = false;
                    ShowCollectButton = false;
                    IsBetEnabled = !isAnimating;
                    IsDealDrawEnabled = !isAnimating;
                    StatusText = "Place your bet!";
                    StatusBrush = Brushes.White;
                    break;

                case GameState.DoubleUp:
                    DealDrawLabel = "DRAW";
                    ShowBetControls = false;
                    ShowDealDrawButton = false;
                    ShowDoubleButton = false; // During Double Up, player must pick a card, not restart the round
                    ShowCollectButton = false; // Prevent collecting before choosing a card in Double Up
                    IsBetEnabled = false;
                    IsDealDrawEnabled = false;
                    StatusText = "Pick a card higher than the Dealer's (Left)";
                    StatusBrush = Brushes.White;
                    break;

                case GameState.Dealt:
                    DealDrawLabel = "DRAW";
                    ShowBetControls = false;
                    ShowDealDrawButton = true;
                    ShowDoubleButton = false;
                    ShowCollectButton = false;
                    IsBetEnabled = false;
                    IsDealDrawEnabled = !isAnimating;
                    StatusText = "Select cards to hold...";
                    StatusBrush = Brushes.White;
                    break;

                case GameState.GameOver:
                    DealDrawLabel = "NEW GAME";
                    ShowBetControls = true;
                    ShowDealDrawButton = game.LastWin <= 0;
                    ShowDoubleButton = game.LastWin > 0 && !isAnimating;
                    ShowCollectButton = game.LastWin > 0 && !isAnimating;
                    IsBetEnabled = false;
                    IsDealDrawEnabled = !isAnimating;
                    if (game.LastWin > 0)
                    {
                        StatusText = $"{game.LastHandRank} - YOU WIN {game.LastWin}!";
                        StatusBrush = Brushes.Yellow;
                    }
                    else
                    {
                        StatusText = "Game Over";
                        StatusBrush = Brushes.White;
                    }
                    break;
            }

            RaiseCommandCanExecute();
        }

        public void SetStatus(string message, Brush? brush = null)
        {
            StatusText = message;
            if (brush != null)
            {
                StatusBrush = brush;
            }
        }

        private void RaiseCommandCanExecute()
        {
            _betOneCommand.RaiseCanExecuteChanged();
            _betMaxCommand.RaiseCanExecuteChanged();
            _dealDrawCommand.RaiseCanExecuteChanged();
            _doubleCommand.RaiseCanExecuteChanged();
            _collectCommand.RaiseCanExecuteChanged();
        }
    }
}
