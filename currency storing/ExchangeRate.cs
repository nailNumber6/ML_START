using System.Text;

namespace ML_START_1;

internal record ExchangeRate (Dictionary<CurrencyType, double> Rates)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Текущий курс валют:");
        foreach (var rate in Rates)
        {
            sb.AppendLine($"{rate.Key}: {rate.Value}");
        }
        return sb.ToString();
    }
}
