using System.Text;
using System.Security.Cryptography;
using System;
using LoggingLibrary;
using static Serilog.Events.LogEventLevel;

namespace MLSTART_GUI.Services;

public static class Md5Hasher
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
        catch (ArgumentNullException ex)
        {
            LoggingTool.LogByTemplate(Information, ex, "В качестве параметра передана пустая строка");
        }
        catch (Exception ex)
        {
            LoggingTool.LogByTemplate(Warning, ex, "Не удалось совершить хэширование строки");
        }

        return hashedString;
    }
}
