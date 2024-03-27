using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Diagnostics;


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
        try
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Text File",
                AllowMultiple = false
            });
            string filePath = files[0].Path.LocalPath;

            filePathBox.Text = filePath;
        }
        catch
        {
            Debug.WriteLine("���� ������ ����� �������");
        }
    }
}