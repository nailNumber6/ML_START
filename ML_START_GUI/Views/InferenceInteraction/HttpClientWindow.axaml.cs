using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace MLSTART_GUI;

public partial class HttpClientWindow : Window
{
    public HttpClientWindow()
    {
        InitializeComponent();

        browseButton.Click += BrowseButton_Click;
        filePathBox.GotFocus += FilePathBox_GotFocus;
    }

    private void FilePathBox_GotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
        statusMessage.Text = string.Empty;
    }

    private async void BrowseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Start async operation to open the dialog
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        string filePath = files[0].Path.LocalPath;

        filePathBox.Text = filePath;
    }
}