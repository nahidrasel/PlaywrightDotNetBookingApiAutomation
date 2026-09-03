using PlaywrightDotNetBookingApiAutomation.Models;

namespace PlaywrightDotNetBookingApiAutomation.TestData;

public static class BookingTestData
{
    public static BookingRequest CreateBooking(
        string? firstName = null,
        string? lastName = null,
        int? totalPrice = null,
        bool? depositPaid = null,
        string? checkIn = null,
        string? checkOut = null,
        string? additionalNeeds = null)
    {
        var bookingDate = DateTime.UtcNow.Date.AddDays(7);

        return new BookingRequest
        {
            firstname = firstName ?? $"Test-{Guid.NewGuid():N}",
            lastname = lastName ?? "Automation",
            totalprice = totalPrice ?? 150,
            depositpaid = depositPaid ?? true,
            bookingdates = new BookingDates
            {
                checkin = checkIn ?? bookingDate.ToString("yyyy-MM-dd"),
                checkout = checkOut ?? bookingDate.AddDays(5).ToString("yyyy-MM-dd")
            },
            additionalneeds = additionalNeeds ?? "Breakfast"
        };
    }
}