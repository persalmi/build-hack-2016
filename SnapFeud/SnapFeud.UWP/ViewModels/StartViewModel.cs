using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using SnapFeud.UWP.Views;

namespace SnapFeud.UWP.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public StartViewModel(INavigationService navigationService, SnapFeudContext context)
        {
            _navigationService = navigationService;
            Context = context;
        }

        public SnapFeudContext Context { get; }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { Set(ref _userName, value); }
        }

        private RelayCommand _startCommand;
        public ICommand StartCommand => _startCommand ?? (_startCommand = new RelayCommand(Start));

        private async void Start()
        {
            var context = SimpleIoc.Default.GetInstance<SnapFeudContext>();
            //var proxy = new GameProxy();
            //var game = await proxy.CreateGame(UserName);
            //context.CurrentGame = game;
            _navigationService.Navigate(typeof(GamePage));
        }

        private RelayCommand _leaderBoardCommand;
        public ICommand LeaderBoardCommand => _leaderBoardCommand ?? (_leaderBoardCommand = new RelayCommand(LeaderBoard));

        private void LeaderBoard()
        {
            // TODO
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
