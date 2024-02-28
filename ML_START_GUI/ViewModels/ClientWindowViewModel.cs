using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LoggingLibrary;

namespace MLSTART_GUI.ViewModels;

internal partial class ClientWindowViewModel : ObservableObject
{
    // TODO: Окно клиента
    [ObservableProperty]
    private string? _ip = "127.0.0.1";

    [ObservableProperty]
    private int _port = 8080;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CloseConnectionCommand))]
    private TcpClient? _client;

    [ObservableProperty]
    private string? _input;


    [RelayCommand]
    public async Task ConnectServer()
    {
        if (Client != null && Client.Connected)
        {
            Debug.WriteLine("Клиент уже подключен к серверу");
        }
        else
        {
            Client ??= new();

            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                    Client
                    .ConnectAsync(IPAddress.Parse(Ip!), Port));

                await LoggingTool.LogByTemplateAsync(
                        Serilog.Events.LogEventLevel.Information, note:"Пользователь подключился");
            }
            // TODO: Обработать исключение
            catch (SocketException)
            {

            }
        }
    }

    [RelayCommand]
    public async Task Send()
    {
        if (Client != null && Client.Connected)
        {
            NetworkStream tcpStream = Client.GetStream();
            byte[] encodedMessage = Encoding.UTF8.GetBytes(Input ?? "пустая строка");

            await tcpStream.WriteAsync(encodedMessage);

            #region response from the server
            byte[] buffer = new byte[256];
            int readTotal;

            while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
            {
                string response = Encoding.UTF8.GetString(buffer, 0, readTotal);

                Debug.WriteLine(response);
            }
            #endregion
        }

        else
        {
            Debug.WriteLine("Клиент не подключен");
        }
    }

    [RelayCommand]
    public void CloseConnection()
    {
        if (Client != null && Client.Connected)
        {
            Client!.Client.Shutdown(SocketShutdown.Both);
            Client!.Close();

            Client.Dispose();
            Client = null;
        }
        else
        {
            Debug.WriteLine("Клиент не подключен");
        }
    }
}
