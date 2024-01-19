

namespace ML_START_1
{
    internal class Bank : IPlacement
    {
        private class BankChest : ICurrencyStorage
        {
            private Dictionary<CurrencyType, int> _storageArea;

            public BankChest()
            {

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
