

namespace ML_START_1
{
    internal interface ICurrencyStorage
    {
        void Collect(CurrencyType currencyType, int count);

        void Pull(CurrencyType currencyType, int count);
    }
}
