using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomMessageBox.Avalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

using MLSTART_GUI.Views;
using ToolLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace MLSTART_GUI.ViewModels;
public enum ConnectionStateEnum { Отключен = 0, Подключен = 1 }

internal partial class ClientWindowViewModel : ObservableObject
{
    public ClientWindowViewModel()
    {
        IsAuthorized = false;
        _networkMessages = [];
    }

    private string _username = "Гость";

    private ObservableCollection<string> _networkMessages;

    [ObservableProperty]
    private ConnectionStateEnum _connectionState = ConnectionStateEnum.Отключен;

    [ObservableProperty]
    private string? _ip = "127.0.0.1";

    [ObservableProperty]
    private int _port = 8080;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DisconnectOnButtonCommand))]
    private TcpClient? _client;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string? _input;

    public ObservableCollection<string> NetworkMessages
    {
        get => _networkMessages;
        set => SetProperty(ref _networkMessages, value);
    }
    public bool IsAuthorized { get; set; }
    public bool ClientIsConnected { get => ConnectionState == ConnectionStateEnum.Подключен; }
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
            if (ClientIsConnected)
            {
                new MessageBox("Клиент уже подключен к серверу", "Клиент", MessageBoxIcon.Information).Show();
            }
            else
            {
                Client ??= new();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        Client.Connect(IPAddress.Parse(Ip!), Port);
                        ConnectionState = ConnectionStateEnum.Подключен;

                        NetworkMessages.Add("Покдлючен к серверу");
                        LoggingTool.LogByTemplate(Serilog.Events.LogEventLevel.Information, note: "Успешное подключение к серверу");
                    }
                    catch (SocketException ex)
                    {
                        LoggingTool.LogByTemplate(Serilog.Events.LogEventLevel.Error,
                            ex, "Клиент принял попытку подключения к отключенному сереверу");

                        NetworkMessages.Add("Ошибка: Сервер не принимает подключения");
                    }
                });
            }
        }
    }

    [RelayCommand]
    public async Task DisconnectOnButton()
    {
        if (ClientIsConnected)
        {
            DisconnectClient();
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

    public async Task<bool> IsClientDisconnectionAccepted()
    {
        if (ClientIsConnected)
        {
            var dialogResult = await MessageBoxManager
                .GetMessageBoxStandard("Клиент",
                "В данный момент клиент подключен к серверу" +
                "\nЗакрыть подключение?", ButtonEnum.OkCancel, Icon.Warning)
                .ShowAsync().ContinueWith(async task =>
                {
                    if (task.Result == ButtonResult.Ok)
                    {
                        Dispatcher.UIThread.Invoke(DisconnectClient);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            if (dialogResult.Result == false) return false;
            else return true;
        }
        return true;
    }

    private void DisconnectClient()
    {
        Client!.Client.Shutdown(SocketShutdown.Both);
        Client!.Close();

        NetworkMessages.Add("Отключен от сервера");

        Client.Dispose();
        Client = null;
        ConnectionState = ConnectionStateEnum.Отключен;
    }

    private bool InputNotEmpty() => !string.IsNullOrEmpty(Input) && ConnectionState == ConnectionStateEnum.Подключен;

    [RelayCommand(CanExecute = nameof(InputNotEmpty))]
    public async Task Send()
    {
        if (ClientIsConnected)
        {
            NetworkStream tcpStream = Client!.GetStream();
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
}
