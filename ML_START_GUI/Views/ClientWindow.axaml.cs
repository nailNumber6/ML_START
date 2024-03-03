using Avalonia.Controls;
using Avalonia.Threading;
using MLSTART_GUI.ViewModels;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{ 
    public ClientWindow()
    {
        InitializeComponent();
        clientWindow.Closing += ClientWindow_Closing;
    }

    private void ClientWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() => new ClientWindowViewModel().Disconnect());
        
    }
}
