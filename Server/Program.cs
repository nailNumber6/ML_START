using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Avalonia;
using Avalonia.ReactiveUI;

using ToolLibrary;
using static Serilog.Events.LogEventLevel;


namespace Server;

internal record class Config(int NameLength, int LastNameLength, int ActionDelay, Dictionary<string, string>? ConnectionStrings);

internal class Program
{
    internal const string CONFIG_FILE_NAME = "config.json";
    internal static Config ConfigSettings { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        LoggingTool.CreateLogDirectory();
        LoggingTool.ConfigureByDefault(Debug, Debug, Information, Warning, Error);

        var os = Environment.OSVersion;
        LoggingTool.LogByTemplate(Debug, note: $"Проект запустился на платформе ОС: {os.Platform}, версии: {os.Version}");

        #region creating or reading config file
        if (!File.Exists(CONFIG_FILE_NAME))
        {
            var cfg = new Config(6, 6, 500,
                new Dictionary<string, string>
                {
                    ["MSSQL Server"] = "Server=(localdb)\\MSSQLLocalDB;Database=Test;Trusted_Connection=True;TrustServerCertificate=True"
                });
            #pragma warning disable CA1869
            var options = new JsonSerializerOptions { WriteIndented = true };
            #pragma warning restore CA1869

            string configText = JsonSerializer.Serialize(cfg, options);

            File.WriteAllText(CONFIG_FILE_NAME, configText);
        }
        string configFileContent = File.ReadAllText(CONFIG_FILE_NAME);

        try
        {
            ConfigSettings = JsonSerializer
                .Deserialize<Config>(configFileContent)!;
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
