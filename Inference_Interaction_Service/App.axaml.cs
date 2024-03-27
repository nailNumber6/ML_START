using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Inference_Interaction_Service.ViewModels;
using Inference_Interaction_Service.Views;

namespace Inference_Interaction_Service
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new HttpClientWindow
                {
                    DataContext = new HttpClientViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}