using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightDotNetBookingApiAutomation.Fixtures;

public abstract class BaseTest
{
    protected IPlaywright Playwright = null!;
    protected IAPIRequestContext ApiContext = null!;

    [SetUp]
    public async Task Setup()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        ApiContext = await Playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = "https://reqres.in",
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                },
                Timeout = 30000
            });
    }
    [TearDown]
    public async Task Teardown()
    {
        await ApiContext.DisposeAsync();
        Playwright.Dispose();
    }
}