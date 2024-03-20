using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Media;
using CustomMessageBox.Avalonia;


namespace MLSTART_GUI.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;

        // если пользователь не авторизовалс€ - он не сможет выйти (жестоко)
        Closing += MainWindow_Closing;

        SignInButton.KeyDown += MenuButton_Click;
        SignUpButton.KeyDown += MenuButton_Click;
        ResetButton.KeyDown += MenuButton_Click;

        #region Button Focus events
        SignInButton.GotFocus += MenuButton_GotFocus;
        SignUpButton.GotFocus += MenuButton_GotFocus;
        ResetButton.GotFocus += MenuButton_GotFocus;

        SignInButton.LostFocus += MenuButton_LostFocus; 
        SignUpButton.LostFocus += MenuButton_LostFocus;
        ResetButton.LostFocus += MenuButton_LostFocus;
        #endregion

        #region Control navigation
        LoginField.KeyDown += LoginField_KeyDown;
        PasswordField.KeyDown += PasswordField_KeyDown;
        RepeatPasswordField.KeyDown += RepeatPasswordField_KeyDown;

        SignInButton.KeyDown += SignInButton_KeyDown;
        SignUpButton.KeyDown += SignUpButton_KeyDown;
        ResetButton.KeyDown += ResetButton_KeyDown;
        #endregion
    }

    private void UserAuthorized(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        Close();
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (isAuthorized.IsChecked == false)
        {
            e.Cancel = true;
            new MessageBox("¬ы не авторизовались", "ѕредупреждение", MessageBoxIcon.Warning).Show();
        }
        else
        {
            Closing -= MainWindow_Closing;
        }
    }

    private void MainWindow_Loaded(object? sender, EventArgs e)
    {
        LoginField.Focus();
        isAuthorized.PropertyChanged += UserAuthorized;
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

    #region KeyDown event handlers
    // TextBoxes
    private void LoginField_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down)
        {
            PasswordField.Focus();
        }
    }

    private void PasswordField_KeyDown(object? sender, KeyEventArgs e)  
    {
        if (e.Key == Key.Down)
        {
            RepeatPasswordField.Focus();
        }
        else if (e.Key == Key.Up)
        {
            LoginField.Focus();
        }
    }

    private void RepeatPasswordField_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            PasswordField.Focus();
        }
        else if (e.Key == Key.Down)
        {
            SignInButton.Focus();
        }
    }

    // Buttons
    private void SignInButton_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            RepeatPasswordField.Focus();
        }
        else if (e.Key == Key.Down)
        {
            SignUpButton.Focus();
        }
    }

    private void SignUpButton_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            SignInButton.Focus();
        }
        else if (e.Key == Key.Down)
        {
            ResetButton.Focus();
        }
    }

    private void ResetButton_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            SignUpButton.Focus();
        }
        else if (e.Key == Key.Down)
        {
            LoginField.Focus();
        }
    }
    #endregion
}