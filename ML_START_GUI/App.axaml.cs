using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MLSTART_GUI.ViewModels.InferenceInteraction;
using MLSTART_GUI.Views;

namespace MLSTART_GUI
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