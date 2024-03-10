using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomMessageBox.Avalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Serilog;

using MLSTART_GUI.Views;
using System;


namespace MLSTART_GUI.ViewModels;
public enum ConnectionStateEnum { Отключен = 0, Подключен = 1 }

internal partial class ClientWindowViewModel : ObservableObject
{
    private IPAddress _serverIp;
    private int _serverPort;

    public ClientWindowViewModel()
    {
        try
        {
            var connectionParameters = Program.Configuration.GetSection("Connection parameters");
            _serverIp = IPAddress.Parse(connectionParameters["Server IP"]!);
            _serverPort = int.Parse(connectionParameters["Server port"]!);
        }
        catch (Exception ex)
        {
            Log.Error("Источник: {thisProject}.При попытке прочитать значения для подключения из файла {config} произошла ошибка {exType} : {exMessage}",
                ex.Source,Program.configFileName, ex.GetType(), ex.Message);
        }

        IsAuthorized = false;
        _networkMessages = [];
    }

    #region Observable properties
    private string _username = "Гость";
    public string Username 
    {
        get => _username;
        set => _username = value;
    }

    [ObservableProperty]
    private bool _isWindowClosingAllowed;

    private ObservableCollection<string> _networkMessages;

    [ObservableProperty]
    private ConnectionStateEnum _connectionState = ConnectionStateEnum.Отключен;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DisconnectOnButtonCommand))]
    private TcpClient? _currentClient;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string? _input;

    public ObservableCollection<string> NetworkMessages
    {
        get => _networkMessages;
        set => SetProperty(ref _networkMessages, value);
    }
    #endregion

    public bool IsAuthorized { get; set; }
    public bool ClientIsConnected { get => ConnectionState == ConnectionStateEnum.Подключен; }

    private void DisconnectFromServer()
    {
        CurrentClient!.Client.Shutdown(SocketShutdown.Both);
        Log.Information("Клиент с адресом {clientAddress} теперь не может принимать и посылать сообщения на сервер", CurrentClient.Client.RemoteEndPoint);

        CurrentClient!.Close();
        CurrentClient = null;
        Log.Information("Клиент закрыт");

        NetworkMessages.Add("Отключен от сервера");
        ConnectionState = ConnectionStateEnum.Отключен;
    }

    #region methods for bindings

    #region network interaction methods
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
            if (ClientIsConnected)
            {
                new MessageBox("Клиент уже подключен к серверу", "Клиент", MessageBoxIcon.Information).Show();
            }
            else
            {
                if (CurrentClient == null)
                {
                    CurrentClient = new TcpClient();
                    Log.Information("Создан клиент (класс TcpClient)");
                }

                var connectionParameters = Program.Configuration.GetSection("Other parameters");
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        CurrentClient.Connect(_serverIp, _serverPort);
                        ConnectionState = ConnectionStateEnum.Подключен;

                        Log.Information("Клиент с адресом {clientAddress} подключился к серверу", CurrentClient.Client.LocalEndPoint!);
                        NetworkMessages.Add("Подключен к серверу");
                    }
                    catch (SocketException ex)
                    {
                        Log.Error("Попытка подключения клиента с адресом {clientAddress} вызвала исключение {exType} : {exMessage}",
                            CurrentClient.Client.RemoteEndPoint, ex.GetType(), ex.Message);

                        NetworkMessages.Add("Ошибка: Сервер не принимает подключения");
                    }
                });
            }
        }
    }

    [RelayCommand(CanExecute = nameof(InputNotEmpty))]
    public async Task Send()
    {
        if (ClientIsConnected)
        {
            NetworkStream tcpStream = CurrentClient!.GetStream();
            byte[] encodedMessage = Encoding.UTF8.GetBytes(Input ?? "пустая строка");

            await tcpStream.WriteAsync(encodedMessage);

            #region response from the server
            byte[] buffer = new byte[256];
            int readTotal;

            while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
            {
                string response = Encoding.UTF8.GetString(buffer, 0, readTotal);

                NetworkMessages.Add("Ответ сервера: " + response);
                break;
            }
            #endregion
        }

        else
        {
            new MessageBox("Клиент не подключен", "Клиент", MessageBoxIcon.Warning).Show();
        }
    }
    private bool InputNotEmpty() => !string.IsNullOrEmpty(Input) && ConnectionState == ConnectionStateEnum.Подключен;

    [RelayCommand]
    public async Task DisconnectOnButton()
    {
        if (ClientIsConnected)
        {
            DisconnectFromServer();
        }
        else
        {
            await MessageBoxManager
                .GetMessageBoxStandard("Клиент", 
                "Клиент не подключен к серверу",
                ButtonEnum.Ok, Icon.Info)
                .ShowAsync();
        }
    }
    #endregion

    /// <summary>
    /// При попытке закрытия окна, если клиент покдлючен в серверу, вызывает диалоговое окно. 
    /// В нем спрашивается согласен ли пользователь закрыть программу и отключиться от сервера.
    /// </summary>
    public void AskForWindowClosing()
    {
        if (ClientIsConnected)
        {
            var dialogResult = MessageBoxManager
                .GetMessageBoxStandard("Клиент",
                "В данный момент клиент подключен к серверу" +
                "\nЗакрыть подключение?", ButtonEnum.OkCancel, Icon.Warning)
                .ShowAsync().ContinueWith(async task =>
                {
                    if (task.Result == ButtonResult.Ok)
                    {
                        await Dispatcher.UIThread.InvokeAsync(DisconnectFromServer);
                        IsWindowClosingAllowed = true;
                    }
                    else
                    {
                        IsWindowClosingAllowed = false;
                        await Task.CompletedTask;
                    }
                });
        }
        else IsWindowClosingAllowed = true;
    }
    #endregion
}
