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
    }

    private void ClientWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        username.PropertyChanged += Username_PropertyChanged;
    }

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
