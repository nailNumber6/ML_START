using System.Text;


namespace ToolLibrary;

public static class NetworkMessageProcessor
{
    /// <summary>
    /// Создает сообщение по шаблону
    /// </summary>
    /// <param name="mode">Режим сообщения, передаваемого в сеть. Нужен для дальнейшего чтения сообщения</param>
    /// <param name="messagePart1">Часть сообщения</param>
    /// <param name="messagePart2">Нужен когда сообщение включает в себя две части (например, логин и пароль)</param>
    /// <returns>Возвращает построенное по шаблону сообщение</returns>
    public static string BuildMessage(NetworkMessageMode mode, string messagePart1, string? messagePart2 = null)
    {
        StringBuilder message = new(mode + " ");

        message.Append(messagePart1 + " ");
        if (messagePart2 != null) message.Append(messagePart2);
        
        return message.ToString().Trim();
    }

    /// <summary>
    /// Обрабатывает сообщение, переданное в сеть. Для корректности работы следует передавать сообщение, построенное с помощью метода BuildMessage
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns></returns>
    public static List<string> ReadMessage(string message)
    {
        string[] messageParams = message.Split();
        string mode = messageParams[0];

        var result = new List<string>();
        switch (mode)
        {
            case "Message":
                result[0] = nameof(NetworkMessageMode.Message);
                result[1] = messageParams[1];
                return result;
            case "AuthRequest":
                break;
        }
        return result;
    }
}
