using Avalonia.Controls;
using Avalonia.Media;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{
    public ClientWindow()
    {
        InitializeComponent();
        Closing += ClientWindow_Closing;
        Loaded += ClientWindow_Loaded;
        connectButton.Click += ConnectButton_Click;
    }

    private void ConnectButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (isAuthorized.IsChecked == false)
        {
            var dialog = new MainWindow { DataContext = ThisWindow.DataContext };
            dialog.ShowDialog<string>(this);
        }
    }

    private void ClientWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        username.PropertyChanged += Username_PropertyChanged;
    }

    // Green foreground if user is  authorized
    private void Username_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (username.Foreground!.Equals(Brushes.Red))
        {
            username.Foreground = Brushes.Green;
            usernameTitle.IsVisible = true;
            username.PropertyChanged -= Username_PropertyChanged;
        }
    }

    private void ClientWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (allowWindowClosing.IsChecked == false)
        {
            e.Cancel = true;
        }
        else
        {
            e.Cancel = false;
        }
    }
}
