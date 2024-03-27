using System.Net;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomMessageBox.Avalonia;

using Inference_Interaction_Service.Models;


namespace Inference_Interaction_Service.ViewModels;

public partial class HttpClientViewModel : ObservableObject
{
    private readonly HttpService _httpService = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendImageCommand))]
    private string? _filePathText;

    [ObservableProperty]
    private string? _statusMessage;

    [RelayCommand(CanExecute = nameof(CanSendImage))]
    private async Task SendImage()
    {
        string fileExtensionString = FilePathText![^4..]; // = FilePathText.Substring(FilePathText.Length - 4)

        if (fileExtensionString != ".jpg")
        {
            StatusMessage = "неверное имя или формат файла";
        }
        else
        {
            // POST
        }
    }
    private bool CanSendImage()
    {
        return !string.IsNullOrEmpty(FilePathText)
            && FilePathText.Length > 4;
    }

    [RelayCommand]
    private async Task CheckServiceHealth()
    {
        HttpStatusCode healthState = await _httpService.PerformHealthCheck();

        new MessageBox($"Состояние : {healthState}", "Сведения", MessageBoxIcon.Information).Show();
    }

    public void ClearStatusMessage()
    {
        if (!string.IsNullOrEmpty(StatusMessage))
        {
            StatusMessage = string.Empty;
        }
    }
}
