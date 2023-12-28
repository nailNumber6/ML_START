using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Text;

namespace ML_START_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Debug)
                .WriteTo.File("debugLog.txt"))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
                .WriteTo.File("infoLog.txt"))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning)
                .WriteTo.File("warningsLog.txt"))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)
                .WriteTo.File("errorsLog.txt"))
            .CreateLogger();

            int[] k = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray(); // 1

            double[] x = new double[13]; // 2
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = random.Next(-12, 16) + random.NextDouble();
            }

            double[,] k2 = new double[8, 13]; // 3
            int[] range = { 5, 7, 11, 15 };

            for (int i = 0; i < k.Length; i++)
            {
                if (range.Contains(k[i]))
                {
                    for (int j = 0; j < k2.GetLength(1); j++)
                    {
                        double expression = 0.5 / (Math.Tan(2 * x[j]) + (2.0 / 3.0));
                        k2[i, j] = Math.Pow(expression, Math.Pow(Math.Pow(x[j], 1.0 / 3.0), 1.0 / 3.0));
                        if (double.IsNaN(k2[i, j]))
                            LogByTemplate(LogEventLevel.Warning, null, $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    };
                }
                else if (k[i] == 9)
                {
                    for (int j = 0; j < k2.GetLength(1); j++)
                    {
                        k2[i, j] = Math.Sin(Math.Sin(Math.Pow(x[j] / (x[j] + 0.5), x[j])));
                        if (double.IsNaN(k2[i, j]))
                            LogByTemplate(LogEventLevel.Warning, null, $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                    }
                else
                {
                    for (int j = 0; j < k2.GetLength(1); j++)
                    {
                        k2[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - x[j] / Math.PI) / 3) / 4), 3));
                        if (double.IsNaN(k2[i, j]))
                            LogByTemplate(LogEventLevel.Warning, null, $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                }
            }

            try
            {
                if (!File.Exists("config.txt")) // 4
                {
                    File.Create("config.txt");
                    File.AppendAllText("config.txt", $"{6} {6}");
                }

                string[] variables = File.ReadAllText("config.txt").Split();
                int N = int.Parse(variables[0]);
                int L = int.Parse(variables[1]);

                double[] subArray1 = Enumerable.Range(0, k2.GetLength(1))
                                .Select(col => k2[N % 8, col])
                                .ToArray();
                double minElement = subArray1.Min(); // 5

                double[] subArray2 = Enumerable.Range(0, k2.GetLength(1))
                                .Select(col => k2[L % 13, col])
                                .ToArray();
                double averageValue = subArray2.Average();

                Console.WriteLine($"Минимальный элемент - {minElement:F4}"); // 6
                Console.WriteLine($"Среднее число - {averageValue:F4}");
            }
            catch (FormatException ex)
            {
                LogByTemplate(LogEventLevel.Error, ex, "Преобразование данных из файла \"config.txt\" вызвало ошибку");
            }
            catch (IndexOutOfRangeException ex)
            {
                LogByTemplate(LogEventLevel.Error, ex, "Индекс вышел за границы массива");
            }
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

            info.AppendLine($"Файл: {frame.GetFileName()}\n");
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
 

    
