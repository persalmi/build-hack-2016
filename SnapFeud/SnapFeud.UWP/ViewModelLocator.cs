using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using SnapFeud.UWP.ViewModels;

namespace SnapFeud.UWP
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<INavigationService>(() => new NavigationService());

            SimpleIoc.Default.Register<SnapFeudContext>();
            SimpleIoc.Default.Register<StartViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();
        }

        public SnapFeudContext Context => SimpleIoc.Default.GetInstance<SnapFeudContext>();

        public StartViewModel StartViewModel => SimpleIoc.Default.GetInstance<StartViewModel>();

        public GameViewModel GameViewModel => SimpleIoc.Default.GetInstance<GameViewModel>();
    }
}
