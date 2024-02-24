using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Avalonia;
using Avalonia.ReactiveUI;

using LoggingLibrary;

using static Serilog.Events.LogEventLevel;


namespace MLSTART_GUI
{
    internal record Configuration(int NameLength, int LastNameLength, int ActionDelay);

    internal class Program
    {
        internal static Configuration Config { get; private set; } = null!;

        [STAThread]
        public static void Main(string[] args)
        {
            LoggingTool.WriteToFile(Debug, Information, Warning, Error);

            #region creating or reading config file
            string configFile = "config.json";
            if (!File.Exists(configFile))
            {
                var config = new Dictionary<string, object>()
                {
                    ["NameLength"] = 6,
                    ["LastNameLength"] = 6,
                    ["ActionDelay"] = 500
                };
                string configText = JsonSerializer.Serialize(config);

                File.WriteAllText(configFile, configText);
            }
            string configFileContent = File.ReadAllText(configFile);

            try
            {
                Config = JsonSerializer
                    .Deserialize<Configuration>(configFileContent)!;
            }
            catch (JsonException ex)
            {
                LoggingTool.LogByTemplate(Error, ex, $"Чтение данных из файла {configFile} вызвало ошибку");
            }
            catch (NullReferenceException ex)
            {
                LoggingTool.LogByTemplate(Error, ex, $"Чтение данных из файла {configFile} вызвало ошибку");
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
}