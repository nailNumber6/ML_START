using System;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace MLSTART_GUI.Models;
internal static class TcpClientExtensions
{
    /// <summary>
    /// Осуществляет попытку подключения клиента, возвращает результат об успешности попытки
    /// <para>Логирует исключение при провальной попытке</para>
    /// </summary>
    /// <param name="client">Клиент</param>
    /// <param name="ip">IP-адрес подключения</param>
    /// <param name="port">Порт подключения</param>
    /// <returns>true - если клиент покдлючился успешно, если нет - false</returns>
    internal static bool TryConnect(this TcpClient client, IPAddress ip, int port)
    {
        try
        {
            client.Connect(ip, port);
            Log.Information("Клиент с адресом {clientAddress} подключился к серверу {ip} : {port}",
            client.Client.LocalEndPoint!, ip, port);
        }
        catch (Exception ex)
        {
            Log.Error("Попытка подключения клиента с адресом {clientAddress} к серверу {ip} : {port} вызвала исключение {exType} : {exMessage}",
                            client.Client.RemoteEndPoint, ip, port,
                            ex.GetType(), ex.Message);
            return false;
        }
        return true;
    }
}
