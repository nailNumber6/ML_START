

namespace ML_START_1
{
    internal class Bank : IPlacement
    {
        bool _isEnableToEnter;
        bool IPlacement.IsEnableToEnter { get => _isEnableToEnter; }

        public Bank()
        {
            _isEnableToEnter = true;
        }
        public void Exchange(Person customer, CurrencyType inputCurrency, CurrencyType returnCurrency, int currencyCount)
        {
            // TODO: Реализовать метод обмена валют
        }

        private class BankChest : ICurrencyStorage // TODO: Реализовать класс
        {
            private Dictionary<CurrencyType, int> _storageArea;
            private int _storageCapacity;

            public BankChest(int storageCapacity)
            {
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
