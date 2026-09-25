# TechMove

TechMove is an ASP.NET Core MVC application for managing clients, service contracts, and service requests. It stores its data in SQL Server and converts service-request costs from USD to ZAR using a live exchange rate.

## Features

- Create, view, update, and delete client records.
- Manage contracts associated with clients, including service level and contract dates.
- Upload signed contract agreements as PDF files.
- Track service requests associated with contracts, including status and cost.
- Convert USD costs to ZAR using the latest rate from ExchangeRate-API; the rate and fetch time are stored with each service request.

## Technology

- ASP.NET Core MVC, targeting .NET 8
- Entity Framework Core 8 with SQL Server
- xUnit tests; the test project targets .NET 9
- ExchangeRate-API for the USD-to-ZAR rate

## Prerequisites

- Windows
- .NET 8 SDK to build and run the application
- .NET 9 SDK to run the test project
- SQL Server LocalDB (included with common Visual Studio workloads), or another SQL Server instance
- Internet access for live exchange-rate requests

Check installed SDKs with:

```powershell
dotnet --list-sdks
```

## Run locally

From the repository root, restore the solution packages:

```powershell
dotnet restore TechMove.sln
```

The default connection string is configured in `TechMove/appsettings.json` and uses SQL Server LocalDB with a database named `TechMoveDb`. If you use this default, make sure LocalDB is installed and available.

Install the EF Core command-line tool if you do not already have it:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.0
```

Apply the existing migrations to create or update the database:

```powershell
dotnet ef database update --project TechMove/TechMove.csproj --startup-project TechMove/TechMove.csproj
```

Start the application:

```powershell
dotnet run --project TechMove/TechMove.csproj
```

Open the URL printed by `dotnet run`. The configured launch profiles use `https://localhost:7173` and `http://localhost:5232`. The default page is the Clients list.

## Configuration

### SQL Server connection

To use a different SQL Server instance, set the `ConnectionStrings:DefaultConnection` configuration value. For local development, .NET User Secrets keeps machine-specific connection details out of tracked settings files:

```powershell
dotnet user-secrets init --project TechMove/TechMove.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your SQL Server connection string>" --project TechMove/TechMove.csproj
```

After changing the connection string, run the database update command above against that database.

### Exchange rate

The exchange-rate endpoint is configured by `ExchangeRateApi:LatestUsdUrl` in `TechMove/appsettings.json`. It defaults to `https://api.exchangerate-api.com/v4/latest/USD`; no API key is configured by this project. The application needs network access to this endpoint when it fetches the current USD-to-ZAR rate.

### Uploaded agreements

Uploaded PDF agreements are saved under `TechMove/wwwroot/uploads`. Keep this directory writable by the application process. Uploaded files are stored on the local filesystem, so they are not automatically shared across application instances.

## Run tests

The test project targets .NET 9, so install the .NET 9 SDK before running:

```powershell
dotnet test TechMove.Tests/TechMove.Tests.csproj
```

The current tests cover currency conversion and PDF file validation.

## Database migrations

Migrations are in `TechMove/Migrations`. To add a migration after changing the EF Core models:

```powershell
dotnet ef migrations add <MigrationName> --project TechMove/TechMove.csproj --startup-project TechMove/TechMove.csproj
```

Then apply it with `dotnet ef database update` as shown above.

## Repository layout

```text
TechMove/          ASP.NET Core MVC application
TechMove.Tests/    xUnit test project
TechMove.sln       Visual Studio solution
```
