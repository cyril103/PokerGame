using System.Windows.Input;

namespace PokerGame
{
    public class GameSelectionViewModel : ViewModelBase
    {
        private readonly System.Action _onJacksOrBetterSelected;
        private readonly System.Action _onDeucesWildSelected;

        public ICommand SelectJacksOrBetterCommand { get; }
        public ICommand SelectDeucesWildCommand { get; }

        public GameSelectionViewModel(System.Action onJacksOrBetterSelected, System.Action onDeucesWildSelected)
        {
            _onJacksOrBetterSelected = onJacksOrBetterSelected;
            _onDeucesWildSelected = onDeucesWildSelected;

            SelectJacksOrBetterCommand = new RelayCommand(_ => _onJacksOrBetterSelected());
            SelectDeucesWildCommand = new RelayCommand(_ => _onDeucesWildSelected());
        }
    }
}
