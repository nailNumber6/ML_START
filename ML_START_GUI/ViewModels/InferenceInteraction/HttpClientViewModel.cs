using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;


namespace MLSTART_GUI.ViewModels.InferenceInteraction;

public partial class HttpClientViewModel : ObservableObject
{
    [RelayCommand(CanExecute = nameof(CanSendImage))]
    private void OpenExplorer()
    {
        //Process.Start("explorer.exe");
    }

    private bool CanSendImage()
    {
        return true;
    }
}
