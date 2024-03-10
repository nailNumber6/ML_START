using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Threading;

using MLSTART_GUI.ViewModels;


namespace MLSTART_GUI.Views;
public partial class ClientWindow : Window
{
    public ClientWindow()
    {
        InitializeComponent();
        Closing += ClientWindow_Closing;
    }

    private void ClientWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (allowWindowClosing.IsChecked == false)
        {
            e.Cancel = true;
        }
        else
        {
            e.Cancel = false;
        }
    }
}
