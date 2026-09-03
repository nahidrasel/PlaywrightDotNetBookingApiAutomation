# PlaywrightDotNetApiAutomation

C# Playwright API automation framework for validating REST endpoints, checking status codes, parsing JSON responses, and running tests in CI.

## Project structure

- `Api/ApiClient.cs` - low-level HTTP wrapper around `IAPIRequestContext`
- `Api/UsersApi.cs` - endpoint-specific API methods
- `Config/AppSettings.cs` - environment and config-based settings loader
- `Config/appsettings.json` - default application config
- `Fixtures/BaseTest.cs` - shared test setup and teardown
- `Helpers/JsonHelper.cs` - JSON deserialization helper
- `Helpers/TestDataHelper.cs` - dynamic test data generation
- `Models/` - typed API request/response models
- `Tests/UserTests.cs` - API tests using NUnit + FluentAssertions

## Features

- Playwright-based API request context
- Layered architecture for API clients and tests
- Generic response models using `ApiResponse<T>`
- Config-based settings with environment override support
- GitHub Actions CI workflow with test reporting
- Parallel NUnit execution
- Scheduled and PR-triggered runs

## Local setup

1. Restore dependencies:

   ```bash
   dotnet restore
   ```

2. Run tests locally:

   ```bash
   dotnet test --nologo
   ```

## Configuration

The project reads default settings from `Config/appsettings.json`.

Example:

```json
{
  "ApiBaseUrl": "https://reqres.in",
  "Username": "",
  "Password": "",
  "TimeoutMs": 30000
}
```

For CI or secret-based auth, set GitHub Actions secrets such as:

- `APP_USERNAME`
- `APP_PASSWORD`

These values are loaded from environment variables when present.

## CI/CD

The workflow in `.github/workflows/dotnet-api-tests.yml` runs on:

- push to `main` and `development`
- pull requests to `main` and `development`
- nightly cron schedule
- manual workflow dispatch

It also publishes a test report using `dorny/test-reporter`.

## Example test flow

```csharp
var response = await _userApi.GetUser(2);
response.Status.Should().Be(200);

var json = await response.TextAsync();
var result = JsonHelper.Deserialize<ApiResponse<UserResponse>>(json);

result.Should().NotBeNull();
result!.Data.Should().NotBeNull();
result.Data!.Id.Should().Be(2);
```

## Setup, teardown, and request context pattern

The GitHub API examples typically create test data in `beforeAll` and remove it in `afterAll`. In this project, the same pattern is handled in the shared fixture base class so every test gets a fresh `APIRequestContext`.

```csharp
public abstract class BaseTest
{
    protected IPlaywright Playwright = null!;
    protected IAPIRequestContext ApiContext = null!;

    [SetUp]
    public async Task Setup()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var headers = new Dictionary<string, string>
        {
            ["Accept"] = "application/json"
        };

        if (!string.IsNullOrWhiteSpace(AppSettings.Username)
            && !string.IsNullOrWhiteSpace(AppSettings.Password))
        {
            var basicAuth = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{AppSettings.Username}:{AppSettings.Password}"));

            headers["Authorization"] = $"Basic {basicAuth}";
        }

        ApiContext = await Playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = AppSettings.ApiBaseUrl,
                ExtraHTTPHeaders = headers,
                Timeout = AppSettings.TimeoutMs
            });
    }

    [TearDown]
    public async Task Teardown()
    {
        if (ApiContext is not null)
        {
            await ApiContext.DisposeAsync();
        }

        Playwright?.Dispose();
    }
}
```

This is the same idea as using a `request` fixture in Playwright JavaScript: create the API context once, send requests to the API, and make sure it is disposed during teardown for clean test isolation.

For repository-style tests, the lifecycle is usually:

1. Create test data before the suite starts
2. Run the tests against that data
3. Clean up after the suite finishes

This is the recommended pattern when the API under test is stateful, such as GitHub repositories, issues, or users.

## Notes

- The URL remains in repo config and is safe to keep in source control.
- Credentials should be stored as GitHub secrets instead of being committed.
- The suite uses NUnit parallel execution to speed up local and CI execution where tests are independent.
