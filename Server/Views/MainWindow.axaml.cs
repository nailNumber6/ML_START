using Avalonia.Controls;

using Server.ViewModels;

namespace Server.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
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