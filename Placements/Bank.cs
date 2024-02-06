

namespace ML_START_1;

internal class Bank : IPlacement
{
    private bool _isOpen;
    private ExchangeRate _exchangeRate;
    private List<BankChest> _chests;

    public Bank(ExchangeRate exchangeRate, int chestsCount, int[] chestsCapacties)
    {
        // Инициализация банка с пустыми сундуками
        _exchangeRate = exchangeRate;
        _isOpen = true;
        TotalCapacity = chestsCapacties.Sum();

        _chests = new List<BankChest>();

        for (int i = 0; i < chestsCount; i++)
            _chests.Add(new BankChest(chestsCapacties[i], false));
    }

    public Bank(ExchangeRate exchangeRate, int chestsCount, int[] chestsCapacties, CurrencyType[]? currenciesToFill)
    {
        // Инициализация банка с заполненными или частично заполненными сундуками
        _exchangeRate = exchangeRate;
        _isOpen = true;
        TotalCapacity = chestsCapacties.Sum();
        _chests = new List<BankChest>();

        if (currenciesToFill == null)
            for (int i = 0; i < chestsCount; i++)
                _chests.Add(new BankChest(chestsCapacties[i]));
        else
        {
            for (int i = 0; i < chestsCount; i++)
            {
                var newChest = new BankChest(chestsCapacties[i], false);
                newChest.FillWith(currenciesToFill);
                _chests.Add(newChest);
            }
        }
    }

    public int TotalCapacity { get; }

    public bool IsOpen { get => _isOpen; }

    bool IPlacement.CanAccomodate(Person person)
    {
        return _isOpen;
    }

    public int GetChestsCount() => _chests.Count;

    public void ToggleBankStatus() => _isOpen = !_isOpen;

    public override string ToString()
    {
        return "Банк";
    }

    public bool IsFull() => _chests.TrueForAll(chest => chest.IsFull());

    public bool IsFull(CurrencyType currencyType) => _chests.TrueForAll(chest => chest.IsFull(currencyType));

    public bool HasCurrency(params CurrencyType[] currencyTypes) => _chests.TrueForAll(chest => chest.ContainsCurrency(currencyTypes));

    public void Exchange(Person customer, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
    {
        var currentChest = _chests
            .Find(chest =>
            chest.ContainsCurrency(inputCurrency, returnCurrency) && !chest.IsFull(inputCurrency)
        ); // Сундук с запрашиваемой валютой и не переполненный валютой для обмена

        if (currentChest == null)
        {
            StoryTeller.AddSentence($"В {ToString()} закончилась запрашиваемая валюта");
        }
        else
        {
            double inputCurrencyPrice = _exchangeRate.Rates[inputCurrency];
            double returnCurrencyPrice = _exchangeRate.Rates[returnCurrency];
            int currencyToSpend = (int)Math.Round(returnCurrencyPrice * currencyCount / inputCurrencyPrice);

            StoryTeller.AddSentence($"{customer} обменял валюту");

            customer.PutCurrency(currentChest, inputCurrency, currencyToSpend);
            customer.TakeCurrency(currentChest, returnCurrency, currencyCount);
        }
    }

    private class BankChest : CurrencyReceiver
    {
        public BankChest(int storageCapacity, bool storageFillingOn = true) : base(storageCapacity, storageFillingOn)
        {
        }

        public override string ToString()
        {
            return "Банковский сундук";
        }
    }
}
