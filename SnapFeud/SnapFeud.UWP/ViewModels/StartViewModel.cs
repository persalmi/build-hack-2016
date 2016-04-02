using System.Windows.Input;
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

        private RelayCommand _debugCommand;
        public ICommand DebugCommand => _debugCommand ?? (_debugCommand = new RelayCommand(Debug));

        private void Debug()
        {
            _navigationService.Navigate(typeof(TestPage));
        }
    }
}
