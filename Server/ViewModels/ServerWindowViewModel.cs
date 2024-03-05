﻿using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

using Avalonia.Threading;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomMessageBox.Avalonia;

using ML_START_1;
using ToolLibrary;
using static ML_START_1.CurrencyType;
using static Serilog.Events.LogEventLevel;


namespace Server.ViewModels;
public partial class ServerWindowViewModel : ObservableObject
{
    public string IpAddress => "127.0.0.1";
    public int Port { get; private set; } = 8080;

    // TODO: Перенести необходимое в Model

    public async Task StartServer(ListBox listBox)
    {
        var tcpListener = new TcpListener(IPAddress.Parse(IpAddress), Port);

        try
        {
            tcpListener.Start();


            while (true)
            { 
                var tcpClient = await tcpListener.AcceptTcpClientAsync();

                Task.Run(async () => await ProcessClientAsync(tcpClient));
            }
        }
        finally
        {
            tcpListener.Stop();
        }

        async Task ProcessClientAsync(TcpClient tcpClient)
        {
            if (tcpClient.Connected!)
            {

            }

            #region connected client into ListBox
            string? clientRow = tcpClient.Client.RemoteEndPoint?.ToString();

            await Dispatcher.UIThread
                .InvokeAsync(() =>
                listBox.Items.Add(clientRow ?? "ошибка добавления клиента"));
            #endregion

            await LoggingTool.LogByTemplateAsync(Information, note: $"Подключился клиент с адресом {tcpClient.Client.RemoteEndPoint}");

            #region reading and responding to the client's message
            var tcpStream = tcpClient.GetStream();

            byte[] buffer = new byte[256];
            int readTotal;

            while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, readTotal);

                string response = "сообщение получено";

                await tcpStream.WriteAsync(Encoding.UTF8.GetBytes(response));

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    new MessageBox("Сервер получил от клиента: " + receivedMessage, "Сервер").Show();
                });
            }
            #endregion
        }
    }

    public async Task StartAndShowStory(ListBox listBox)
    {
        Random random = new();

        while (true)
        {
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
                for (int j = 0; j < k2.GetLength(1); j++)
                {
                    if (range.Contains(k[i]))
                    {
                        double expression = 0.5 / (Math.Tan(2 * x[j]) + (2.0 / 3.0));
                        k2[i, j] = Math.Pow(expression, Math.Pow(Math.Pow(x[j], 1.0 / 3.0), 1.0 / 3.0));
                        if (double.IsNaN(k2[i, j]))
                            LoggingTool.LogByTemplate(Warning, note: $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                    else if (k[i] == 9)
                    {
                        k2[i, j] = Math.Sin(Math.Sin(Math.Pow(x[j] / (x[j] + 0.5), x[j])));
                        if (double.IsNaN(k2[i, j]))
                            LoggingTool.LogByTemplate(Warning, note: $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                    else
                    {
                        k2[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - x[j] / Math.PI) / 3.0) / 4.0), 3.0));
                        if (double.IsNaN(k2[i, j]))
                            LoggingTool.LogByTemplate(Warning, note: $"В результате вычислений элементу k2[{i}, {j}] было присвоено NaN");
                    }
                }
            }
            #endregion

            #region values from Program.Configuration, getting minElement and averageValue
            double minElement = 0.0, averageValue = 0.0;
            double totalSum = 0.0;
            int actionDelay = 0;

            try
            {
                int N = Convert.ToInt32(Program.Config.NameLength);
                int L = Convert.ToInt32(Program.Config.LastNameLength);
                actionDelay = Program.Config.ActionDelay;

                double[] subArray1 = Enumerable.Range(0, k2.GetLength(1))
                                .Select(col => k2[N % 8, col])
                                .ToArray();
                minElement = subArray1.Min(); // 5 Задание

                double[] subArray2 = Enumerable.Range(0, k2.GetLength(1))
                                .Select(col => k2[L % 13, col])
                                .ToArray();
                averageValue = subArray2.Average();

                totalSum = minElement + averageValue; // 6 Задание
            }
            catch (FormatException ex)
            {
                LoggingTool.LogByTemplateAsync(Error, ex, $"Преобразование данных из файла {Program.CONFIG_FILE_NAME} вызвало ошибку");
            }
            catch (IndexOutOfRangeException ex)
            {
                LoggingTool.LogByTemplateAsync(Error, ex, "Индекс вышел за границы массива");
            }
            #endregion

            #region story about Neznayka
            #region story objects initializing
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
            #endregion

            StoryBuilder.AddSentence($"На улице стояла прекрасная погода, градусник показывал {totalSum}°C");

            character1.ComeIn(bank);
            character1.RequestToExchange(bank, Fertings, Stocks, 1000);

            StoryBuilder.AddSentence("Наступило утро...");
            character2.GoTo(bank);
            character3.GoTo(bank);
            character3.ComeIn(bank);
            character2.ComeIn(bank);
            character2.RequestToExchange(bank, Fertings, Stocks, 1000);

            character3.RequestToExchange(bank, Fertings, Stocks, 1000);
            character3.GoTo(ch3House);
            character3.ComeIn(ch3House);
            character3.PutCurrency(ch3Wardrobe, Fertings, 100);

            StoryBuilder.AddSentence("Наступил вечер...");
            await DisplayStory(StoryBuilder.Story, listBox, 500);

            await Task.Delay(actionDelay);
            StoryBuilder.Clear();
            StoryBuilder.AddSentence("Утро следующего дня...");

            bank.ToggleBankStatus();
            while (!bank.IsFull(Fertings))
            {
                //Console.Clear();
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
                    StoryBuilder.AddSentence("Многие покупатели являлись в контору слишком рано. От нечего делать они толклись на улице, дожидаясь открытия конторы.");
                    bank.ToggleBankStatus();
                }
                await DisplayStory(StoryBuilder.Story, listBox, actionDelay);
                StoryBuilder.Clear();
                await Task.Delay(actionDelay);
            }

            StoryBuilder.AddSentence($"В результате {bank.TotalCapacity}, хранившиеся в {bank.GetChestsCount()} несгораемых сундуках, были быстро распроданы.");
            
            await DisplayStory(StoryBuilder.Story, listBox, 500);
            #endregion
        }
    }

    private async Task DisplayStory(List<string> story, ListBox listBox, int delayInMilliseconds)
    {
        await Task.Delay(1000); // в это время завершается чтение конфига
        foreach (var sentence in story)
        {
            listBox.Items.Add(sentence);
            await Task.Delay(delayInMilliseconds);
        }
        listBox.Items.Clear();
    }
}