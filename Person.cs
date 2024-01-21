

namespace ML_START_1
{
    internal abstract class Person
    {
        private Pocket _pocket;

        private IPlacement _location = null;

        protected Person(byte pocketCapacity) // Так как в кармане мало места, объем инвентаря записан в переменную типа byte
        {
            _pocket = new Pocket(pocketCapacity);
        }
        
        public bool HasVehicle { get; private set; }

        public void ComeIn(IPlacement destination) => _location = destination.IsEnableToEnter ? destination : null;

        public void ComeOut() => _location = null;

        public bool IsIndoors(IPlacement placement) => _location == placement;

        public void RequestToExchange(CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
        {
            // TODO: Реализовать метод запроса на обмен валют в банке
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
            public Pocket(byte storageCapacity) : base(storageCapacity)
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

        public MainCharacter(string name, byte pocketCapacity) : base(pocketCapacity) => Name = name;

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
        public Extra(byte pocketCapacity) : base(pocketCapacity) { }

        public void GoTo(IPlacement placement)
        {
            if (HasVehicle)
                Console.WriteLine($"Прохожий доезжает до {placement}");
            else
                Console.WriteLine($"Прохожий доходит до {placement}");
        }
    }
}
