using System.Text.Json;

using ToolLibrary;

using static Serilog.Events.LogEventLevel;
using static ML_START_1.CurrencyType;


namespace ML_START_1;
internal class Program
{
    public record Configuration(int NameLength, int LastNameLength, int ActionDelay);
    static void Main(string[] args)
    {
    }
}