using System.Net.Sockets;
using System.Threading.Tasks;

namespace MLSTART_GUI.ViewModels;

internal class ClientWindowViewModel
{
    // TODO: Окно клиента
    public async Task ConnectServer()
    {
        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync("127.0.0.1", 8080);
        var stream = tcpClient.GetStream();
    }
}
