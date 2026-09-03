using System.Text.Json.Serialization;

namespace PlaywrightDotNetApiAutomation.Models;

public sealed record UserResponse
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; init; } = string.Empty;

    public string Avatar { get; init; } = string.Empty;
}