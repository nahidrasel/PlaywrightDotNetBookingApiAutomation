# Playwright .NET Booking API Automation

A C# API automation sample built with Microsoft Playwright, xUnit, FluentAssertions, and Testcontainers. It demonstrates API client abstractions, typed request/response models, reusable test setup, and booking-oriented test data. The repository also includes Docker configuration for the optional local test service.

## Prerequisites

- .NET SDK 10.0
- Git
- Docker Desktop or a Docker Engine installation if you choose to run container-based tests
- Network access to the configured API under test when using the hosted service

## Run the tests

From the repository root:

```bash
dotnet restore
dotnet test
```

The GitHub Actions workflow runs the suite on Ubuntu and Windows, publishes TRX results, and can rerun failed tests once. The CI configuration currently sets `APP_USE_TESTCONTAINERS=false` and targets the hosted Restful Booker demo API.

## Project structure

```text
Api/          API client and endpoint wrappers
Config/       app settings and typed configuration
Fixtures/     shared test setup
Helpers/      JSON helpers
Models/       request and response models
TestData/     booking-related test data
Tests/        xUnit API tests
Dockerfile    optional container image definition
docker-compose.yml  optional local service configuration
```

## Configuration and credentials

Review `Config/appsettings.json` and `Config/AppSettings.cs` before changing the target environment. CI supplies API configuration through environment variables, including `APP_API_BASE_URL`, `APP_USERNAME`, `APP_PASSWORD`, and `APP_TIMEOUTMS`.

The tracked username/password values and the workflow fallback values are public demo-only credentials for the hosted sample service. They are not secrets, must not be reused for real systems, and must be replaced with repository secrets for a private test environment. Never commit production credentials, tokens, customer data, or private environment configuration. The repository's ignore rules exclude common local environment and build outputs, but ignore rules do not remove files that have already been committed.

## Notes

The hosted demo API is external and may be rate-limited, unavailable, or change independently of this repository. Treat failures against that service separately from build failures. Container-based testing is optional and is disabled in the current CI workflow.
