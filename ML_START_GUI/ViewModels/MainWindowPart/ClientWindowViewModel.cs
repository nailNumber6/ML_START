using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomMessageBox.Avalonia;
using Serilog;
using MLSTART_GUI.Models;

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
    public bool CanValidateForRegistration() =>
        !string.IsNullOrEmpty(LoginInput) &&
        !string.IsNullOrEmpty(PasswordInput) &&
        !string.IsNullOrEmpty(RepeatPasswordInput);

    public bool CanValidateForLogin() =>
        !string.IsNullOrEmpty(LoginInput) &&
        !string.IsNullOrEmpty(PasswordInput);

    [RelayCommand(CanExecute = nameof(CanValidateForLogin))]
    public async Task LogUserIn()
    {
        string authorizationString;

        if (LoginInput!.Length <= 20)
        {
            authorizationString = $"login {LoginInput} {PasswordInput}";

            CurrentClient = new();
            bool connectionIsSuccessful = CurrentClient.TryConnect(_serverIp, _serverPort);

            if (connectionIsSuccessful)
            {
                NetworkMessages.Add($"Клиент {CurrentClient.Client.LocalEndPoint} подключен к серверу {_serverIp} : {_serverPort}");

                #region authorization data sending to the server
                var tcpStream = CurrentClient.GetStream();

                byte[] encodedMessage = Encoding.UTF8.GetBytes(authorizationString);
                await tcpStream.WriteAsync(encodedMessage);

                Log.Information("Клиент {clientAddress} отправил на сервер {serverIp} : {serverPort} данные для авторизации",
                    CurrentClient.Client.LocalEndPoint, _serverIp, _serverPort);
                #endregion

                #region server response processing
                int readTotal;
                string response;
                byte[] buffer = new byte[1024];

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
                        OnPropertyChanged(nameof(IsAuthorized));

                        Log.Information("Пользователь {username} успешно вошел в систему", Username);
                        new MessageBox("Вы успешно вошли!\n", "Успех", MessageBoxIcon.Information).Show();
                    }
                    else
                    {
                        new MessageBox("Вы не вошли в систему", "Отказ в доступе", MessageBoxIcon.Information).Show();
                        Log.Information("Клиент {clientAddress} не смог войти в систему", CurrentClient.Client.LocalEndPoint);

                        DisconnectFromServer();
                    }
                    break;
                }
                #endregion
            }
            else
            {
                new MessageBox("Ошибка при подключении", "Авторизация", MessageBoxIcon.Error).Show();
            }
        }

        else
        {
            new MessageBox("Пароли не совпадают", "Ошибка", MessageBoxIcon.Warning).Show();
        }
    }

    [RelayCommand(CanExecute = nameof(CanValidateForRegistration))]
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
                bool connectionSuccessful = CurrentClient.TryConnect(_serverIp, _serverPort);

                if (connectionSuccessful)
                {
                    NetworkMessages.Add($"Клиент {CurrentClient.Client.LocalEndPoint} подключен к серверу {_serverIp} : {_serverPort}");

                    #region authorization data sending to the server
                    var tcpStream = CurrentClient.GetStream();

                    byte[] encodedMessage = Encoding.UTF8.GetBytes(authorizationString);
                    await tcpStream.WriteAsync(encodedMessage);

                    Log.Information("Клиент {clientAddress} отправил на сервер {serverIp} : {serverPort} данные для авторизации",
                        CurrentClient.Client.LocalEndPoint, _serverIp, _serverPort);
                    #endregion

                    #region server response processing
                    int readTotal;
                    string response;
                    byte[] buffer = new byte[1024];

                    while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
                    {
                        response = Encoding.UTF8.GetString(buffer, 0, readTotal);
                        Log.Information("Ответ сервера: {response}", response);

                        switch (response)
                        {
                            case "success":
                                {
                                    Username = LoginInput;
                                    IsAuthorized = true;

                                    OnPropertyChanged(nameof(ConnectionStateText));
                                    OnPropertyChanged(nameof(Username));
                                    OnPropertyChanged(nameof(IsAuthorized));

                                    Log.Information("Пользователь {username} успешно зарегистрирован", Username);
                                    new MessageBox("Вы успешно зарегистрировались!", "Успех", MessageBoxIcon.Information).Show();
                                    break;
                                }
                            case "exists":
                                {
                                    new MessageBox("Пользователь с такими данными уже существует", "Отказ в доступе", MessageBoxIcon.Information).Show();
                                    Log.Information("Клиент {clientAddress} не зарегистрирован", CurrentClient.Client.LocalEndPoint);
                                    DisconnectFromServer();
                                    break;
                                }
                            case "error":
                                {
                                    new MessageBox("При регистрации на стороне сервера произошла ошибка", "Отказ в доступе", MessageBoxIcon.Information).Show();
                                    Log.Information("Клиент {clientAddress} не зарегистрирован", CurrentClient.Client.LocalEndPoint);
                                    CurrentClient.Close();
                                    CurrentClient = null;
                                    break;
                                }
                            default:
                                {
                                    Log.Warning("Поступил неожидаемый ответ от сервера {response}", response);
                                    break;
                                }
                        }
                        break;
                    }
                    #endregion
                }
                else
                {
                    new MessageBox("Ошибка при подключении", "Авторизация", MessageBoxIcon.Error).Show();
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
