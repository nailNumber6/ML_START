

namespace ML_START_1
{
    internal abstract class Person
    {
        private Pocket _pocket;

        private IPlacement _location = null;

        protected Person(int pocketCapacity)
        {
            _pocket = new Pocket(pocketCapacity);
        }
        
        public bool HasVehicle { get; private set; }

        public void ComeIn(IPlacement destination) => _location = destination.IsEnableToEnter ? destination : null;

        public void ComeOut() => _location = null;

        public bool IsIndoors(IPlacement placement) => _location == placement;

        public void RequestToExchange(Bank bank, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
        {
            if (GetCurrencyCount(inputCurrency) >= currencyCount)
            {
                bank.Exchange(this, inputCurrency, returnCurrency, currencyCount);
            }
            else
                Console.WriteLine("Невозможно совершить операцию");
        }

        public int GetCurrencyCount(CurrencyType currencyType) => _pocket.GetCurrencyCount(currencyType);

        public void PutCurrency(CurrencyReceiver destinationStorage, CurrencyType currencyType, int currencyCount)
        {
            destinationStorage.ReceiveFrom(_pocket, currencyType, currencyCount);
        }

        public void TakeCurrency(CurrencyReceiver sourceStorage, CurrencyType currencyType, int currencyCount)
        {
            sourceStorage.RemoveTo(_pocket, currencyType, currencyCount);
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

    internal class MainCharacter : Person, IMovable
    {
        public string Name { get; private set; }

        public MainCharacter(string name, int pocketCapacity) : base(pocketCapacity) => Name = name;

        public void GoTo(IPlacement placement)
        {
            if (HasVehicle)
                Console.WriteLine($"{Name} доезжает до {placement}");
            else
                Console.WriteLine($"{Name} доходит до {placement}");
        }
    }

    internal class Extra : Person, IMovable
    {
        public Extra(int pocketCapacity) : base(pocketCapacity) { }

        public void GoTo(IPlacement placement)
        {
            if (HasVehicle)
                Console.WriteLine($"Прохожий доезжает до {placement}");
            else
                Console.WriteLine($"Прохожий доходит до {placement}");
        }
    }
}
