using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using SnapFeud.UWP.ViewModels;

namespace SnapFeud.UWP {
    public class ViewModelLocator {
        public ViewModelLocator() {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<StartViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();
        }

        public StartViewModel StartViewModel => SimpleIoc.Default.GetInstance<StartViewModel>();

        public GameViewModel GameViewModel => SimpleIoc.Default.GetInstance<GameViewModel>();
    }
}
