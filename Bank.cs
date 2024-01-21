

namespace ML_START_1
{
    internal class Bank : IPlacement
    {
        private List<BankChest> _chests;
        bool _isEnableToEnter;
        bool IPlacement.IsEnableToEnter { get => _isEnableToEnter; }

        public Bank(int chestsCount, int[] chestsCapacties)
        {
            _isEnableToEnter = true;

            _chests = new List<BankChest>();
            for (int i = 0; i < chestsCount; i++) 
                _chests.Add(new BankChest(chestsCapacties));
        }
        public void Exchange(Person customer, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
        {
            // TODO: Реализовать метод обмена валют
            if (customer.GetCurrencyCount(inputCurrency) >= currencyCount)
            {
                
            }
        }

        private static class CurrencyConverter
        {
            public static int Convert(ExchangeRate exchangeRate, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
            {

                return 0;
            }
        }

        private class BankChest : ICurrencyStorage // TODO: Реализовать класс
        {
            private Dictionary<CurrencyType, int> _storageArea;
            private int _storageCapacity;

            public BankChest(int storageCapacity)
            {
                _storageArea = new Dictionary<CurrencyType, int>();
                _storageCapacity = storageCapacity;
            } 

            void ICurrencyStorage.ReceiveFrom(ICurrencyStorage sourceStorage, CurrencyType currencyType, int currencyCount) 
            { 

            }

            void ICurrencyStorage.RemoveTo(ICurrencyStorage destinationStorage, CurrencyType currencyType, int currencyCount)
            {

            }
        }
    }
}
