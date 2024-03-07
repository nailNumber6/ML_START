using Serilog;
using System.Text;
using Serilog.Events;
using Serilog.Configuration;


namespace ToolLibrary;
public static class LoggingTool
{
    /// <summary>
    /// Устанавливает конфигурацию по умолчанию для указанных уровней логирования, первый параметр - минимальный уровень логирования
    /// </summary>
    public static void ConfigureByDefault(LogEventLevel minimumLevel, params LogEventLevel[] logEventLevels) 
    {
       var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel);

        SetLoggingPath(loggerConfig, logEventLevels);
        
        Log.Logger = loggerConfig.CreateLogger(); 
    }

    /// <summary>
    /// Автоматически создает папку для логов по текущей дате
    /// </summary>
    public static void CreateLogDirectory()
    {
        string currentDate = DateTime.Now.Date.ToShortDateString();

        if (!Directory.Exists("log") || !Directory.Exists(currentDate))
            Directory.CreateDirectory(@"log\" + currentDate);
    }

    /// <summary>
    /// Устанавливает путь для записи указанных уровней логирования по текущей дате
    /// </summary>
    public static void SetLoggingPath(LoggerConfiguration loggerConfig, params LogEventLevel[] logEventLevels)
    {
        string logName = string.Empty;

        string currentDate = DateTime.Now.Date.ToShortDateString();

        foreach (var logEventLevel in logEventLevels)
        {
            logName = logEventLevel.ToString().ToLower() + "Log";
            loggerConfig.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
            .WriteTo.File($@"log\{currentDate}\{logName}.txt"));
        };
    }

    /// <summary>
    /// Логирует по шаблону
    /// </summary>
    public static void LogByTemplate(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
    {
        StringBuilder info = new();
        info.Append(note);

        if (ex != null)
        {
            info.Append($"; {ex.Source}; {ex.GetType()}; Сообщение об ошибке: \"{ex.Message}\"");
        }

        info.AppendLine();

        Log.Write(logEventLevel, info.ToString());
    }
}

