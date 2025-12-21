using System.Windows.Input;

namespace PokerGame
{
    public class GameSelectionViewModel : ViewModelBase
    {
        private readonly System.Action _onJacksOrBetterSelected;
        private readonly System.Action _onDeucesWildSelected;
        private readonly System.Action _onDoubleDoubleBonusSelected;
        private readonly System.Action _onQuit;

        public ICommand SelectJacksOrBetterCommand { get; }
        public ICommand SelectDeucesWildCommand { get; }
        public ICommand SelectDoubleDoubleBonusCommand { get; }
        public ICommand QuitCommand { get; }

        public GameSelectionViewModel(System.Action onJacksOrBetterSelected, System.Action onDeucesWildSelected, System.Action onDoubleDoubleBonusSelected, System.Action onQuit)
        {
            _onJacksOrBetterSelected = onJacksOrBetterSelected;
            _onDeucesWildSelected = onDeucesWildSelected;
            _onDoubleDoubleBonusSelected = onDoubleDoubleBonusSelected;
            _onQuit = onQuit;

            SelectJacksOrBetterCommand = new RelayCommand(_ => _onJacksOrBetterSelected());
            SelectDeucesWildCommand = new RelayCommand(_ => _onDeucesWildSelected());
            SelectDoubleDoubleBonusCommand = new RelayCommand(_ => _onDoubleDoubleBonusSelected());
            QuitCommand = new RelayCommand(_ => _onQuit());
        }
    }
}
