namespace ML_START_1;

internal class House : IPlacement
{
    private Person _owner;
    public House(Person owner)
    {
        _owner = owner;
    }
    bool IPlacement.CanAccomodate(Person person) => person == _owner;
    public override string ToString()
    {
        return "Дом";
    }
}
