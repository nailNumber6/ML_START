using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace Server.Models.Network;

/// <summary>
/// Обертка над TcpListener, которая позволяет получить свойство Active
/// </summary>
public class TcpServer : TcpListener
{
    /// <summary>
    /// Конструктор, совпадающий с базовам конструктором TcpListener
    /// </summary>
    public TcpServer(IPAddress localaddr, int port) : base(localaddr, port)
    {
    }

    /// <summary>
    /// Указывет на то, принимает ли сервер подключения
    /// </summary>
    /// <value>
    /// значение типа bool, true - сервер принимает подключения, false - не принимает
    /// </value>
    public new bool Active
    {
        get { return base.Active; }
    }
}
