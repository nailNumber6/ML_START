using System.Text.Json;

using LoggingLibrary;

using static Serilog.Events.LogEventLevel;
using static ML_START_1.CurrencyType;


namespace ML_START_1;
internal class Program
{
    public record Configuration(int NameLength, int LastNameLength, int ActionDelay); 
    static void Main(string[] args)
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

        Configuration configuration = null!;

        try
        {
            configuration = JsonSerializer
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

        // TODO: Реализовать обновление модели данных

        Random random = new();

        #region k and x filling
        int[] k = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray(); // 1 Задание 

        double[] x = new double[13]; // 2 Задание
        for (int i = 0; i < x.Length; i++)
        {
            x[i] = random.NextDouble(-12, 16);
            LoggingTool.LogByTemplate(Information,
                        note: $"Используется неявное приведение типа int в double, и значение записывается в элемент x[{i}]");
        }
        #endregion

        #region k2 array filling
        double[,] k2 = new double[8, 13]; // 3 Задание
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
                        LoggingTool.LogByTemplate(Warning, note:$"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                };
            }
            else if (k[i] == 9)
            {
                for (int j = 0; j < k2.GetLength(1); j++)
                {
                    k2[i, j] = Math.Sin(Math.Sin(Math.Pow(x[j] / (x[j] + 0.5), x[j])));
                    if (double.IsNaN(k2[i, j]))
                        LoggingTool.LogByTemplate(Warning, note:$"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                }
            }
            else
            {
                for (int j = 0; j < k2.GetLength(1); j++)
                {
                    k2[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - x[j] / Math.PI) / 3.0) / 4.0), 3.0));
                    if (double.IsNaN(k2[i, j]))
                        LoggingTool.LogByTemplate(Warning, note: $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                }
            }
        }
        #endregion

        #region values from config.json, getting minElement and averageValue
        double minElement = 0.0, averageValue = 0.0;
        int actionDelay = 0;

        try
        {
            int N = Convert.ToInt32(configuration.NameLength); 
            int L = Convert.ToInt32(configuration.LastNameLength);
            actionDelay = configuration.ActionDelay;

            double[] subArray1 = Enumerable.Range(0, k2.GetLength(1))
                            .Select(col => k2[N % 8, col])
                            .ToArray();
            minElement = subArray1.Min(); // 5 Задание

            double[] subArray2 = Enumerable.Range(0, k2.GetLength(1))
                            .Select(col => k2[L % 13, col])
                            .ToArray();
            averageValue = subArray2.Average();

            Console.WriteLine($"Минимальный элемент - {minElement:F4}"); // 6 Задание
            Console.WriteLine($"Среднее число - {averageValue:F4}");
        }
            catch (FormatException ex)
            {
                LoggingTool.LogByTemplate(Error, ex, $"Преобразование данных из файла {configFile} вызвало ошибку");
            }
            catch (IndexOutOfRangeException ex)
            {
                LoggingTool.LogByTemplate(Error, ex, "Индекс вышел за границы массива");
            }
        #endregion

        #region story about Neznayka
        var exchangeRate = new ExchangeRate(
            new Dictionary<CurrencyType, double> 
            {
                [Fertings] = 1.0,
                [Stocks] = 1.1,
            });
        
        var currenciesToFill = new CurrencyType[] { Stocks };
        var bankChestsCapacities = new int[] { 1000000, 1000000 };

        var bank = new Bank(exchangeRate, 2, bankChestsCapacities, currenciesToFill); 

        MainCharacter character1 = new("Коротышка", 1000000);
        MainCharacter character2 = new("Незнайка", 1000);

        MainCharacter character3 = new("Мига", 1000, true);
        House ch3House = new(character3);
        Wardrobe ch3Wardrobe = new(10000, false);

        StoryTeller.AddSentence($"На улице стояла прекрасная погода, градусник показывал {minElement + averageValue}°C");

        character1.ComeIn(bank);
        character1.RequestToExchange(bank, Fertings, Stocks, 1000);

        StoryTeller.AddSentence("Наступило утро...");
        character2.GoTo(bank);
        character3.GoTo(bank);
        character3.ComeIn(bank);
        character2.ComeIn(bank);
        character2.RequestToExchange(bank, Fertings, Stocks, 1000);

        character3.RequestToExchange(bank, Fertings, Stocks, 1000);
        character3.GoTo(ch3House);
        character3.ComeIn(ch3House);
        character3.PutCurrency(ch3Wardrobe, Fertings, 100);

        StoryTeller.AddSentence("Наступил вечер...");
        StoryTeller.Tell(actionDelay);

        Thread.Sleep(actionDelay);
        StoryTeller.Clear();
        StoryTeller.AddSentence("Утро следующего дня...");

        bank.ToggleBankStatus();
        while (!bank.IsFull(Fertings))
        {
            Console.Clear();
            var crowd = new Queue<Extra>();

            if (crowd.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                    crowd.Enqueue(new Extra(10000000));
            }

            for (int i = 0; i < crowd.Count; i++)
            {
                var currentCustomer = crowd.Dequeue();
                currentCustomer.ComeIn(bank);
                currentCustomer.RequestToExchange(bank, Fertings, Stocks, 100000);
            }

            if (!bank.IsOpen)
            {
                StoryTeller.AddSentence("Многие покупатели являлись в контору слишком рано. От нечего делать они толклись на улице, дожидаясь открытия конторы.");
                bank.ToggleBankStatus();
            }    

            StoryTeller.Tell(actionDelay);
            StoryTeller.Clear();
            Thread.Sleep(actionDelay);
            UpdateExchangeRate(exchangeRate, x);
        }
        Console.Clear();
        StoryTeller.AddSentence($"В результате {bank.TotalCapacity}, хранившиеся в {bank.GetChestsCount()} несгораемых сундуках, были быстро распроданы.");
        StoryTeller.Tell(actionDelay);
        #endregion
    }

    static void UpdateExchangeRate(ExchangeRate actualRate, double[] priceChanges)
    {
        var random = new Random();
        var newCurrencyPrices = new Dictionary<CurrencyType, double>(actualRate.Rates);

        priceChanges = priceChanges.Where(n => n >= 0 && !double.IsNaN(n)).ToArray();

        foreach (var currencyType in newCurrencyPrices.Keys)
        {
            double priceChange = priceChanges[random.Next(priceChanges.Length)];

            newCurrencyPrices[currencyType] = actualRate.Rates[currencyType] += priceChange;
        }
        var updatedRate = actualRate with { Rates = newCurrencyPrices };
        StoryTeller.AddSentence(updatedRate.ToString());
    }
}