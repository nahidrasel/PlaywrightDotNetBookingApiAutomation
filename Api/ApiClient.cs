using Microsoft.Playwright;

namespace PlaywrightDotNetBookingApiAutomation.Api;

public class ApiClient
{
    private readonly IAPIRequestContext _context;

    public ApiClient(IAPIRequestContext context)
    {
        _context = context;
    }

    public async Task<IAPIResponse> GetAsync(string endpoint)
    {
        return await _context.GetAsync(endpoint);
    }

    public async Task<IAPIResponse> GetAsync(string endpoint, Dictionary<string, string> queryParams)
    {
        var url = QueryBuilder.Build(endpoint, queryParams);
        return await _context.GetAsync(url);
    }

    public async Task<IAPIResponse> PostAsync(string endpoint, object request)
    {
        return await _context.PostAsync(endpoint, new APIRequestContextOptions
        {
            DataObject = request
        });
    }

    public async Task<IAPIResponse> PutAsync(string endpoint, object request)
    {
        return await _context.PutAsync(endpoint, new APIRequestContextOptions
        {
            DataObject = request
        });
    }

    public async Task<IAPIResponse> DeleteAsync(string endpoint)
    {
        return await _context.DeleteAsync(endpoint);
    }
}

public static class QueryBuilder
{
    public static string Build(string endpoint, Dictionary<string, string> queryParams)
    {
        if (queryParams.Count == 0)
        {
            return endpoint;
        }

        var queryString = string.Join("&", queryParams
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return endpoint.Contains('?')
            ? $"{endpoint}&{queryString}"
            : $"{endpoint}?{queryString}";
    }
}
