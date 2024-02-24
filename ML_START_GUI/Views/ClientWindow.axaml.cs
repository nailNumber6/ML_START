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

        connectButton.Click += ConnectButton_Click;
    }

    private void ConnectButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = new ClientWindowViewModel();
        Task.Run(vm.ConnectServer);
    }
}
