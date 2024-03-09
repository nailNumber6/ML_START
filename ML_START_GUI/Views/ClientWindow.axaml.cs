using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Threading;

using MLSTART_GUI.ViewModels;
using ToolLibrary;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{
    public ClientWindow()
    {
        InitializeComponent(); // Invalid Cast ex
        Closing += ClientWindow_Closing;
    }

    private async void ClientWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        ClientWindowViewModel vm = (ClientWindowViewModel)DataContext!;

        await Dispatcher.UIThread.InvokeAsync(vm.HandleClientDisconnection, DispatcherPriority.MaxValue)
            .ContinueWith(t =>
            {
                if (t.Result == true)
                {
                    e.Cancel = false;
                    clientWindow.Closing -= ClientWindow_Closing;
                    Dispatcher.UIThread.Invoke(Close);

                    LoggingTool
                    .LogByTemplate(Serilog.Events.LogEventLevel.Information,
                        note: "������ ��� �������� � ���������� �������� ����");
                }
                else
                {
                    e.Cancel = true;
                }
            });

        await Task.CompletedTask;
    }
}
