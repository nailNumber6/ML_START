using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Inference_Interaction_Service.Models;

internal class HttpService
{
    private HttpClient _client;

    public HttpService()
    {
        _client = new HttpClient();
    }

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
