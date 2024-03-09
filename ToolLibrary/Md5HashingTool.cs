using System.Text;
using System.Security.Cryptography;

using static Serilog.Events.LogEventLevel;
using Serilog;


namespace ToolLibrary;
public static class Md5HashingTool
{
    public static string GetHash(string input)
    {
        string hashedString = string.Empty;

        try
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5.HashData(byteArray);

            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            hashedString = Convert.ToString(sb)!;
        }
        catch (Exception ex)
        {
            Log.Error("Источник: {exSource}; Исключение: {exType}; Сообщение: {exMessage}", ex.Source, ex.GetType(), ex.Message);
        }
        return hashedString;
    }
}
