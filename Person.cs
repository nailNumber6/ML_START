

namespace ML_START_1
{
    internal abstract class Person
    {

    }

    internal class MainCharacter : Person 
    {
        public string Name { get; private set; }

        public MainCharacter(string name) 
        {
            Name = name;
        }
    }

    internal class Extra : Person { }
}
