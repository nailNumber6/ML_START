using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Threading;

using MLSTART_GUI.ViewModels;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{
    public ClientWindow()
    {
        InitializeComponent();
        Closing += ClientWindow_Closing;
    }

    private async void ClientWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        ClientWindowViewModel vm = (ClientWindowViewModel)DataContext!;

        await Dispatcher.UIThread.InvokeAsync(vm.IsClientDisconnectionAccepted, DispatcherPriority.MaxValue)
            .ContinueWith(t =>
            {
                if (t.Result == true)
                {
                    e.Cancel = false;
                    clientWindow.Closing -= ClientWindow_Closing;
                    Dispatcher.UIThread.Invoke(Close);
                }
                else
                {
                    e.Cancel = true;
                }
            });

        await Task.CompletedTask;
    }
}
