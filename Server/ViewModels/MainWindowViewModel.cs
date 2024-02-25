using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

using ML_START_1;
using static ML_START_1.CurrencyType;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;


namespace Server.ViewModels;
public partial class MainWindowViewModel : ObservableObject
{
    public string IpAddress => "127.0.0.1";
    public int Port { get; private set; } = 8080;

    [ObservableProperty]
    private int _clientsCount;

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
            await Dispatcher.UIThread
                .InvokeAsync(() =>
                listBox.Items.Add(tcpClient.Client.RemoteEndPoint!.ToString()));

            Debug.WriteLine(tcpClient.Client.RemoteEndPoint);
        }
    }

    public async Task StartAndShowStory(ListBox listBox)
    {
        while (true)
        {
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

            //TODO: значения из конфиг файла
            StoryBuilder.AddSentence($"На улице стояла прекрасная погода, градусник показывал {10}°C");

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

            await Task.Delay(500);
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
                await DisplayStory(StoryBuilder.Story, listBox, 500);
                StoryBuilder.Clear();
                await Task.Delay(500);
            }
            //Console.Clear();
            StoryBuilder.AddSentence($"В результате {bank.TotalCapacity}, хранившиеся в {bank.GetChestsCount()} несгораемых сундуках, были быстро распроданы.");
            //StoryTeller.Tell(500);
            await DisplayStory(StoryBuilder.Story, listBox, 500);
        }
    }

    private async Task DisplayStory(List<string> story, ListBox listBox, int delayInMilliseconds)
    {
        foreach (var sentence in story)
        {
            listBox.Items.Add(sentence);
            await Task.Delay(delayInMilliseconds);
        }
    }
}
