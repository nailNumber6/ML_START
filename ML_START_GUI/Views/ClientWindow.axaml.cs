using Avalonia.Controls;
using MLSTART_GUI.ViewModels;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{ 
    public ClientWindow()
    {
        InitializeComponent();
        clientWindow.Closed += ClientWindow_Closed;
    }

    private void ClientWindow_Closed(object? sender, System.EventArgs e)
    {
        var vm = new ClientWindowViewModel();
        vm.CloseConnection();
    }
}
