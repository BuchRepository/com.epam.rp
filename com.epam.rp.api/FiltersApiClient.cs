using com.epam.rp.core;
using com.epam.rp.core.Configuration;
using com.epam.rp.core.Models;
using RestSharp;

namespace com.epam.rp.api;

public class FiltersApiClient
{
    private readonly RestClient _client;
    private const string BaseUrl = "https://rp.epam.com/";
    private const string Project = "test_user_personal";
    private const string ApiPrefix = "api/v1/"; 
    private static readonly string ApiToken = ConfigManager.ApiToken;

    public FiltersApiClient()
    {
        _client = new RestClient(BaseUrl);
        _client.AddDefaultHeader("Authorization", $"bearer {ApiToken}");
    }
    
    private static string BuildUrl(string path) => $"{ApiPrefix}{Project}/{path}";
    
        public async Task<RestResponse> GetFiltersAsync()
        {
            var url = BuildUrl($"filter");
            var request = (new RestRequest(url, Method.Get));

            var response = await _client.ExecuteAsync(request);
            
            LoggerService.LogRestRequestResponse(_client, request, response);

            return response;
        }
    
    public async Task<RestResponse> GetFilterByIdAsync(int filterId)
    {
        var url = BuildUrl($"filter/{filterId}");
        var request = (new RestRequest(url, Method.Get));

        var response = await _client.ExecuteAsync(request);
        
        LoggerService.LogRestRequestResponse(_client, request, response);

        return response;
    }

    public async Task<RestResponse> CreateFilterAsync(
        CreateFilterRequest? body = null,
        string? name = null,
        string? description = null,
        string? type = null,
        List<FilterCondition>? conditions = null,
        List<FilterOrder>? orders = null)
    {
        if (body == null)
        {
            body = new CreateFilterRequest
            {
                Name = name,
                Description = description,
                Type = type,
                Conditions = conditions ?? new List<FilterCondition>(),
                Orders = orders ?? new List<FilterOrder>()
            };
        }
        
        var request = new RestRequest(BuildUrl("filter"), Method.Post).AddJsonBody(body);
        var response = await _client.ExecuteAsync(request);
        LoggerService.LogRestRequestResponse(_client, request, response);
        return response;
    }

    public async Task<RestResponse> UpdateFiltersAsync(
        UpdateFiltersRequest? body = null,
        int? id = null,
        string? name = null,
        string? description = null,
        string? type = null,
        List<FilterCondition>? conditions = null,
        List<FilterOrder>? orders = null)
    {
        if (body == null)
        {
            body = new UpdateFiltersRequest
            {
                Elements = new List<UpdateFilterElement>
                {
                    new UpdateFilterElement
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        Type = type,
                        Conditions = conditions ?? new List<FilterCondition>(),
                        Orders = orders ?? new List<FilterOrder>()
                    }
                }
            };
        }
            
        var request = new RestRequest(BuildUrl($"filter"), Method.Put).AddJsonBody(body);
        var response = await _client.ExecuteAsync(request);
        LoggerService.LogRestRequestResponse(_client, request, response);
        return response;
    }

    public async Task<RestResponse> UpdateFilterByIdAsync(
        int? filterId = null,
        UpdateFilterRequest? body = null,
        string? name = null,
        string? description = null,
        string? type = null,
        List<FilterCondition>? conditions = null,
        List<FilterOrder>? orders = null)
    {
        if (body == null)
        {
            body = new UpdateFilterRequest
            {
                Name = name,
                Description = description,
                Type = type,
                Conditions = conditions ?? new List<FilterCondition>(),
                Orders = orders ?? new List<FilterOrder>()
            };
        }
            
        var request = new RestRequest(BuildUrl($"filter/{filterId}"), Method.Put).AddJsonBody(body);
        var response = await _client.ExecuteAsync(request);
        LoggerService.LogRestRequestResponse(_client, request, response);
        return response;
    }
    
    public async Task<RestResponse> DeleteFilterAsync(int filterId)
    {
        var request = new RestRequest(BuildUrl($"filter/{filterId}"), Method.Delete);
        var response = await _client.ExecuteAsync(request);
        LoggerService.LogRestRequestResponse(_client, request, response);
        return response;
    }
}
