using System.Text.Json;

namespace PlaywrightDotNetBookingApiAutomation.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}
