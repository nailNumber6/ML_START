﻿

namespace ML_START_1
{
    internal abstract class Person //TODO: Реализовать метод покупки акций и наоборот
    {
        private Pocket _pocket;

        protected Person(int pocketCapacity)
        {
            _pocket = new Pocket(pocketCapacity);
        }

        public void ComeIn(IPlacement placement)
        {
            if (placement.IsEnableToEnter)
            {
                // TODO: Реализовать метод входа в помещение
            }
        }

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
            private Dictionary<CurrencyType, int> _storageArea;

            public Pocket(int storageCapacity)
            {
                _storageCapacity = storageCapacity;

                _storageArea = new Dictionary<CurrencyType, int>();
                var currencyTypes = Enum.GetValues(typeof(CurrencyType));

                foreach (CurrencyType currType in currencyTypes) // Добавляет все типы валют
                    _storageArea.Add(currType, 0);
            }

            void ICurrencyStorage.ReceiveFrom(ICurrencyStorage sourceStorage, CurrencyType currencyType, int currencyCount)
            {
                sourceStorage.RemoveTo(this, currencyType, currencyCount);

                if (_storageArea[currencyType] + currencyCount <= _storageCapacity)
                    _storageArea[currencyType] += currencyCount;
                else
                    Console.WriteLine("Недостаточно места");
            }

            void ICurrencyStorage.RemoveTo(ICurrencyStorage destinationStorage, CurrencyType currencyType, int currencyCount)
            {
                destinationStorage.ReceiveFrom(this, currencyType, currencyCount);

                if (_storageArea[currencyType] - currencyCount >= 0)
                    _storageArea[currencyType] -= currencyCount;
                else Console.WriteLine("Место хранения не насчитывает столько предметов");
            }
        }
    }

    internal class MainCharacter : Person 
    {
        public string Name { get; private set; }

        public MainCharacter(string name, int pocketCapacity) : base(pocketCapacity) => Name = name;
    }

    internal class Extra : Person
    {
        public Extra(int pocketCapacity) : base(pocketCapacity) { }
    }
}
