

namespace ML_START_1
{
    internal class Bank : IPlacement
    {
        private bool _isOpen;
        private ExchangeRate _exchangeRate;
        private List<BankChest> _chests;
        private BankChest _reserveChest;

        public Bank(ExchangeRate exchangeRate, int chestsCount, int[] chestsCapacties, bool chestsFillingOn = true)
        {
            _exchangeRate = exchangeRate;
            _isOpen = true;
            TotalCapacity = chestsCapacties.Sum();
            _reserveChest = new BankChest(chestsCapacties.Max(), false);

            _chests = new List<BankChest>();

            if (chestsFillingOn)
            {
                for (int i = 0; i < chestsCount; i++)
                    _chests.Add(new BankChest(chestsCapacties[i]));
            }
            else
            {
                for (int i = 0; i < chestsCount; i++)
                    _chests.Add(new BankChest(chestsCapacties[i], false));
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

        public bool HasCurrency() => !_reserveChest.ContainsCurrency();

        public void Exchange(Person customer, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
        {
            var currentChest = _chests.Find(chest => chest.ContainsCurrency(inputCurrency, returnCurrency)); // Сундуки, содержащие валюту
            if (currentChest == null) 
                StoryTeller.AddSentence($"В {ToString()} закончилась запрашиваемая валюта");

            double inputCurrencyPrice = _exchangeRate.ExchangeRates[inputCurrency];
            double returnCurrencyPrice = _exchangeRate.ExchangeRates[returnCurrency];
            int currencyToSpend = (int)Math.Round(currencyCount / inputCurrencyPrice);

            StoryTeller.AddSentence($"{customer} обменял валюту");

            if (currencyToSpend + currentChest.GetCurrencyCount(inputCurrency) > currentChest.StorageCapacity) // Проверка того, что в сундук поместится данное кол-во валюты
                customer.PutCurrency(currentChest, inputCurrency, currencyToSpend);
            else
            { 
                customer.PutCurrency(_reserveChest, inputCurrency, currencyToSpend);
            }

            customer.TakeCurrency(currentChest, inputCurrency, currencyCount);
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
}
