using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomMessageBox.Avalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

using MLSTART_GUI.Views;
using ToolLibrary;


namespace MLSTART_GUI.ViewModels;
internal partial class ClientWindowViewModel : ObservableObject
{
    [ObservableProperty]
    //[NotifyCanExecuteChangedFor(nameof(CloseConnectionCommand))]
    private TcpClient? _client;

    [ObservableProperty]
    private string? _ip = "127.0.0.1";

    [ObservableProperty]
    private int _port = 8080;

    [ObservableProperty]
    private string? _input;

    private string _username = "Гость";

    public ClientWindowViewModel()
    {
        IsAuthorized = false;
    }

    public bool ExistsAndConnected => Client != null && Client.Connected;
    public bool IsAuthorized { get; set; }
    public string Username 
    {
        get => _username;
        set => _username = value;
    }


    [RelayCommand]
    public async Task ConnectServer()
    {
        if (IsAuthorized == false)
        {
            var authorizationWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel
                {
                    SourceWindowViewModel = this
                },
            };
            authorizationWindow.Show();
        }

        else
        {
            if (ExistsAndConnected)
            {
                new MessageBox("Клиент уже подключен к серверу", "Клиент", MessageBoxIcon.Information).Show();
            }
            else
            {
                Client ??= new();

                try
                {
                    Task t = Client.ConnectAsync(IPAddress.Parse(Ip!), Port).ContinueWith(t =>
                    {
                        if (t.IsFaulted) new MessageBox("a", "a").Show();
                        else t.Start();
                    });


                    await LoggingTool.LogByTemplateAsync(
                            Serilog.Events.LogEventLevel.Information, note: "Пользователь подключился");
                }
                // TODO: Обработать исключение
                catch (SocketException)
                {

                }
            }
        }
    }

    [RelayCommand]
    public async Task Send()
    {
        if (ExistsAndConnected)
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

                new MessageBox(response, "Сообщение от сервера", MessageBoxIcon.Information).Show();
                break;
            }
            #endregion
        }

        else
        {
            new MessageBox("Клиент не подключен", "Клиент", MessageBoxIcon.Warning).Show();
        }
    }

    [RelayCommand]
    public void DisconnectOnButton()
    {

    }

    public async Task<bool> IsConnectionClosed()
    {
        var dialogResult = await MessageBoxManager.
                GetMessageBoxStandard("Клиент", "В данный момент клиент подключен к серверу" +
                "\nЗакрыть подключение?", ButtonEnum.OkCancel)
                .ShowAsync();

        if (dialogResult == ButtonResult.Ok)
        {
            if (ExistsAndConnected)
            {
                Client!.Client.Shutdown(SocketShutdown.Both);
                Client!.Close();

                Client.Dispose();
                Client = null;

                return true;
            }
            else return true;
        }
        return false;
    }
}
