

namespace ML_START_1
{
    internal interface ICurrencyStorage
    {
        void ReceiveFrom(ICurrencyStorage sourceStorage, CurrencyType currencyType, int currencycount);

        void RemoveTo(ICurrencyStorage destinationStorage, CurrencyType currencyType, int currencyCount);
    }
}
