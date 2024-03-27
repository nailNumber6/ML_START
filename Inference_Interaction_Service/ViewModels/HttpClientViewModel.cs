using System.Net;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomMessageBox.Avalonia;

using Inference_Interaction_Service.Models;
using System.Diagnostics;
using System.Text;
using Serilog;


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
            StatusMessage = "неверный формат файла";
        }
        else
        {
            var response = await _httpService.SendImage(FilePathText);

            StringBuilder responseText = new();

            foreach (var responseLine in response)
            {
                responseText.Append($"{responseLine.Key} : {responseLine.Value}");
                responseText.Append('\n');
            }

            new MessageBox(responseText.ToString(), "Сведения", MessageBoxIcon.Information).Show();
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
