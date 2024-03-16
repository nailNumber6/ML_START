﻿using System.Net;
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

    #region observable properties for MainWindow (Authorization)
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _loginInput;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _passwordInput;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _repeatPasswordInput;
    #endregion

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

    #region methods for MainWindow
    public bool CanValidate() =>
        !string.IsNullOrEmpty(LoginInput) &&
        !string.IsNullOrEmpty(PasswordInput) &&
        !string.IsNullOrEmpty(RepeatPasswordInput);

    [RelayCommand(CanExecute = nameof(CanValidate))]
    public async Task LogUserIn()
    {
        string authorizationString;

        if (PasswordInput == RepeatPasswordInput)
        {
            if (LoginInput!.Length <= 20)
            {
                // authorization string sending to the server
                authorizationString = $"login {LoginInput} {PasswordInput}";

                CurrentClient = new();
                CurrentClient.Connect(_serverIp, _serverPort);

                using var tcpStream = CurrentClient.GetStream();

                byte[] encodedMessage = Encoding.UTF8.GetBytes(authorizationString);
                await tcpStream.WriteAsync(encodedMessage);

                Log.Information("Клиент {clientAddress} отправил на сервер данные для авторизации: {authString}",
                    CurrentClient.Client.LocalEndPoint, authorizationString);

                byte[] buffer = new byte[1024];
                int readTotal;
                string response = string.Empty;

                while (CurrentClient != null && (readTotal = await tcpStream.ReadAsync(buffer)) != 0)
                {
                    response = Encoding.UTF8.GetString(buffer, 0, readTotal);
                    Log.Information("Ответ сервера: {response}", response);

                    if (response == "success")
                    {
                        Username = LoginInput;
                        IsAuthorized = true;

                        OnPropertyChanged(nameof(ConnectionStateText));
                        OnPropertyChanged(nameof(Username));

                        Log.Information("Пользователь {username} успешно вошел в систему", Username);
                        new MessageBox("Вы успешно вошли!\n Окно авторизации можно закрыть", "Успех", MessageBoxIcon.Information).Show();
                    }
                    else
                    {
                        new MessageBox("Вы не вошли в систему", "Отказ в доступе", MessageBoxIcon.Information).Show();
                        Log.Information("Клиент {clientAddress} не смог войти в систему", CurrentClient.Client.LocalEndPoint);
                        CurrentClient.Client.Close();
                        CurrentClient = null;
                    }
                }
            }
        }
        else
        {
            new MessageBox("Пароли не совпадают", "Ошибка", MessageBoxIcon.Warning).Show();
        }
    }

    [RelayCommand(CanExecute = nameof(CanValidate))]
    public async Task RegisterUser()
    {
        string authorizationString;

        if (PasswordInput == RepeatPasswordInput)
        {
            if (LoginInput!.Length <= 20)
            {
                // authorization string sending to the server
                authorizationString = $"register {LoginInput} {PasswordInput}";

                CurrentClient = new();
                CurrentClient.Connect(_serverIp, _serverPort);

                using var tcpStream = CurrentClient.GetStream();

                byte[] encodedMessage = Encoding.UTF8.GetBytes(authorizationString);
                await tcpStream.WriteAsync(encodedMessage);

                Log.Information("Клиент {clientAddress} отправил на сервер данные для авторизации: {authString}",
                    CurrentClient.Client.LocalEndPoint, authorizationString);

                byte[] buffer = new byte[1024];
                int readTotal;
                string response = string.Empty;

                while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
                {
                    response = Encoding.UTF8.GetString(buffer, 0, readTotal);
                    Log.Information("Ответ сервера: {response}", response);

                    if (response == "success")
                    {
                        Username = LoginInput;
                        IsAuthorized = true;

                        OnPropertyChanged(nameof(ConnectionStateText));
                        OnPropertyChanged(nameof(Username));

                        Log.Information("Пользователь {username} успешно зарегистрирован", Username);
                        new MessageBox("Вы успешно зарегистрировались!\n Окно авторизации можно закрыть", "Успех", MessageBoxIcon.Information).Show();
                    }
                    else if (response == "exists")
                    {
                        new MessageBox("Пользователь с такими данными уже существует", "Отказ в доступе", MessageBoxIcon.Information).Show();
                        Log.Information("Клиент {clientAddress} не зарегистрирован", CurrentClient.Client.LocalEndPoint);
                        CurrentClient.Close();
                        CurrentClient = null;
                    }
                }
            }
        }
        else
        {
            new MessageBox("Пароли не совпадают", "Ошибка", MessageBoxIcon.Warning).Show();
        }
    }

    [RelayCommand]
    public void ResetInputFields()
    {
        LoginInput = string.Empty;
        PasswordInput = string.Empty;
        RepeatPasswordInput = string.Empty;
    }
    #endregion

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
                        NetworkMessages.Add("Подключен к серверу");
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
            using NetworkStream tcpStream = CurrentClient!.GetStream();
            byte[] encodedMessage = Encoding.UTF8.GetBytes("message" + " " + Input!);

            await tcpStream.WriteAsync(encodedMessage);

            NetworkMessages.Add($"На сервер отправлено сообщение: {Input}");
            Log.Information("Клиент {clientAddress} отправил на сервер сообщение: {message}",
                CurrentClient.Client.LocalEndPoint, Input);

            Input = string.Empty;

            #region response from the server
            byte[] buffer = new byte[1024];
            int readTotal;

            while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
            {
                string response = Encoding.UTF8.GetString(buffer, 0, readTotal);

                NetworkMessages.Add("Ответ сервера: " + response);
                Log.Information("Получен ответ от сервера: {response}", response);
                break;
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
