using Avalonia.Controls;

namespace MLSTART_GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        authentificationWindow.Loaded += AuthentificationWindow_Loaded;

        // TODO: мб сделать перемещение менее костыльным
        LoginField.KeyDown += LoginField_KeyDown;
        PasswordField.KeyDown += PasswordField_KeyDown;
        RepeatPasswordField.KeyDown += RepeatPasswordField_KeyDown;
    }

    private void AuthentificationWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        LoginField.Focus();
    }

    private void RepeatPasswordField_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Up)
        {
            PasswordField.Focus();
        }
    }

    private void PasswordField_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Down)
        {
            RepeatPasswordField.Focus();
        }
        else if (e.Key == Avalonia.Input.Key.Up)
        {
            LoginField.Focus();
        }
    }

    private void LoginField_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Down)
        {
            PasswordField.Focus();
        }
    }
}