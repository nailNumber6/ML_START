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
            serverWindow.KeyDown += ServerWindow_KeyDown;
        }

        private void ServerWindow_Activated(object? sender, System.EventArgs e)
        {
            MainWindowViewModel.StartAndShowStory(list);
        }

        private void ServerWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                var tb = new TextBlock
                {
                    Text = "I'm rookie"
                };

                list.Items.Add(tb);
            }
        }
    }
}