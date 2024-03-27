using Avalonia.Controls;
using Avalonia.Platform.Storage;


namespace Inference_Interaction_Service.Views;

public partial class HttpClientWindow : Window
{
    public HttpClientWindow()
    {
        InitializeComponent();

        browseButton.Click += BrowseButton_Click;
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