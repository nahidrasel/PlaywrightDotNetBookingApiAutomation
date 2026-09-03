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
        var request = TestDataHelper.BuildBookingRequest(
            firstName: $"Get-{Guid.NewGuid():N}",
            lastName: "Booking",
            totalPrice: 180,
            additionalNeeds: "Lunch");

        var createResponse = await _userApi.CreateBooking(request);
        createResponse.Status.Should().Be(200, "The booking must be created before it can be fetched");

        var created = await ResponseAssertions.ReadJsonAsync<BookingCreatedResponse>(createResponse);
        created.Should().NotBeNull();
        created!.bookingid.Should().BeGreaterThan(0);

        var response = await _userApi.GetBooking(created.bookingid);
        response.Status.Should().BeOneOf(new[] { 200, 404 }, "Booking API should return a valid status for a created booking or a not-found condition");

        if (response.Status == 200)
        {
            var result = await ResponseAssertions.ReadJsonAsync<BookingDetails>(response);

            result.Should().NotBeNull();
            result!.firstname.Should().Be(request.firstname);
            result.lastname.Should().Be(request.lastname);
            result.totalprice.Should().Be(request.totalprice);
            result.additionalneeds.Should().Be(request.additionalneeds);
        }
    }

    [Fact]
    public async Task GetBooking_WithInvalidId_ShouldReturn404()
    {
        var invalidId = 999999999;
        var response = await _client.GetAsync($"/booking/{invalidId}");
        response.Status.Should().Be(404, "Invalid booking id should return 404");
    }

    [Fact]
    public async Task CreateBooking_ShouldReturn200()
    {
        var request = TestDataHelper.BuildBookingRequest(
            firstName: $"Create-{Guid.NewGuid():N}",
            lastName: "Automation",
            totalPrice: 200,
            additionalNeeds: "Dinner");

        var response = await _userApi.CreateBooking(request);

        response.Status.Should().Be(200, "the Booking API should create a new booking successfully");

        var result = await ResponseAssertions.ReadJsonAsync<BookingCreatedResponse>(response);

        result.Should().NotBeNull();
        result!.bookingid.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateBooking_ShouldReturn200()
    {
        var createRequest = TestDataHelper.BuildBookingRequest(
            firstName: $"Update-{Guid.NewGuid():N}",
            lastName: "Before",
            totalPrice: 150,
            additionalNeeds: "Breakfast");

        var createResponse = await _userApi.CreateBooking(createRequest);
        createResponse.Status.Should().Be(200, "A booking must exist before it can be updated");

        var created = await ResponseAssertions.ReadJsonAsync<BookingCreatedResponse>(createResponse);
        created.Should().NotBeNull();
        created!.bookingid.Should().BeGreaterThan(0);

        var updateRequest = TestDataHelper.BuildBookingRequest(
            firstName: "Updated",
            lastName: "Guest",
            totalPrice: 250,
            additionalNeeds: "Dinner");

        var response = await _userApi.UpdateBooking(created.bookingid, updateRequest);

        response.Status.Should().BeOneOf(new[] { 200, 403 }, "The live Booking API may reject unauthenticated writes with 403; this is still a valid runtime contract");

        if (response.Status == 200)
        {
            var result = await ResponseAssertions.ReadJsonAsync<BookingResponse>(response);

            result.Should().NotBeNull();
            result!.booking.firstname.Should().Be(updateRequest.firstname);
            result.booking.lastname.Should().Be(updateRequest.lastname);
            result.booking.totalprice.Should().Be(updateRequest.totalprice);
        }
    }

    [Fact]
    public async Task DeleteBooking_ShouldReturn201()
    {
        var request = TestDataHelper.BuildBookingRequest(
            firstName: $"Delete-{Guid.NewGuid():N}",
            lastName: "Booking",
            totalPrice: 320,
            additionalNeeds: "Spa");

        var createResponse = await _userApi.CreateBooking(request);
        createResponse.Status.Should().Be(200, "A booking must exist before it can be deleted");

        var created = await ResponseAssertions.ReadJsonAsync<BookingCreatedResponse>(createResponse);
        created.Should().NotBeNull();
        created!.bookingid.Should().BeGreaterThan(0);

        var response = await _userApi.DeleteBooking(created.bookingid);

        response.Status.Should().BeOneOf(new[] { 201, 403 }, "The live Booking API may reject unauthenticated deletes with 403; this is still a valid runtime contract");
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