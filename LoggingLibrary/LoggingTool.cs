using System.Diagnostics;
using System.Text;
using Serilog.Events;
using Serilog;

namespace LoggingLibrary;

public static class LoggingTool
{
    public static void WriteToFile(params LogEventLevel[] logEventLevels) 
    {
        // TODO: Сделать возможность записывать в консоль
        string currentDate = DateTime.Now.Date.ToShortDateString();

        if (!Directory.Exists("log") || !Directory.Exists(currentDate))
            Directory.CreateDirectory(@"log\" + currentDate);

        var loggerConfig = new LoggerConfiguration();

        string logName = string.Empty;

        foreach (var logEventLevel in logEventLevels)
        {
            logName = logEventLevel.ToString().ToLower() + "Log";
            loggerConfig.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
            .WriteTo.File($@"log\{currentDate}\{logName}.txt",
                outputTemplate: "{Message}{NewLine}"));
        };
        
        Log.Logger = loggerConfig.CreateLogger(); 
    }

    public static void LogByTemplate(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
    {
        StringBuilder info = new(DateTime.Now.ToShortTimeString() + " - ");
        info.Append(note);

        if (ex != null)
        {
            info.Append($"; {ex.Source}; {ex.GetType()}; {ex.Message}");
        }

        Log.Write(logEventLevel, info.ToString());
    }

    public static async Task LogByTemplateAsync(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
    {
        await Task.Run(() => LogByTemplate(logEventLevel, ex, note));
    }
}

