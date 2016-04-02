using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SnapFeud.UWP.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public GameViewModel(INavigationService navigationService, SnapFeudContext context)
        {
            _navigationService = navigationService;
            Context = context;
        }

        public SnapFeudContext Context { get; }

        private RelayCommand _photoCommand;
        public ICommand PhotoCommand => _photoCommand ?? (_photoCommand = new RelayCommand(TakePhoto));

        private void TakePhoto()
        {
            // TODO
        }
    }
}
