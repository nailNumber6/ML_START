

namespace ML_START_1
{
    internal class Bank : IPlacement
    {
        private int _instancesCount = 0;
        private bool _isEnableToEnter;
        private ExchangeRate _exchangeRate;
        private List<BankChest> _chests;

        public Bank(ExchangeRate exchangeRate, int chestsCount, int[] chestsCapacties)
        {
            _instancesCount++;
            _exchangeRate = exchangeRate;
            _isEnableToEnter = true;

            _chests = new List<BankChest>();
            for (int i = 0; i < chestsCount; i++) 
                _chests.Add(new BankChest(chestsCapacties[i]));
        }
        
        bool IPlacement.IsEnableToEnter { get => _isEnableToEnter; }
        public int InstancesCount { get => _instancesCount; }

        public void Exchange(Person customer, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
        {
            var currentChest = _chests.Find(chest => chest.ContainsCurrency(inputCurrency, returnCurrency)); // Сундуки, содержащие валюту

            double inputCurrencyPrice = _exchangeRate.ExchangeRates[inputCurrency];
            double returnCurrencyPrice = _exchangeRate.ExchangeRates[returnCurrency];
            int currencyToSpend = (int)Math.Round(currencyCount / inputCurrencyPrice);

            customer.PutCurrency(currentChest, inputCurrency, currencyToSpend);
            customer.TakeCurrency(currentChest, inputCurrency, currencyCount);
        }

        private class BankChest : CurrencyReceiver
        {
            public BankChest(int storageCapacity) : base(storageCapacity)
            {
            }

            public override string ToString()
            {
                return "Банковский сундук";
            }
        }
    }
}
