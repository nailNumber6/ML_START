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
        }

        private void ServerWindow_Loaded(object? sender, System.EventArgs e)
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