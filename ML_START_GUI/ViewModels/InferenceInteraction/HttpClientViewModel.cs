using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;


namespace MLSTART_GUI.ViewModels.InferenceInteraction;

public partial class HttpClientViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendImageCommand))]
    private string? _filePathText;

    [ObservableProperty]
    private string? _statusMessage;

    [RelayCommand(CanExecute = nameof(CanSendImage))]
    private void SendImage()
    {
        string fileExtensionString = FilePathText![^4..]; // = FilePathText.Substring(FilePathText.Length - 4)

        if (fileExtensionString != ".jpg")
        {
            StatusMessage = "неверное имя или формат файла";
        }
    }

    private bool CanSendImage()
    {
        return !string.IsNullOrEmpty(FilePathText);
    }
}
