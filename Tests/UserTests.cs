using FluentAssertions;
using NUnit.Framework;
using PlaywrightDotNetBookingApiAutomation.Fixtures;

namespace PlaywrightDotNetBookingApiAutomation.Tests;

[TestFixture]
public class UsersTests : BaseTest
{
    [Test]
    public async Task GetUser_ShouldReturn200()
    {
        var response = await ApiContext.GetAsync("/api/users/2");

        response.Status.Should().Be(200);
    }
}