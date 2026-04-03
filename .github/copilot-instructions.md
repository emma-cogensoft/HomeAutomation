# Copilot Instructions

## Build & Test Commands

```bash
# Restore, build, test (matches CI)
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal

# Run a single test class
dotnet test Tests/ --filter "FullyQualifiedName~BatteryStateFactoryTests"

# Run a single test method
dotnet test Tests/ --filter "FullyQualifiedName~BatteryStateFactoryTests.ShouldReturnFullState"

# Frontend (from HomeAutomation.Web/ClientApp/)
npm install
npm start       # dev server on port 44404 (HTTPS)
npm run build   # production build
npm test        # Karma/Jasmine frontend tests
```

## Architecture

This is a **layered DDD + CQRS** application for monitoring and controlling a home solar/battery inverter system.

```
Domain       → core models, value objects, factories (no dependencies)
Application  → MediatR query handlers, service interfaces
Web          → ASP.NET Core REST API + Angular 15 SPA
CloudInverter  → implements IInverterRealtimeDataReader via cloud API
LocalInverter  → implements IInverterRealtimeDataReader + IInverterSettingsDataReader via local API
MetOffice      → implements IWeatherForecastReader via Met Office weather API
Tests        → NUnit tests for Domain project only
```

**Dependency direction**: Web → Application → Domain. The three infrastructure projects (CloudInverter, LocalInverter, MetOffice) implement interfaces defined in Application and are registered in Web's DI container.

At runtime, **only one inverter project is wired up** (either Cloud or Local), controlled by DI registration in `Program.cs`.

## Key Conventions

### CQRS with MediatR — queries only
All API endpoints go through a MediatR query. There are currently no commands. A new feature requires:
1. A `Get[Feature]` record implementing `IRequest<TResponse>` in `Application/`
2. A `Get[Feature]Handler` implementing `IRequestHandler<Get[Feature], TResponse>` in the same folder
3. A controller action that sends the query via `_mediator.Send(...)`

### Domain value objects
`Percentage`, `Watt`, `WattHours`, and `TimeOfDay` all inherit from `ValueObject<T>` and provide structural equality. Use these types on domain models — never raw `int`/`double` for domain quantities.

### Battery state is derived, not stored
`BatteryStateFactory` creates a `BatteryState` subtype (`FullState`, `DrainedState`, `PartiallyFullState`) from a `Percentage`. `BatteryActivityFactory` creates a `ChargingActivity` or `DischargingActivity` from wattage. Both factories use switch expressions. `BatteryInfo` is a record that composes state + activity and is the aggregate passed to the Application layer.

### New external integrations
Follow the pattern in `MetOffice/` or `CloudInverter/`:
- `[Service]ApiOptions.cs` for configuration (bound via `IOptions<T>`)
- `I[Service]ApiAccessor` interface + implementation using `IHttpClientFactory`
- A reader class implementing the relevant Application interface
- `ServiceCollectionExtensions.cs` for DI registration
- Register the `HttpClient` using `AddHttpClient<T>()`

### Naming
- Interfaces: `IInverterRealtimeDataReader`, `IWeatherForecastReader`
- Handlers: `GetBatteryDataHandler`, `GetWeatherForecastHandler`
- API response DTOs: `BatteryResponse`, `WeatherResponse`, `InverterSettingsResponse`
- Options/config: `CloudInverterApiOptions`, `MetOfficeApiOptions`
- Custom exceptions: `CloudInverterException`, `LocalInverterApiException`

### Code style
- `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>` are on in all projects
- Prefer `record` and `init`-only properties for immutable domain types
- Use `required` on properties that must always be set
- Tests use `[TestCase(...)]` for parameterized cases and `Assert.Multiple()` for grouped assertions
- Test classes carry `[TestOf(nameof(ClassName))]`

## Project Structure Notes

- `HomeAutomation.Web/ClientApp/` — Angular SPA; built into `wwwroot/` on publish
- `HomeAutomation.Web/Controllers/` — thin controllers, one per domain concept
- `Tests/HomeAutomation.Domain.Tests/` — only domain logic is unit-tested; no Application/Web tests currently
- Battery capacity (5800 Wh) and state thresholds (10% minimum, 99% full) are constants in the Domain
