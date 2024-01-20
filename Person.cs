

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

        public void ComeIn(IPlacement destination) => _location = destination.IsEnableToEnter ? destination : null;

        public void ComeOut() => _location = null;

        public bool IsIndoors(IPlacement placement) => _location == placement;

        public void RequestToExchange()
        {
            // TODO: Реализовать метод запроса на обмен валют в банке
        }

        public int GetCurrencyCount(CurrencyType currencyType) => _pocket.GetCurrencyCount(currencyType);

        public void PutCurrency(ICurrencyStorage destinationStorage, CurrencyType currencyType, int currencyCount)
        {
            destinationStorage.ReceiveFrom(_pocket, currencyType, currencyCount);
        }

        public void TakeCurrency(ICurrencyStorage sourceStorage, CurrencyType currencyType, int currencyCount)
        {
            sourceStorage.RemoveTo(_pocket, currencyType, currencyCount);
        }

        private class Pocket : ICurrencyStorage
        {
            private readonly int _storageCapacity; 
            private Dictionary<CurrencyType, byte> _storageArea;

            public Pocket(byte storageCapacity)
            {
                _storageCapacity = storageCapacity;

                _storageArea = new Dictionary<CurrencyType, byte>();
                var currencyTypes = Enum.GetValues(typeof(CurrencyType));

                foreach (CurrencyType currType in currencyTypes) // Добавляет все типы валют
                    _storageArea.Add(currType, 0);
            }

            void ICurrencyStorage.ReceiveFrom(ICurrencyStorage sourceStorage, CurrencyType currencyType, int currencyCount)
            {
                sourceStorage.RemoveTo(this, currencyType, currencyCount);

                if (_storageArea[currencyType] + currencyCount <= _storageCapacity)
                    _storageArea[currencyType] += (byte)currencyCount;
                else
                    Console.WriteLine("Недостаточно места");
            }

            void ICurrencyStorage.RemoveTo(ICurrencyStorage destinationStorage, CurrencyType currencyType, int currencyCount)
            {
                destinationStorage.ReceiveFrom(this, currencyType, currencyCount);

                if (_storageArea[currencyType] - currencyCount >= 0)
                    _storageArea[currencyType] -= (byte)currencyCount;
                else Console.WriteLine("Место хранения не насчитывает столько предметов");
            }

            public int GetCurrencyCount(CurrencyType currencyType) => _storageArea[currencyType];
        }
    }

    internal class MainCharacter : Person 
    {
        public string Name { get; private set; }

        public MainCharacter(string name, byte pocketCapacity) : base(pocketCapacity) => Name = name;
    }

    internal class Extra : Person
    {
        public Extra(byte pocketCapacity) : base(pocketCapacity) { }
    }
}
