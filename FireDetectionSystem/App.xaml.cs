using FireDetectionSystem.ViewModels;
using FireDetectionSystem.Views;
using Prism.Ioc;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FireDetectionSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Views.MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LoginView,LoginViewModel>();
        }

        protected override void OnInitialized()
        {
           var dialog=Container.Resolve<IDialogService>();

            dialog.ShowDialog("LoginView", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    base.OnInitialized();
                }
                else
                {
                    Environment.Exit(0);
                    Current.Shutdown();
                }
            });

            
        }
    }

}
