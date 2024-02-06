namespace ML_START_1;

internal class Wardrobe : CurrencyReceiver
{
    public Wardrobe(int storageCapacity, bool storageFillingOn = true) : base(storageCapacity, storageFillingOn)
    {
    }

    public override string ToString()
    {
        return "Шкаф";
    }
}
