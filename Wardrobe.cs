

namespace ML_START_1
{
    internal class Wardrobe : ICurrencyStorage //TODO: Реализовать интерфейс
    {
        public void ReceiveFrom(ICurrencyStorage sourceStorage, CurrencyType currencyType, int currencyCount)
        {
            throw new NotImplementedException();
        }

        public void RemoveTo(ICurrencyStorage destinationStorage, CurrencyType currencyType, int currencyCount)
        {
            throw new NotImplementedException();
        }
    }
}
