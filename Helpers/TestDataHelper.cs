using PlaywrightDotNetApiAutomation.Models;

namespace PlaywrightDotNetApiAutomation.Helpers;

public static class TestDataHelper
{
    public static BookingRequest BuildBookingRequest(
        string? firstName = null,
        string? lastName = null,
        int? totalPrice = null,
        bool? depositPaid = null,
        string? checkIn = null,
        string? checkOut = null,
        string? additionalNeeds = null)
    {
        return new BookingRequest
        {
            firstname = firstName ?? $"Test-{Guid.NewGuid():N}",
            lastname = lastName ?? "Automation",
            totalprice = totalPrice ?? 150,
            depositpaid = depositPaid ?? true,
            bookingdates = new BookingDates
            {
                checkin = checkIn ?? "2026-09-10",
                checkout = checkOut ?? "2026-09-15"
            },
            additionalneeds = additionalNeeds ?? "Breakfast"
        };
    }
}