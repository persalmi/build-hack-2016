using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SnapFeud.UWP.Views;

namespace SnapFeud.UWP.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public StartViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public string Title => "Snap Feud";

        private RelayCommand _startCommand;
        public ICommand StartCommand => _startCommand ?? (_startCommand = new RelayCommand(Start));

        private void Start()
        {

        }

        private RelayCommand _exitCommand;
        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new RelayCommand(Exit));

        private void Exit()
        {
            Application.Current.Exit();
        }

        public object DebugVisibility
        {
            get
            {
#if DEBUG
                return Visibility.Visible;
#else
                return Visibility.Collapsed;
#endif
            }
        }

        private RelayCommand _debugCommand;
        public ICommand DebugCommand => _debugCommand ?? (_debugCommand = new RelayCommand(Debug));

        private void Debug()
        {
            _navigationService.Navigate(typeof(TestPage));
        }
    }
}
