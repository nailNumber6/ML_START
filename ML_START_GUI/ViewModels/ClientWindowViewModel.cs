using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MLSTART_GUI.ViewModels;

internal partial class ClientWindowViewModel : ObservableObject
{
    // TODO: Окно клиента
    [ObservableProperty]
    private string _ip = "127.0.0.1";

    [ObservableProperty]
    private int _port = 8070;

    public async Task ConnectServer()
    {
        IPEndPoint endPoint = new(IPAddress.Parse(Ip), Port);
        using TcpClient tcpClient = new(endPoint);

        await tcpClient.ConnectAsync("127.0.0.1", 8080);
        var stream = tcpClient.GetStream();
    }

    private void CloseConnection(TcpClient client)
    {
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    } 
}
