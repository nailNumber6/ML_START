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
    private HttpClient _client;

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
            var connectionConfig = Program.Configuration.GetSection("Connection parameters");

            string inferenceUri = connectionConfig["Inference URI"]!;
            string getUri = connectionConfig["GET URI"]!;

            using HttpResponseMessage response = await _client.GetAsync(
                RequestUriBuilder.Build(inferenceUri, getUri));

            status = response.StatusCode;
        }
        catch (Exception ex)
        {
            Log.Error("Попытка выполнения GET-запроса к сервису инференса вызвала исключение {exType} : {exMessage}", 
                ex.GetType(), ex.Message);
        }
        return status;
    }

    public async Task<Dictionary<string, object>> SendImage(string fullImagePath)
    {
        var content = new ByteArrayContent(File.ReadAllBytes(fullImagePath));

        var formData = new MultipartFormDataContent
        {
            { content, "image", fullImagePath }
        };

        #region config values reading
        var connectionConfig = Program.Configuration.GetSection("Connection parameters");
        string inferenceUri = connectionConfig["Inference URI"]!;
        string postUri = connectionConfig["POST URI"]!;
        #endregion

        #region getting service response
        using var response = await _client.PostAsync(RequestUriBuilder.Build(inferenceUri, postUri),
            formData);

        string responseText = await response.Content.ReadAsStringAsync();

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
