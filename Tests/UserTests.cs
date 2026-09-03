using FluentAssertions;
using Microsoft.Playwright;
using PlaywrightDotNetBookingApiAutomation.Api;
using PlaywrightDotNetBookingApiAutomation.Fixtures;
using PlaywrightDotNetBookingApiAutomation.Helpers;
using PlaywrightDotNetBookingApiAutomation.Models;

namespace PlaywrightDotNetBookingApiAutomation.Tests;

public class UsersTests : BaseTest
{
    private ApiClient _client = null!;
    private UsersApi _userApi = null!;

    protected override Task InitializeTestDataAsync()
    {
        _client = new ApiClient(ApiContext);
        _userApi = new UsersApi(_client);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetBooking_ShouldReturnCorrectBooking()
    {
        var bookingId = 1;
        var response = await _userApi.GetBooking(bookingId);

        response.Status.Should().Be(200, "Booking API should return 200 for an existing booking");

        var json = await response.TextAsync();
        var result = JsonHelper.Deserialize<BookingResponse>(json);

        result.Should().NotBeNull();
        result!.booking.Should().NotBeNull();
        result.bookingid.Should().Be(bookingId);
        result.booking.firstname.Should().NotBeNullOrWhiteSpace();
        result.booking.lastname.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetBooking_WithInvalidId_ShouldReturn404()
    {
        var response = await _client.GetAsync("/booking/999999");
        response.Status.Should().Be(404, "Invalid booking id should return 404");
    }

    [Fact]
    public async Task CreateBooking_ShouldReturn200()
    {
        var request = TestDataHelper.BuildBookingRequest();
        var response = await _userApi.CreateBooking(request);

        response.Status.Should().Be(200, "the Booking API should create a new booking successfully");

        var result = await ResponseAssertions.ReadJsonAsync<BookingCreatedResponse>(response);

        result.Should().NotBeNull();
        result!.bookingid.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateBooking_ShouldReturn200()
    {
        var bookingId = 1;
        var request = TestDataHelper.BuildBookingRequest(firstName: "Updated", lastName: "Guest", totalPrice: 250, additionalNeeds: "Dinner");
        var response = await _userApi.UpdateBooking(bookingId, request);

        response.Status.Should().Be(200, "the Booking API should update the booking successfully");

        var result = await ResponseAssertions.ReadJsonAsync<BookingResponse>(response);

        result.Should().NotBeNull();
        result!.booking.firstname.Should().Be(request.firstname);
        result.booking.lastname.Should().Be(request.lastname);
        result.booking.totalprice.Should().Be(request.totalprice);
    }

    [Fact]
    public async Task DeleteBooking_ShouldReturn201()
    {
        var response = await _userApi.DeleteBooking(1);

        response.Status.Should().Be(201, "the Booking API should delete the booking successfully");
    }
}

public static class ResponseAssertions
{
    public static async Task<T?> ReadJsonAsync<T>(IAPIResponse response)
    {
        var json = await response.TextAsync();
        return JsonHelper.Deserialize<T>(json);
    }
}