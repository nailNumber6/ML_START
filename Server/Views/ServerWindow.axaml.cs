using System.Threading.Tasks;
using Avalonia.Controls;

using Server.ViewModels;


namespace Server.Views
{
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
            serverWindow.Loaded += ServerWindow_Loaded;
            serverWindow.Closed += ServerWindow_Closed;
        }

        private void ServerWindow_Closed(object? sender, System.EventArgs e)
        {
            // TODO: Закрытие подключения сервера
            
        }

        private void ServerWindow_Loaded(object? sender, System.EventArgs e)
        {
            MainWindowViewModel vm = new();

            Task.Run(async () => await vm.StartAndShowStory(list));

            Task.Run(async () => await vm.StartServer(clientList));
        }
    }
}