using RestSharp;
using Serilog;

namespace com.epam.rp.core
{
    public static class LoggerService
    {
        public static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Debug() 
                .CreateLogger();
        }

        public static void Info(string message) => Log.Information(message);
        public static void Warn(string message) => Log.Warning(message);
        public static void Error(string message, Exception? ex = null)
        {
            if (ex != null)
                Log.Error(ex, message);
            else
                Log.Error(message);
        }
        
        public static async Task LogRestRequestResponse(RestClient client, RestRequest request, RestResponse response)
        {
            Info($"Request Method: {request.Method}");
            Info($"Request URL: {client.Options.BaseUrl}{request.Resource}");
            var headers = request.Parameters
                .Where(p => p.Type == ParameterType.HttpHeader)
                .Select(p => $"{p.Name}:{p.Value}");
            Info($"Request Headers: {string.Join(", ", headers)}");
            
            var bodyParam = request.Parameters
                .FirstOrDefault(p => p.Type == RestSharp.ParameterType.RequestBody);
            
            if (bodyParam != null)
            {
                Info($"Request Body: {bodyParam.Value}");
            }

            Info($"Response Status: {response.StatusCode}");
            var responseHeaders = response.Headers != null
                ? string.Join(", ", response.Headers.Select(h => $"{h.Name}:{h.Value ?? "<null>"}"))
                : "<no headers>";
            Info($"Response Headers: {responseHeaders}");
            Info($"Response Content: {response.Content}");
        }
    }
}