using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;

using Serilog;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using Microsoft.Extensions.Configuration;

using ToolLibrary;


namespace Server;

internal class Program
{
    internal static readonly string configFileName = "appsettings.json";

    private static IConfiguration _configuration = null!;
    public static IConfiguration Configuration { get { return _configuration; } }

    [STAThread]
    public static void Main(string[] args)
    {
        #region "appsettings.json" creating if does not exists
        if (!File.Exists(configFileName))
        {
            var configurationSettings = new AppConfigurationSettings()
            {
                IntVariables = new()
                {
                    ["N"] = 6,
                    ["L"] = 6,
                    ["action delay"] = 500
                },
                ConnectionStrings = new()
                {
                    ["MSSQL Server"] = "Server=(localdb)\\MSSQLLocalDB;Database=Test;Trusted_Connection=True;TrustServerCertificate=True"
                },
                SerilogSettings = new()
                {
                    ["Using"] = "Serilog.Sinks.File",
                    ["MinimumLevel"] = "Debug",
                    ["WriteTo"] = new List<Dictionary<object, object>>()
                    {
                        new()
                        {
                            ["Name"] = "File",
                            ["Args"] = new Dictionary<string,string>() 
                            { 
                                ["path"] = $"logs/log-.txt", 
                                ["rollingInterval"] = "Day",
                                ["outputTemplate"] = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u11}] {Message:lj}{NewLine}{Exception}"
                            }
                        }
                    }
                }
            };
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var jsonConfig = JsonSerializer.Serialize(configurationSettings, serializerOptions);

            File.WriteAllText(configFileName, jsonConfig);
        }
        #endregion

        _configuration = new ConfigurationBuilder()
        .AddJsonFile(configFileName)
        .Build();

        Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(_configuration)
           .CreateLogger();

        string projectName = Assembly.GetExecutingAssembly().GetName().Name!;
        var os = Environment.OSVersion;

        Log.Debug("Проект {projectName} запустился на платформе ОС: {osPlatform}, версии: {osVersion}", projectName, os.Platform, os.Version);
        Log.Debug("Версия репозитория: {currentCommit}; Ветка: {currentBranch}", ThisAssembly.Git.Commit, ThisAssembly.Git.Branch);

        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

        Log.CloseAndFlush();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        GC.KeepAlive(typeof(Interaction).Assembly);
        GC.KeepAlive(typeof(ComparisonConditionType).Assembly);
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
