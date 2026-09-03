using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Playwright;
using PlaywrightDotNetBookingApiAutomation.Config;
using PlaywrightDotNetBookingApiAutomation.Helpers;
using PlaywrightDotNetBookingApiAutomation.Models;

namespace PlaywrightDotNetBookingApiAutomation.Fixtures;

public abstract class BaseTest : IAsyncLifetime
{
    protected IPlaywright Playwright = null!;
    protected IAPIRequestContext ApiContext = null!;
    private IContainer? _bookingContainer;

    public async Task InitializeAsync()
    {
        var baseUrl = AppSettings.ApiBaseUrl;

        if (AppSettings.UseTestcontainers)
        {
            var container = new ContainerBuilder()
                .WithImage(AppSettings.TestcontainersImage)
                .WithPortBinding(3000, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPort(3000).ForPath("/api/health")))
                .Build();

            await container.StartAsync();
            _bookingContainer = container;
            baseUrl = $"http://localhost:{container.GetMappedPublicPort(3000)}";
        }

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var headers = new Dictionary<string, string>
        {
            ["Accept"] = "application/json"
        };

        if (!string.IsNullOrWhiteSpace(AppSettings.Username) && !string.IsNullOrWhiteSpace(AppSettings.Password))
        {
            var basicAuth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{AppSettings.Username}:{AppSettings.Password}"));
            headers["Authorization"] = $"Basic {basicAuth}";
        }

        ApiContext = await Playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = baseUrl,
                ExtraHTTPHeaders = headers,
                Timeout = AppSettings.TimeoutMs
            });

        var token = await AuthenticateAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            headers["Cookie"] = $"token={token}";
            await ApiContext.DisposeAsync();
            ApiContext = await Playwright.APIRequest.NewContextAsync(
                new APIRequestNewContextOptions
                {
                    BaseURL = baseUrl,
                    ExtraHTTPHeaders = headers,
                    Timeout = AppSettings.TimeoutMs
                });
        }

        await InitializeTestDataAsync();
    }

    protected virtual Task InitializeTestDataAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (ApiContext is not null)
        {
            await ApiContext.DisposeAsync();
        }

        if (_bookingContainer is not null)
        {
            await _bookingContainer.DisposeAsync();
        }

        Playwright?.Dispose();
    }

    private async Task<string?> AuthenticateAsync()
    {
        if (string.IsNullOrWhiteSpace(AppSettings.Username) || string.IsNullOrWhiteSpace(AppSettings.Password))
        {
            return null;
        }

        var response = await ApiContext.PostAsync("/auth", new APIRequestContextOptions
        {
            DataObject = new AuthRequest
            {
                username = AppSettings.Username,
                password = AppSettings.Password
            }
        });

        if (response.Status != 200)
        {
            return null;
        }

        var json = await response.TextAsync();
        var auth = JsonHelper.Deserialize<AuthResponse>(json);
        return auth?.token;
    }
}