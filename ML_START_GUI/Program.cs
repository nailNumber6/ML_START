using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Avalonia;
using Avalonia.ReactiveUI;

using LoggingLibrary;

using static Serilog.Events.LogEventLevel;


namespace MLSTART_GUI;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        LoggingTool.WriteToFile(Debug, Information, Warning, Error);

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