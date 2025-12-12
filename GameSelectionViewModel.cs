using System.Windows.Input;

namespace PokerGame
{
    public class GameSelectionViewModel : ViewModelBase
    {
        private readonly System.Action _onJacksOrBetterSelected;
        private readonly System.Action _onDeucesWildSelected;
        private readonly System.Action _onQuit;

        public ICommand SelectJacksOrBetterCommand { get; }
        public ICommand SelectDeucesWildCommand { get; }
        public ICommand QuitCommand { get; }

        public GameSelectionViewModel(System.Action onJacksOrBetterSelected, System.Action onDeucesWildSelected, System.Action onQuit)
        {
            _onJacksOrBetterSelected = onJacksOrBetterSelected;
            _onDeucesWildSelected = onDeucesWildSelected;
            _onQuit = onQuit;

            SelectJacksOrBetterCommand = new RelayCommand(_ => _onJacksOrBetterSelected());
            SelectDeucesWildCommand = new RelayCommand(_ => _onDeucesWildSelected());
            QuitCommand = new RelayCommand(_ => _onQuit());
        }
    }
}
