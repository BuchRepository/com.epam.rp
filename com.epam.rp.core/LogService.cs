using Newtonsoft.Json;
using ReportPortal.Serilog;
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
                .WriteTo.ReportPortal()
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
        
        public static void LogRestRequestResponse(RestClient client, RestRequest request, RestResponse response)
        {
            Info($"Request Method: {request.Method}");
            Info($"Request URL: {client.Options.BaseUrl}{request.Resource}");
            
            var requestHeaders = request.Parameters
                .Where(p => p.Type == ParameterType.HttpHeader)
                .Select(p => $"{p.Name}:{p.Value}");
            
            var defaultHeaders = client.DefaultParameters
                .Where(p => p.Type == ParameterType.HttpHeader)
                .Select(p => $"{p.Name}: {p.Value}");
            
            var allHeaders = requestHeaders.Concat(defaultHeaders).ToList();
            Info($"Request Headers: {(allHeaders.Count > 0 ? string.Join(", ", allHeaders) : "<no headers>")}");
            
            var bodyParam = request.Parameters
                .FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if (bodyParam != null)
            {
                string json = JsonConvert.SerializeObject(bodyParam.Value, Formatting.Indented);
                Info($"Request Body: {json}");
            }

            Info($"Response Status: {response.StatusCode}");
            var responseHeaders = new List<string>();

            if (response.Headers != null)
            {
                foreach (var header in response.Headers)
                {
                    var value = header.Value;
                    responseHeaders.Add($"{header.Name}: {value}");
                }
            }

            string headersLog = responseHeaders.Count > 0
                ? string.Join(", ", responseHeaders)
                : "<no headers>";

            Info("Response Headers: " + headersLog);
            
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                try
                {
                    var parsed = JsonConvert.DeserializeObject(response.Content);
                    string pretty = JsonConvert.SerializeObject(parsed, Formatting.Indented);
                    Info($"Response Content: {pretty}");
                }
                catch
                {
                    Info($"Response Content: {response.Content}");
                }
            }
            else
            {
                Info("Response Content: <empty>");
            }
        }
    }
}