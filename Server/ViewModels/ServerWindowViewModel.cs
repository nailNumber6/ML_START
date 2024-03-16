using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Serilog;
using CommunityToolkit.Mvvm.ComponentModel;

using ML_START_1;
using static ML_START_1.CurrencyType;
using Server.Models.Network;
using Server.Models.TestDB;
using Azure;
using Avalonia.Threading;


namespace Server.ViewModels;
public partial class ServerWindowViewModel : ObservableObject
{
    private IPAddress _serverIp;
    private int _serverPort;

    TcpServer _server;

    public ServerWindowViewModel()
    {
        #region server address reading from the config file
        try
        {
            var connectionParameters = Program.Configuration.GetSection("Connection parameters");
            _serverIp = IPAddress.Parse(connectionParameters["Server IP"]!);
            _serverPort = int.Parse(connectionParameters["Server port"]!);
        }
        catch (Exception ex)
        {
            Log.Error("Источник: {thisProject}.При попытке прочитать значения для подключения из файла {config} произошла ошибка {exType} : {exMessage}",
                ex.Source, Program.configFileName, ex.GetType(), ex.Message);
        }
        #endregion

        _items = [];
        _networkMessages = [];
    }

    public IPAddress IpAddress { get { return _serverIp; } }
    public int Port { get { return _serverPort; } }

    #region ServerWindow collections
    private ObservableCollection<string> _items;

    public ObservableCollection<string> Items
    {
        get { return _items; }
        set { SetProperty(ref _items, value); }
    }

    private ObservableCollection<string> _networkMessages;

    public ObservableCollection<string> NetworkMessages
    {
        get { return _networkMessages; }
        set { SetProperty(ref _networkMessages, value); }
    }
    #endregion


    public async Task StartServer()
    {
        _server = new TcpServer(IpAddress, Port);
        try
        {
            _server.Start();
            Log.Information("Сервер с адресом {ip} : {port} запустился и начал принимать подключения",
                _serverIp, _serverPort);
      
            while (true)
            {
                TcpClient tcpClient = await _server.AcceptTcpClientAsync();
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                Task.Run(async () => await ProcessClientAsync(tcpClient));
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            }
        }
        catch(Exception ex)
        {
            Log.Error("Во время работы сервера произошла ошибка {exType} : {exMessage}", ex.GetType(), ex.Message);
        }

        async Task ProcessClientAsync(TcpClient tcpClient)
        {
            string? clientRow = tcpClient.Client.RemoteEndPoint!.ToString();

            Log.Information("Подключился клиент с адресом {clientEndPoint}", tcpClient.Client.RemoteEndPoint);

            #region reading client's message and responding to it 
            var tcpStream = tcpClient.GetStream();

            byte[] buffer = new byte[1024];
            int readTotal;

            string response = string.Empty;

            while ((readTotal = await tcpStream.ReadAsync(buffer)) != 0)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, readTotal);
                Log.Information("Получено сообщение от клиента; Текст сообщения: {messageText}", receivedMessage);

                #region message processing
                // getting parts of message
                string[] messageParts = receivedMessage.Split();

                string command = messageParts[0];

                if (command == "login")
                {
                    Log.Information("Клиент {clientAddress} отправил запрос на вход в систему", clientRow);
                    using TestContext context = new();

                    var login = messageParts[1]; var password = messageParts[2];

                    if (await context.UserExistsAsync(login, password))
                    {
                        response = "success";

                        NetworkMessages.Add($"Клиент {clientRow!} подключился!");

                        Log.Information("Пользователь {userLogin} успешно авторизовался", login);
                    }
                    else
                    {
                        response = "fail";
                        Log.Information("Пользователь {userLogin} не авторизовался", login);
                    }
                    await tcpStream.WriteAsync(Encoding.UTF8.GetBytes(response));
                }
                else if (command == "register")
                {
                    Log.Information("Клиент {clientAddress} отправил запрос на регистрацию в системе", clientRow);
                    using TestContext context = new();

                    var login = messageParts[1]; var password = messageParts[2];

                    if (!await context.UserExistsAsync(login, password))
                    {
                        var newUser = User.Create(login, password);
                        await context.AddAsync(newUser);
                        await context.SaveChangesAsync();

                        response = "success";
                        NetworkMessages.Add($"Клиент {clientRow} подключился!");
                    }
                    else
                    {
                        response = "exists";
                    }
                    await tcpStream.WriteAsync(Encoding.UTF8.GetBytes(response));
                }
                else if (command == "message")
                {
                    response = "сообщение получено";
                    Log.Information("От клиента {clientAddress} получено простое сообщение");

                    NetworkMessages.Add("Сообщение от клиента: " + receivedMessage);

                    await tcpStream.WriteAsync(Encoding.UTF8.GetBytes(response));
                }
                #endregion

                 Log.Information("Клиенту отправлен ответ: {response}", response);
            }
            #endregion

            NetworkMessages.Add($"Клиент {clientRow!} отключился!");
            Log.Information("Отключился от сервера клиент с адресом {clientAddress}", clientRow);
        }
    }

    #region story displaying methods
    public async Task StartAndShowStory()
    {
        await Task.Delay(1000); // during this time configuration is being read
        Random random = new();

        while (true)
        {
            #region k and x filling
            int[] k = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray(); // 1 Задание 

            double[] x = new double[13]; // 2 Задание
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = random.NextDouble(-12, 16);
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
                    }
                    else if (k[i] == 9)
                    {
                        k2[i, j] = Math.Sin(Math.Sin(Math.Pow(x[j] / (x[j] + 0.5), x[j])));
                    }
                    else
                    {
                        k2[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - x[j] / Math.PI) / 3.0) / 4.0), 3.0));
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
                var variables = Program.Configuration.GetSection("Integer variables");
                int N = Convert.ToInt32(variables["N"]);
                int L = Convert.ToInt32(variables["L"]);
                actionDelay = Convert.ToInt32(variables["action delay"]);

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
            catch (Exception ex)
            {
                Log.Error("Чтение значений переменных из конфигурационного файла {configFile} вызвало исключение: {exType}. Сообщение: {exMessage}", 
                    Program.configFileName, ex.GetType(), ex.Message);
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
            await DisplayStory(StoryBuilder.Story, 500);

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
                await DisplayStory(StoryBuilder.Story, actionDelay);
                StoryBuilder.Clear();
                await Task.Delay(actionDelay);
            }

            StoryBuilder.AddSentence($"В результате {bank.TotalCapacity}, хранившиеся в {bank.GetChestsCount()} несгораемых сундуках, были быстро распроданы.");
            
            await DisplayStory(StoryBuilder.Story, 500);
            #endregion
        }
    }

    private async Task DisplayStory(List<string> story, int delayInMilliseconds)
    {
        foreach (var sentence in story)
        {
            Items.Add(sentence);
            await Task.Delay(delayInMilliseconds);
        }
        Items.Clear();
    }
    #endregion
}
