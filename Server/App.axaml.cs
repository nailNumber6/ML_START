using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Server.ViewModels;
using Server.Views;

namespace Server
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
                desktop.MainWindow = new ServerWindow
                {
                    DataContext = new ServerWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}