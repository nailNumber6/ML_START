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


namespace MLSTART_GUI.ViewModels;
internal partial class ClientWindowViewModel : ObservableObject
{
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

    private string _username = "Гость";

    public ClientWindowViewModel()
    {
        IsAuthorized = false;
    }

    public enum ConnectionStateEnum { Отключен = 0, Подключен = 1 }


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

                try
                {
                    await Client.ConnectAsync(IPAddress.Parse(Ip!), Port);

                    ConnectionState = ConnectionStateEnum.Подключен;

                    LoggingTool.LogByTemplate(
                            Serilog.Events.LogEventLevel.Information, note: "Успешное подключение к серверу");
                }
                catch (SocketException ex)
                {
                    LoggingTool.LogByTemplate(Serilog.Events.LogEventLevel.Error, 
                        ex, "Клиент принял попытку подключения к отключенному сереверу");
                }
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
}
