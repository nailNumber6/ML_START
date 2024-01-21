

using System.Text;

namespace ML_START_1
{
    internal record ExchangeRate (Dictionary<CurrencyType, double> ExchangeRates)
    {
        public override string ToString()
        {
            Console.WriteLine("Текущий курс валют:");
            var sb = new StringBuilder();
            foreach (var rate in ExchangeRates)
            {
                sb.AppendLine($"{rate.Key}: {rate.Value}");
            }
            return sb.ToString();
        }
    }
}
