﻿using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

            await Client.ConnectAsync(IPAddress.Parse(Ip!), Port);
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