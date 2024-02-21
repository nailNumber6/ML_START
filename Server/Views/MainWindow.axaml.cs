using Avalonia.Controls;

namespace Server.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            serverWindow.KeyDown += ServerWindow_KeyDown;
        }

        private void ServerWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                var tb = new TextBlock();
                tb.Text = "I'm rookie";
                list.Items.Add(tb);
            }
        }
    }
}