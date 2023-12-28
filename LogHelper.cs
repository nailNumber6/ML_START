using Serilog.Events;
using Serilog;
using System.Diagnostics;
using System.Text;


namespace ML_START_1
{
    internal abstract class LogHelper
    {
        public static void CreateLogDirectory(params LogEventLevel[] logEventLevels) 
        {
        if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");

        string logName = string.Empty;
        foreach (var logEventLevel in logEventLevels)
            {
                logName = logEventLevel.ToString().ToLower() + "Log";
                Log.Logger = new LoggerConfiguration()
                .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                .WriteTo.File($@"log\{logName}.txt")).CreateLogger();
            };
        }

        public static void LogByTemplate(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
        {
            StackTrace stackTrace = new StackTrace(true);
            StringBuilder info = new StringBuilder($"\nПримечание: {note}");

            if (ex != null)
            {
                info.AppendLine($"\n{ex.Message}\n");
                stackTrace = new StackTrace(ex, true);
            }

            StackFrame? frame = new StackFrame();
            for (int i = 0; i < stackTrace.FrameCount; i++)
                if (stackTrace.GetFrame(i).GetFileLineNumber() != 0) // Поиск нужного фрейма
                {
                    frame = stackTrace.GetFrame(i);
                    break;
                }

            info.AppendLine($"\nФайл: {frame.GetFileName()}\n");
            info.AppendLine($"Строка: {frame.GetFileLineNumber()}\n");
            info.AppendLine($"Столбец: {frame.GetFileColumnNumber()}\n");
            info.AppendLine($"Метод: {frame.GetMethod()}");

            switch (logEventLevel)
            {
                case LogEventLevel.Debug:
                    {
                        Log.Debug(info.ToString());
                        break;
                    }
                case LogEventLevel.Information:
                    {
                        Log.Information(info.ToString());
                        break;
                    }
                case LogEventLevel.Warning:
                    {
                        Log.Warning(info.ToString());
                        break;
                    }
                case LogEventLevel.Error:
                    {
                        Log.Error(info.ToString());
                        break;
                    }
            };
        }
    }
}
