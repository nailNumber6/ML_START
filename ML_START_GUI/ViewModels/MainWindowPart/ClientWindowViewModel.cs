using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomMessageBox.Avalonia;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLSTART_GUI.ViewModels;

// ClientWindowViewModel for interaction with MainWindow
internal partial class ClientWindowViewModel
{
    #region observable properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _loginInput;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _passwordInput;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _repeatPasswordInput;
    #endregion

    #region relay commands
    public bool CanValidate() =>
        !string.IsNullOrEmpty(LoginInput) &&
        !string.IsNullOrEmpty(PasswordInput) &&
        !string.IsNullOrEmpty(RepeatPasswordInput);

    [RelayCommand(CanExecute = nameof(CanValidate))]
    public async Task LogUserIn()
    {
        string authorizationString;

        if (PasswordInput == RepeatPasswordInput)
        {
            if (LoginInput!.Length <= 20)
            {
                // authorization string sending to the server
                authorizationString = $"login {LoginInput} {PasswordInput}";

                CurrentClient = new();
                CurrentClient.Connect(_serverIp, _serverPort);

                using var tcpStream = CurrentClient.GetStream();

                byte[] encodedMessage = Encoding.UTF8.GetBytes(authorizationString);
                await tcpStream.WriteAsync(encodedMessage);

                Log.Information("Клиент {clientAddress} отправил на сервер данные для авторизации: {authString}",
                    CurrentClient.Client.LocalEndPoint, authorizationString);

                byte[] buffer = new byte[1024];
                int readTotal;
                string response = string.Empty;

                while (CurrentClient != null && (readTotal = await tcpStream.ReadAsync(buffer)) != 0)
                {
                    response = Encoding.UTF8.GetString(buffer, 0, readTotal);
                    Log.Information("Ответ сервера: {response}", response);

                    if (response == "success")
                    {
                        Username = LoginInput;
                        IsAuthorized = true;

                        OnPropertyChanged(nameof(ConnectionStateText));
                        OnPropertyChanged(nameof(Username));

                        Log.Information("Пользователь {username} успешно вошел в систему", Username);
                        new MessageBox("Вы успешно вошли!\n Окно авторизации можно закрыть", "Успех", MessageBoxIcon.Information).Show();
                    }
                    else
                    {
                        new MessageBox("Вы не вошли в систему", "Отказ в доступе", MessageBoxIcon.Information).Show();
                        Log.Information("Клиент {clientAddress} не смог войти в систему", CurrentClient.Client.LocalEndPoint);
                        CurrentClient.Client.Close();
                        CurrentClient = null;
                    }
                }
            }
        }
        else
        {
            new MessageBox("Пароли не совпадают", "Ошибка", MessageBoxIcon.Warning).Show();
        }
    }

    [RelayCommand(CanExecute = nameof(CanValidate))]
    public async Task RegisterUser()
    {
        string authorizationString;

        if (PasswordInput == RepeatPasswordInput)
        {
            if (LoginInput!.Length <= 20)
            {
                // authorization string sending to the server
                authorizationString = $"register {LoginInput} {PasswordInput}";

                CurrentClient = new();
                CurrentClient.Connect(_serverIp, _serverPort);

                using var tcpStream = CurrentClient.GetStream();

                byte[] encodedMessage = Encoding.UTF8.GetBytes(authorizationString);
                await tcpStream.WriteAsync(encodedMessage);

                Log.Information("Клиент {clientAddress} отправил на сервер данные для авторизации: {authString}",
                    CurrentClient.Client.LocalEndPoint, authorizationString);

                byte[] buffer = new byte[1024];
                int readTotal;
                string response = string.Empty;

                while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
                {
                    response = Encoding.UTF8.GetString(buffer, 0, readTotal);
                    Log.Information("Ответ сервера: {response}", response);

                    if (response == "success")
                    {
                        Username = LoginInput;
                        IsAuthorized = true;

                        OnPropertyChanged(nameof(ConnectionStateText));
                        OnPropertyChanged(nameof(Username));

                        Log.Information("Пользователь {username} успешно зарегистрирован", Username);
                        new MessageBox("Вы успешно зарегистрировались!\n Окно авторизации можно закрыть", "Успех", MessageBoxIcon.Information).Show();
                    }
                    else if (response == "exists")
                    {
                        new MessageBox("Пользователь с такими данными уже существует", "Отказ в доступе", MessageBoxIcon.Information).Show();
                        Log.Information("Клиент {clientAddress} не зарегистрирован", CurrentClient.Client.LocalEndPoint);
                        CurrentClient.Close();
                        CurrentClient = null;
                    }
                }
            }
        }
        else
        {
            new MessageBox("Пароли не совпадают", "Ошибка", MessageBoxIcon.Warning).Show();
        }
    }

    [RelayCommand]
    public void ResetInputFields()
    {
        LoginInput = string.Empty;
        PasswordInput = string.Empty;
        RepeatPasswordInput = string.Empty;
    }
    #endregion
}
