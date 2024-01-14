

namespace ML_START_1
{
    internal static class RandomClassExtentions
    {
        public static double NextDouble(this Random random, int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue) + random.NextDouble(); 
        }
    }
}
