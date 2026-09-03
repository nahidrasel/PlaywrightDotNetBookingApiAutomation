namespace PlaywrightDotNetApiAutomation.Models;

public sealed record ApiResponse<T>
{
    public T? Data { get; init; }
}
