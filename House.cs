

namespace ML_START_1
{
    internal class House : IPlacement
    {
        private Person _owner;
        public House(Person owner = null)
        {
            _owner = owner;
        }
        bool IPlacement.CanAccomodate(Person person) => person == _owner;
    }
}
