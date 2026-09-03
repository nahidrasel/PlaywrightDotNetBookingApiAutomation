using System.Text.Json;

namespace PlaywrightDotNetBookingApiAutomation.Config;

public static class AppSettings
{
    private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "Config", "appsettings.json");

    private static readonly Lazy<Dictionary<string, string>> Settings = new(() =>
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        void AddValue(string? key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            values[CanonicalizeKey(key)] = value.Trim();
        }

        if (File.Exists(ConfigPath))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(ConfigPath));
            foreach (var property in document.RootElement.EnumerateObject())
            {
                AddValue(property.Name, property.Value.ToString());
            }
        }

        foreach (var kvp in Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>())
        {
            AddValue(kvp.Key?.ToString(), kvp.Value?.ToString());
        }

        return values;
    });

    public static string ApiBaseUrl => GetValue("ApiBaseUrl");
    public static string Username => GetValue("Username");
    public static string Password => GetValue("Password");
    public static int TimeoutMs => ParseInt("TimeoutMs");
    public static bool UseTestcontainers => ParseBool("UseTestcontainers");
    public static string TestcontainersImage => GetValue("TestcontainersImage");

    private static string GetValue(string key)
    {
        var canonicalKey = CanonicalizeKey(key);
        var settings = Settings.Value;

        if (settings.TryGetValue(canonicalKey, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"Required configuration value '{key}' was not found in appsettings.json or environment variables.");
    }

    private static int ParseInt(string key)
    {
        var value = GetValue(key);
        return int.TryParse(value, out var parsedValue)
            ? parsedValue
            : throw new InvalidOperationException($"Configuration value '{key}' must be a valid integer.");
    }

    private static bool ParseBool(string key)
    {
        var value = GetValue(key);
        return bool.TryParse(value, out var parsedValue)
            ? parsedValue
            : throw new InvalidOperationException($"Configuration value '{key}' must be true or false.");
    }

    private static string CanonicalizeKey(string key)
    {
        var normalized = key.Trim();

        if (normalized.StartsWith("APP_", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[4..];
        }

        normalized = new string(normalized.Where(char.IsLetterOrDigit).ToArray());

        return normalized.ToLowerInvariant();
    }
}
