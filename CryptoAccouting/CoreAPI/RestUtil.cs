using System;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace CoinBalance.CoreAPI
{
    public static class RestUtil
    {
        public static RestRequest CreateJsonRestRequest(string path, bool addHeader = true)
        {
            var request = new RestRequest(path)
            {
                RequestFormat = DataFormat.Json
            };
            if (addHeader)
            {
                request.AddHeader("Content-Type", "application/json");
            }
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            return request;
        }

        public static async Task<T> ExecuteRequestAsync<T>(IRestClient restClient, RestRequest req) where T : new()
        {
            LogRestRequest(req);
            var response = await restClient.ExecuteTaskAsync<T>(req);
            CheckError(response);
            return response.Data;
        }

        public static async Task ExecuteRequestAsync(IRestClient restClient, RestRequest req)
        {
            LogRestRequest(req);
            var response = await restClient.ExecuteTaskAsync(req);
            CheckError(response);
        }

        private static void LogRestRequest(IRestRequest req)
        {
            System.Diagnostics.Debug.WriteLine($"{req.Method} {req.Resource}");
            foreach (var p in req.Parameters)
            {
                System.Diagnostics.Debug.WriteLine($"{p.Name}, {p.ContentType}, {p.Value}");
            }
        }

        public static void CheckError(IRestResponse response)
        {
            var logText = $"Response from {response.Request.Resource}\n" +
                $"Status Code: {response.StatusCode}, Content:{response.Content}, ErrorMessage: {response.ErrorMessage}";
            System.Diagnostics.Debug.WriteLine(logText);
            if (response.ErrorException != null)
            {
                System.Diagnostics.Debug.WriteLine(logText);
                System.Diagnostics.Debug.WriteLine(response.ErrorException);
                throw response.ErrorException;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                System.Diagnostics.Debug.WriteLine(logText);
                throw new InvalidOperationException(response.Content);
            }
        }
    }
}