using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Avalonia;
using Avalonia.ReactiveUI;

using ToolLibrary;
using static Serilog.Events.LogEventLevel;


namespace Server;
internal record Configuration(int NameLength, int LastNameLength, int ActionDelay, string ConnectionString);
internal class Program
{
    internal const string CONFIG_FILE_NAME = "config.json";
    internal static Configuration Config { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        LoggingTool.WriteToFile(Debug, Information, Warning, Error);

        #region creating or reading config file
        if (!File.Exists(CONFIG_FILE_NAME))
        {
            var config = new Dictionary<string, object>()
            {
                ["NameLength"] = 6,
                ["LastNameLength"] = 6,
                ["ActionDelay"] = 500,
                ["ConnectionString"] = "Server=(localdb)\\MSSQLLocalDB;Database=Test;Trusted_Connection=True;TrustServerCertificate=True"
            };
            string configText = JsonSerializer.Serialize(config);

            File.WriteAllText(CONFIG_FILE_NAME, configText);
        }
        string configFileContent = File.ReadAllText(CONFIG_FILE_NAME);

        try
        {
            Config = JsonSerializer
                .Deserialize<Configuration>(configFileContent)!;
        }
        catch (JsonException ex)
        {
            LoggingTool.LogByTemplate(Error, ex, $"Чтение данных из файла {CONFIG_FILE_NAME} вызвало ошибку");
        }
        catch (NullReferenceException ex)
        {
            LoggingTool.LogByTemplate(Error, ex, $"Чтение данных из файла {CONFIG_FILE_NAME} вызвало ошибку");
        }
        catch (Exception ex)
        {
            LoggingTool.LogByTemplate(Error, ex, $"Чтение данных из файла {CONFIG_FILE_NAME} вызвало непредвиденную ошибку");
        }
        #endregion

        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
