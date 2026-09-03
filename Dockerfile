FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet test --logger "trx;LogFileName=test-results.trx" --results-directory TestResults

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS final
WORKDIR /app
COPY --from=build /src /app
CMD ["dotnet", "test", "--logger", "trx;LogFileName=test-results.trx", "--results-directory", "TestResults"]
