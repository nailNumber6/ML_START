
namespace ML_START_1
{
    internal abstract class CurrencyReceiver
    {
        private Dictionary<CurrencyType, int> _storageArea;
        private int _storageCapacity;

        public CurrencyReceiver(int storageCapacity, bool storageFillingOn = true)
        {
            _storageArea = new Dictionary<CurrencyType, int>();

            CurrencyType[] currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));

            if (storageFillingOn)
            {
                foreach (CurrencyType currencyType in currencyTypes)
                {
                    _storageArea.Add(currencyType, storageCapacity);
                }
            }
            else
            {
                foreach (CurrencyType currencyType in currencyTypes)
                {
                    _storageArea.Add(currencyType, 0);
                }
            }

            _storageCapacity = storageCapacity;
        }

        public override string ToString()
        {
            return "Место для хранения валют";
        }

        public int GetCurrencyCount(CurrencyType currencyType) => _storageArea[currencyType];

        public bool ContainsCurrency(params CurrencyType[] currencyTypes)
        {
            return Array.TrueForAll(currencyTypes, currType => _storageArea[currType] != 0);
        }

        public void ReceiveFrom(CurrencyReceiver sourceStorage, CurrencyType currencyType, int currencyCount)
        {
            sourceStorage._storageArea[currencyType] -= currencyCount;

            if (_storageArea[currencyType] + currencyCount <= _storageCapacity)
                _storageArea[currencyType] += currencyCount;
            else
                Console.WriteLine("Недостаточно места в " + ToString());
        }

        public void RemoveTo(CurrencyReceiver destinationStorage, CurrencyType currencyType, int currencyCount)
        {
            destinationStorage._storageArea[currencyType] += currencyCount;

            if (_storageArea[currencyType] - currencyCount >= 0)
                _storageArea[currencyType] -= currencyCount;
            else Console.WriteLine(ToString() + " не насчитывает столько предметов");
        }
    }

    internal class Wardrobe : CurrencyReceiver
    {
        public Wardrobe(int storageCapacity) : base(storageCapacity)
        {
        }

        public override string ToString()
        {
            return "Шкаф";
        }
    }
}
