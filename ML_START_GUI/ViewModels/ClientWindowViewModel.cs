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

// ClientWindowViewModel for interaction with ClientWindow
internal partial class ClientWindowViewModel : ObservableObject
{
    private IPAddress _serverIp;
    private int _serverPort;

    public ClientWindowViewModel()
    {
        #region Server address reading from the config file
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
        #endregion

        IsAuthorized = false;
        _networkMessages = [];
    }

    #region Observable properties for ClientWindow
    private string _username = "Гость";

    [ObservableProperty]
    private bool _isWindowClosingAllowed;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DisconnectOnButtonCommand))]
    private TcpClient? _currentClient;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]

    private string? _input;
    private ObservableCollection<string> _networkMessages;

    public ObservableCollection<string> NetworkMessages
    {
        get => _networkMessages;
        set => SetProperty(ref _networkMessages, value);
    }
    #endregion

    /// <summary>
    /// Проверяет, что клиент существует и подключен
    /// </summary>
    public bool ClientIsConnected
    {
        get { return CurrentClient != null && CurrentClient.Connected; }  
    }

    public string? ConnectionStateText
    {
        get { return ClientIsConnected ? "подключен" : "отключен"; }
    }

    public string Username 
    {
        get => _username;
        set => _username = value;
    }

    public bool IsAuthorized { get; set; }

    #region methods for bindings

    
    #region network interaction methods
    [RelayCommand]
    public async Task ConnectServer()
    {
        if (IsAuthorized == false)
        {
            var authorizationWindow = new MainWindow
            {
                DataContext = this,
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
                    Log.Information("Создан клиент");
                }

                var connectionParameters = Program.Configuration.GetSection("Other parameters");

                #region client connection to the server
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    try
                    {
                        CurrentClient.Connect(_serverIp, _serverPort);
                        OnPropertyChanged(nameof(ConnectionStateText));

                        Log.Information("Клиент с адресом {clientAddress} подключился к серверу", CurrentClient.Client.LocalEndPoint!);
                        NetworkMessages.Add($"Подключен к серверу {_serverIp} : {_serverPort}");
                    }
                    catch (SocketException ex)
                    {
                        Log.Error("Попытка подключения клиента с адресом {clientAddress} вызвала исключение {exType} : {exMessage}",
                            CurrentClient.Client.RemoteEndPoint, ex.GetType(), ex.Message);

                        NetworkMessages.Add("Ошибка: Сервер не принимает подключения");
                    }
                });
                #endregion
            }
        }
    }
    private void DisconnectFromServer()
    {
        string clientAddressString = CurrentClient!.Client.LocalEndPoint!.ToString()!;

        CurrentClient!.Client.Shutdown(SocketShutdown.Both);
        Log.Information("Клиент с адресом {clientAddress} теперь не может принимать и посылать сообщения на сервер", clientAddressString);

        CurrentClient!.Close();
        CurrentClient = null;
        Log.Information("Клиент {clientAddress} закрыт", clientAddressString);

        NetworkMessages.Add("Отключен от сервера");
    }

    [RelayCommand(CanExecute = nameof(InputNotEmpty))]
    public async Task Send()
     {
        if (ClientIsConnected)
        {
            NetworkStream tcpStream = CurrentClient!.GetStream();
            byte[] encodedMessage = Encoding.UTF8.GetBytes("message" + " " + Input!);

            await tcpStream.WriteAsync(encodedMessage);

            NetworkMessages.Add($"На сервер отправлено сообщение: {Input}");
            Log.Information("Клиент {clientAddress} отправил на сервер сообщение: {message}",
                CurrentClient.Client.LocalEndPoint, Input);

            Input = string.Empty;

            #region response from the server
            int readTotal;
            byte[] buffer = new byte[1024];

            while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
            {
                string response = Encoding.UTF8.GetString(buffer, 0, readTotal);

                NetworkMessages.Add("Ответ сервера: " + response);
                Log.Information("Получен ответ от сервера: {response}", response);
            }
            #endregion
        }
        else
        {
            new MessageBox("Клиент не подключен", "Клиент", MessageBoxIcon.Warning).Show();
        }
    }
    private bool InputNotEmpty() => !string.IsNullOrEmpty(Input) && ClientIsConnected;

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
                "\nЗакрыть подключение?" +
                "\nПосле нажатия ОК нужно повторно нажать кнопку закрытия", 
                ButtonEnum.OkCancel, Icon.Warning)
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
