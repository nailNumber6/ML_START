﻿using static Serilog.Events.LogEventLevel;


namespace ML_START_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var b = new Bank();
            if (!File.Exists("config.txt")) 
            {
                File.WriteAllText(
                    "config.txt", 
                    "N = 6\n" + "L = 6\n" + "Action delay = 1000"
                    );
            }

            LogHelper.CreateLogDirectory(Debug, Information, Warning, Error);

            Random random = new();

            int[] k = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray(); // 1

            double[] x = new double[13]; // 2
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = random.NextDouble(-12, 16);
                LogHelper.LogByTemplate(Information,
                            note: $"Используется неявное приведение типа int в double, и значение записывается в элемент x[{i}]");
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
                            LogHelper.LogByTemplate(Warning, note:$"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    };
                }
                else if (k[i] == 9)
                {
                    for (int j = 0; j < k2.GetLength(1); j++)
                    {
                        k2[i, j] = Math.Sin(Math.Sin(Math.Pow(x[j] / (x[j] + 0.5), x[j])));
                        if (double.IsNaN(k2[i, j]))
                            LogHelper.LogByTemplate(Warning, note:$"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                }
                else
                {
                    for (int j = 0; j < k2.GetLength(1); j++)
                    {
                        k2[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - x[j] / Math.PI) / 3.0) / 4.0), 3.0));
                        if (double.IsNaN(k2[i, j]))
                            LogHelper.LogByTemplate(Warning, note: $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                }
            }

            try
            { 
                int N = 0, L = 0;

                var configLines = File.ReadLines("config.txt"); // 4
                foreach (string line in configLines)
                {
                    if (line.Contains("N"))
                        N = int.Parse(line.Replace("N = ", "").Trim());
                    else if (line.Contains("L"))
                        L = int.Parse(line.Replace("L = ", "").Trim()); 
                }

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
                LogHelper.LogByTemplate(Error, ex, "Преобразование данных из файла \"config.txt\" вызвало ошибку");
            }
            catch (IndexOutOfRangeException ex)
            {
                LogHelper.LogByTemplate(Error, ex, "Индекс вышел за границы массива");
            }

            var exchangeRate = new ExchangeRate(
                new Dictionary<CurrencyType, double> 
                {
                    [CurrencyType.Fertings] = 1.0,
                    [CurrencyType.Stocks] = 4.0,
                });
            Console.WriteLine(exchangeRate);

            UpdateExchangeRate(exchangeRate, x);
            Console.WriteLine(exchangeRate);
        }


        static void UpdateExchangeRate(ExchangeRate actualRate, double[] priceChanges)
        {
            var random = new Random();
            var newCurrencyPrices = new Dictionary<CurrencyType, double>(actualRate.ExchangeRates);

            priceChanges = priceChanges.Where(n => n >= 0 && !double.IsNaN(n)).ToArray();

            foreach (var currencyType in newCurrencyPrices.Keys)
            {
                double priceChange = priceChanges[random.Next(priceChanges.Length)];

                newCurrencyPrices[currencyType] = actualRate.ExchangeRates[currencyType] += priceChange;
            }
            var updatedRate = actualRate with { ExchangeRates = newCurrencyPrices };
            Thread.Sleep(1000);
        }
    }
}