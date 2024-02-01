namespace ML_START_1.person;

internal abstract class Person : IMovable
{
    private Pocket _pocket;

    private IPlacement? _location = null; // Не находится в помещении

    protected Person(int pocketCapacity, bool hasVehicle = false)
    {
        _pocket = new Pocket(pocketCapacity);
        HasVehicle = hasVehicle;
    }

    public bool HasVehicle { get; private set; }

    public void GoTo(IPlacement placement)
    {
        if (HasVehicle)
            StoryTeller.AddSentence($"{ToString()} доезжает до {placement}");
        else
            StoryTeller.AddSentence($"{ToString()} доходит до {placement}");
    }

    public override string ToString() => "Человек";

    public void ComeIn(IPlacement destination)
    {
        if (destination.CanAccomodate(this))
            _location = destination;
        else
            StoryTeller.AddSentence($"{destination} закрыт");
    }

    public void ComeOut() => _location = null;

    public bool IsIndoors(IPlacement placement) => _location == placement;

    public void RequestToExchange(Bank bank, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
    {
        if (_location == bank)
        {
            if (GetCurrencyCount(inputCurrency) >= currencyCount)
            {
                bank.Exchange(this, inputCurrency, returnCurrency, currencyCount);
            }
        }
        else StoryTeller.AddSentence($"{this} не находится в помещении {bank}");
    }

    public int GetCurrencyCount(CurrencyType currencyType) => _pocket.GetCurrencyCount(currencyType);

    public void PutCurrency(CurrencyReceiver destinationStorage, CurrencyType currencyType, int currencyCount)
    {
        destinationStorage.ReceiveFrom(_pocket, currencyType, currencyCount);
        StoryTeller.AddSentence($"{ToString()} положил валюту в {destinationStorage}");
    }

    public void TakeCurrency(CurrencyReceiver sourceStorage, CurrencyType currencyType, int currencyCount)
    {
        sourceStorage.RemoveTo(_pocket, currencyType, currencyCount);
        StoryTeller.AddSentence($"{ToString()} положил валюту в {_pocket}");
    }

    private class Pocket : CurrencyReceiver
    {
        public Pocket(int storageCapacity) : base(storageCapacity)
        {
        }

        public override string ToString()
        {
            return "Карман";
        }
    }
}

internal class MainCharacter : Person
{
    public string Name { get; private set; }

    public override string ToString() => Name;

    public MainCharacter(string name, int pocketCapacity, bool hasVehicle = false) : base(pocketCapacity, hasVehicle) => Name = name;
}

internal class Extra : Person
{
    public Extra(int pocketCapacity, bool hasVehicle = false) : base(pocketCapacity) { }

    public override string ToString() => "Прохожий";
}
