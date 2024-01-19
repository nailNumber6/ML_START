

namespace ML_START_1
{
    internal abstract class Person
    {
        private Pocket _pocket;

        protected Person(int pocketCapacity)
        {
            _pocket = new Pocket(pocketCapacity);
        }

        public void PutCurrency(ICurrencyStorage currencyStorage, CurrencyType currencyType, int currencyCount)
        {
            currencyStorage.Collect(currencyType, currencyCount);
        }

        public void TakeCurrency(ICurrencyStorage currencyStorage, CurrencyType currencyType, int currencyCount)
        {
            currencyStorage.Pull(currencyType, currencyCount);
        }

        private class Pocket : ICurrencyStorage
        {
            private readonly int _storageCapacity; 
            private Dictionary<CurrencyType, int> _storageArea;

            public Pocket(int storageCapacity)
            {
                _storageCapacity = storageCapacity;

                _storageArea = new Dictionary<CurrencyType, int>();
                var currencyTypes = Enum.GetValues(typeof(CurrencyType));

                foreach (CurrencyType currType in currencyTypes)
                    _storageArea.Add(currType, 0);
            }

            void ICurrencyStorage.Collect(CurrencyType currencyType, int count)
            {
                if (_storageArea[currencyType] + count <= _storageCapacity)
                    _storageArea[currencyType] += count;
                else
                    Console.WriteLine("Недостаточно места");
            }

            void ICurrencyStorage.Pull(CurrencyType currencyType, int count)
            {
                if (_storageArea[currencyType] - count >= 0)
                    _storageArea[currencyType] -= count;
                else Console.WriteLine("Место хранения не насчитывает столько предметов");
            }
        }
    }

    internal class MainCharacter : Person 
    {
        public string Name { get; private set; }

        public MainCharacter(string name, int pocketCapacity) : 
        {
            Name = name;
        }
    }

    internal class Extra : Person
    {
        public Extra(int pocketCapacity) : base(pocketCapacity)
        {
        }
    }
}
