using Avalonia.Controls;

namespace MLSTART_GUI.Views
{
    public partial class WindowClosingDialog : Window
    {
        public WindowClosingDialog()
        {
            InitializeComponent();
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }

        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }
    }
}
