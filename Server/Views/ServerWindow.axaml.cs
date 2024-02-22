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
            MainWindowViewModel.StartAndShowStory(list);
        }
    }
}