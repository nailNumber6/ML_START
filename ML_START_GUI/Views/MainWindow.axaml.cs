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

        // если пользователь не авторизовалс€ - он не сможет выйти
        Closing += MainWindow_Closing;

        registrationPage.GotFocus += RegistrationPage_GotFocus;
        logInPage.GotFocus += LogInPage_GotFocus;

        #region KeyDown events
        SignUpButton.KeyDown += MenuButton_Click;
        ResetButton.KeyDown += MenuButton_Click;

        LoginField2.KeyDown += LoginField2_KeyDown;
        PasswordField2.KeyDown += PasswordField2_KeyDown;
        SignInButton.KeyDown += SignInButton_KeyDown1;
        #endregion

        #region Button Focus events
        SignUpButton.GotFocus += MenuButton_GotFocus;
        ResetButton.GotFocus += MenuButton_GotFocus;
        SignInButton.GotFocus += MenuButton_GotFocus;
        ResetButton2.GotFocus += MenuButton_GotFocus;

        SignUpButton.LostFocus += MenuButton_LostFocus;
        ResetButton.LostFocus += MenuButton_LostFocus;
        SignInButton.LostFocus += MenuButton_LostFocus;
        ResetButton2.LostFocus += MenuButton_LostFocus;
        #endregion

        #region Control navigation
        LoginField.KeyDown += LoginField_KeyDown;
        PasswordField.KeyDown += PasswordField_KeyDown;
        RepeatPasswordField.KeyDown += RepeatPasswordField_KeyDown;

        SignUpButton.KeyDown += SignUpButton_KeyDown;
        ResetButton.KeyDown += ResetButton_KeyDown;
        #endregion
    }

    private void LogInPage_GotFocus(object? sender, GotFocusEventArgs e)
    {
        LoginField.Clear();
        PasswordField.Clear();
        RepeatPasswordField.Clear();
    }

    private void RegistrationPage_GotFocus(object? sender, GotFocusEventArgs e)
    {
        LoginField2.Clear();
        PasswordField2.Clear();
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
            SignUpButton.Focus();
        }
    }

    // Log in buttons
    private void SignInButton_KeyDown1(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            PasswordField2.Focus();
        }
        else if (e.Key == Key.Down)
        {
            ResetButton2.Focus();
        }
    }

    private void PasswordField2_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            LoginField2.Focus();
        }
        else if (e.Key == Key.Down)
        {
            SignInButton.Focus();
        }
    }

    private void LoginField2_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down)
        {
            PasswordField2.Focus();
        };
    }

    // Registration Buttons
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
            PasswordField.Focus();
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