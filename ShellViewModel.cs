using System.Windows.Input;

namespace PokerGame
{
    public class ShellViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private readonly System.Action _onExitGame;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetField(ref _currentViewModel, value);
        }

        public ICommand ReturnToMenuCommand { get; }

        public ShellViewModel(ViewModelBase initialViewModel, System.Action onExitGame = null)
        {
            CurrentViewModel = initialViewModel;
            _onExitGame = onExitGame;
            ReturnToMenuCommand = new RelayCommand(_ => ReturnToMenu());
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private void ReturnToMenu()
        {
            // This assumes the Shell is managed by MainWindow which knows how to recreate the Menu
            // Or we can have a callback.
            // For now, let's just expose NavigateTo.
            _onExitGame?.Invoke();
        }
    }
}
