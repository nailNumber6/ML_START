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
            serverWindow.Activated += ServerWindow_Activated;
        }

        private void ServerWindow_Activated(object? sender, System.EventArgs e)
        {
            MainWindowViewModel vm = new();

            Task.Run(async () => 
            {
                await vm.StartAndShowStory(list);
            });

            Task.Run(vm.StartServer);
        }
    }
}