using Microsoft.Playwright;
using PlaywrightDotNetApiAutomation.Models;

namespace PlaywrightDotNetApiAutomation.Api;

public class UsersApi
{
    private readonly ApiClient _client;

    public UsersApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<IAPIResponse> GetBooking(int id)
    {
        return await _client.GetAsync($"/booking/{id}");
    }

    public async Task<IAPIResponse> CreateBooking(BookingRequest request)
    {
        return await _client.PostAsync("/booking", request);
    }

    public async Task<IAPIResponse> UpdateBooking(int id, BookingRequest request)
    {
        return await _client.PutAsync($"/booking/{id}", request);
    }

    public async Task<IAPIResponse> DeleteBooking(int id)
    {
        return await _client.DeleteAsync($"/booking/{id}");
    }
}