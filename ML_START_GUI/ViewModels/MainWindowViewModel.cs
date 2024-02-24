using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomMessageBox.Avalonia;

using LoggingLibrary;
using MLSTART_GUI.Models.TestDB;
using MLSTART_GUI.Views;


namespace MLSTART_GUI.ViewModels;
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _loginInput;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _passwordInput;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand), nameof(LogUserInCommand))]
    private string? _repeatPasswordInput;


    [RelayCommand(CanExecute = nameof(CanValidate))]
    public async Task RegisterUser()
    {
        using var context = new TestContext();

        string statusMessage = "Пользователь успешно зарегистрирован";
        var icon = MessageBoxIcon.Error;

        if (PasswordInput == RepeatPasswordInput)
        {
            try
            {
                var newUser = User.Create(LoginInput!, PasswordInput!);
                await context.AddAsync(newUser);
                await context.SaveChangesAsync();

                icon = MessageBoxIcon.Information;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // TODO: Обработка всех исключений 
            }
        }
        else
        {
            statusMessage = "Некорректный ввод логина или пароля";
        }

        new MessageBox(statusMessage, string.Empty, icon).Show();
    }

    [RelayCommand(CanExecute = nameof(CanValidate))]
    public async Task LogUserIn()
    {
        using var context = new TestContext();

        string statusMessage = $"Добро пожаловать, {LoginInput}";
        var icon = MessageBoxIcon.Error;

        if (PasswordInput == RepeatPasswordInput)
        {
            if (await context.UserExistsAsync(LoginInput!, PasswordInput!))
            {
                var clientWindow = new ClientWindow(LoginInput);

                clientWindow.Show();


                icon = MessageBoxIcon.Information;
            }
            else
            {
                statusMessage = "Некорректные данные пользователя";
            }
        }
        else
        {
            statusMessage = "Некорректный ввод логина или пароля";
        }
        new MessageBox(statusMessage, string.Empty, icon).Show();
    }

    public bool CanValidate() => 
        !string.IsNullOrEmpty(LoginInput) &&
        !string.IsNullOrEmpty(PasswordInput) &&
        !string.IsNullOrEmpty(RepeatPasswordInput);

    [RelayCommand]
    public void ResetInputFields()
    {
        LoginInput = string.Empty; 
        PasswordInput = string.Empty; 
        RepeatPasswordInput = string.Empty;
    }
}
