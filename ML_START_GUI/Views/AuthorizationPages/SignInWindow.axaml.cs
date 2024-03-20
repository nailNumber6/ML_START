using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CustomMessageBox.Avalonia;

namespace MLSTART_GUI;

public partial class SignInWindow : Window
{
    public SignInWindow()
    {
        InitializeComponent();
        SignInButton.KeyDown += MenuButton_Click;
    }
    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (isAuthorized.IsChecked == false)
        {
            e.Cancel = true;
            new MessageBox("Вы не авторизовались", "Предупреждение", MessageBoxIcon.Warning).Show();
        }
        else
        {
            Closing -= MainWindow_Closing;
        }
    }

    private void MenuButton_Click(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is Button button)
        {
            var clickEvent = new RoutedEventArgs(Button.ClickEvent);
            button.RaiseEvent(clickEvent);
        }
    }

    private void MenuButton_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.BorderThickness = Thickness.Parse("0");
        }
    }

    private void MenuButton_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is Button button)
        {
            button.BorderThickness = Thickness.Parse("2");
            button.BorderBrush = Brushes.Black;
        }
    }
}