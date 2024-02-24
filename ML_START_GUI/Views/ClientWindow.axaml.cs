using Avalonia.Controls;
using MLSTART_GUI.ViewModels;
using System.Threading.Tasks;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{ 
    public ClientWindow(string? username)
    {
        InitializeComponent();
        this.username.Content = username;
        clientWindow.Closed += ClientWindow_Closed;
    }

    private void ClientWindow_Closed(object? sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }
}
