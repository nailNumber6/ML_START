using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Inference_Interaction_Service.Models;

internal class HttpService
{
    private readonly HttpClient _client;

    public HttpService()
    {
        _client = new HttpClient();
    }

    /// <summary>
    /// Отправляет GET-запрос сервису инференса
    /// </summary>
    /// <returns>Код статуса</returns>
    public async Task<HttpStatusCode> PerformHealthCheck()
    {
        HttpStatusCode status = new();
        try
        {
            #region config values reading
            var connectionConfig = Program.Configuration.GetSection("Connection parameters");
            string inferenceUri = connectionConfig["Inference URI"]!;
            string getUri = connectionConfig["GET URI"]!;
            #endregion

            #region receiving service response
            string serviceUri = RequestUriBuilder.Build(inferenceUri, getUri);

            using HttpResponseMessage response = await _client.GetAsync(serviceUri);

            Log.Information("Отправлен GET-запрос сервису {serviceURI}", serviceUri);

            status = response.StatusCode;

            Log.Information("Ответ от сервиса {serviceURI} : {responseText}", serviceUri, response.Content.ToString());
            #endregion
        }
        catch (Exception ex)
        {
            Log.Error("Попытка выполнения GET-запроса к сервису инференса вызвала исключение {exType} : {exMessage}", 
                ex.GetType(), ex.Message);
        }
        return status;
    }

    /// <summary>
    /// Отправляет POST-запрос сервису инференса
    /// </summary>
    /// <param name="localImagePath">Локальный путь к изображению</param>
    /// <returns>Содержание ответа сервиса + статус-код сообщения</returns>
    public async Task<Dictionary<string, object>> SendImage(string localImagePath)
    {
        var content = new ByteArrayContent(File.ReadAllBytes(localImagePath));

        var formData = new MultipartFormDataContent
        {
            { content, "image", localImagePath }
        };

        #region config values reading
        var connectionConfig = Program.Configuration.GetSection("Connection parameters");
        string inferenceUri = connectionConfig["Inference URI"]!;
        string postUri = connectionConfig["POST URI"]!;
        #endregion

        #region receiving service response
        string serviceUri = RequestUriBuilder.Build(inferenceUri, postUri);

        using var response = await _client.PostAsync(serviceUri,
            formData);

        Log.Information("На сервис {serviceURI} отправлено изображение для изменения размера. Путь к изображению: {imagePath}",
                serviceUri, localImagePath);

        string responseText = await response.Content.ReadAsStringAsync();

        Log.Information("Ответ от сервиса {serviceURI} : {responseText}", serviceUri, responseText);

        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseText);

        result!.Add("Статус", response.StatusCode.ToString());

        #endregion

        return result;
    }

    private static class RequestUriBuilder
    {
        public static string Build(params string[] uriParts)
        {
            StringBuilder fullUri = new();
            foreach (string uriPart in uriParts)
            {
                fullUri.Append(uriPart);
            }
            return fullUri.ToString();
        }
    }
}
