using FluentAssertions;
using PlaywrightDotNetApiAutomation.Fixtures;
using NUnit.Framework;

namespace PlaywrightDotNetApiAutomation.Tests;

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