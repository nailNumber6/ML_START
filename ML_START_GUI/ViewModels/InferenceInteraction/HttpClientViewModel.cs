using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;


namespace MLSTART_GUI.ViewModels.InferenceInteraction;

public partial class HttpClientViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendImageCommand))]
    private string? _filePathText;

    [RelayCommand(CanExecute = nameof(CanSendImage))]
    private void SendImage()
    {
        string fileExtensionString = FilePathText![^4..]; // = FilePathText.Substring(FilePathText.Length - 4)

        if (fileExtensionString != ".jpg")
        {
            Debug.WriteLine("неверный формат файла");
        }
    }

    private bool CanSendImage()
    {
        return !string.IsNullOrEmpty(FilePathText);
    }
}
