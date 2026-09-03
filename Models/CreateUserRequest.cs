namespace PlaywrightDotNetBookingApiAutomation.Models;

public sealed record BookingRequest
{
    public string firstname { get; init; } = string.Empty;
    public string lastname { get; init; } = string.Empty;
    public int totalprice { get; init; }
    public bool depositpaid { get; init; }
    public BookingDates bookingdates { get; init; } = new();
    public string additionalneeds { get; init; } = string.Empty;
}

public sealed record BookingDates
{
    public string checkin { get; init; } = string.Empty;
    public string checkout { get; init; } = string.Empty;
}