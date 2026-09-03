namespace PlaywrightDotNetBookingApiAutomation.Models;

public sealed record AuthRequest
{
    public string username { get; init; } = string.Empty;
    public string password { get; init; } = string.Empty;
}

public sealed record AuthResponse
{
    public string token { get; init; } = string.Empty;
}

public sealed record BookingCreatedResponse
{
    public int bookingid { get; init; }
}

public sealed record BookingResponse
{
    public int bookingid { get; init; }
    public BookingDetails booking { get; init; } = new();
}

public sealed record BookingDetails
{
    public string firstname { get; init; } = string.Empty;
    public string lastname { get; init; } = string.Empty;
    public int totalprice { get; init; }
    public bool depositpaid { get; init; }
    public BookingDates bookingdates { get; init; } = new();
    public string additionalneeds { get; init; } = string.Empty;
}
