# Multi-Manager .NET — Clean & Onion Architecture (User-facing README)

## Overview

This repository is a reusable, production-minded foundation that combines **Clean Architecture** and **Onion Architecture** principles
with a collection of modular, pluggable "manager" services. The project is built so teams can copy the managers they need into new applications
and keep business logic free from infrastructure concerns.

The README below explains what each manager does, which responsibilities it covers, and how you (as a consumer of the library) would typically use them in an application.

---

## Quick summary of responsibilities

- **FeatureToggleService** — centralizes runtime choices (which database, which logging provider, etc.) so deployments can change behavior via configuration.
- **DatabaseManager** — unified data access abstraction over multiple backends (SQL, MongoDB, Redis) exposing a consistent API for queries and commands.
- **LogManager** — structured, TraceId-aware logging with multiple sink options (console, file, composite/multi-sink).
- **ConfigManager** — a thin wrapper over configuration for safe, typed access to settings.
- **SessionManager** — session storage abstraction with pluggable backends (HttpContext or Redis) for per-request or distributed session state.
- **ProxyManager** — typed HTTP proxying and helper utilities built on top of `HttpClient` to call external services reliably and consistently.
- **CustomJsonSerializer** — single serialization abstraction used across managers and app code to keep serialization behavior consistent.
- **Health checks & Middleware** — lightweight readiness checks and a TraceId middleware that enables request correlation across logs and services.

---

## Managers — detailed descriptions and examples

### FeatureToggleService

**Purpose:** Provide a single authoritative place to read runtime feature toggles and environment-driven choices
(for example: which database provider or logging provider to use).

**Why it exists:** Instead of scattering `IConfiguration` reads across startup code, the `FeatureToggleService` centralizes decision-making. 
This makes startup behavior consistent and easier to reason about.

**Common values:** `DatabaseProvider`, `LogProvider`, `RedisSettings`, `MongoDbSettings`, etc.

**Typical usage:**

```csharp
var featureToggle = serviceProvider.GetRequiredService<IFeatureToggleService>();
var dbType = featureToggle.GetDatabaseProvider();
```

### DatabaseManager

**Purpose:** Offer a single, consistent API for data access regardless of the chosen backend.

**Primary responsibilities:**

- Querying data (e.g. `QueryAsync<T>`)
- Executing commands (`ExecuteAsync`)
- Invoking stored procedures (`ExecuteStoredProcedureAsync<T>`)

**Supported backends:**

- `SqlClient` (relational databases)
- `MongoDB` (document store)
- `Redis` (key-value operations; typically for caching or simple data access patterns)

**When to use:** Use `IDatabaseManager` from application services and controllers to perform data access without coupling to a specific database technology.

**Example:**

```csharp
var users = await _databaseManager.QueryAsync<User>("SELECT * FROM Users");
await _databaseManager.ExecuteAsync("UPDATE Orders SET Status = @status WHERE Id = @id", new { status = "Sent", id = 123 });
```

### LogManager

**Purpose:** Provide structured, consistent logging across the application with support for multiple sinks and request correlation.

**Capabilities:**

- `ConsoleLogManager` — simple stdout logging for development and containers.
- `FileLogManager` — persistent file-based logging for local or host file system sinks.
- `CompositeLogManager` — fan-out logging that writes to multiple sinks (for example: console + file).

**TraceId support:** Loggers are TraceId-aware (via middleware). This allows a unique trace identifier to be included in every log line 
so you can correlate a request flow across services and logs.

**Typical use:** Any service or controller that needs to record operational events or errors uses `ILogManager`.

**Example:**

```csharp
_logManager.LogInfo("User created: {userId}");
_logManager.LogError("Payment failed", ex);
```

### ConfigManager

**Purpose:** A small abstraction over `IConfiguration` that provides typed retrieval helpers and consistent access patterns for configuration values.

**Common methods:**

- `GetValue<T>(string key)` — read a simple value
- `GetSection<T>(string sectionName)` — bind a configuration section to a typed object

**Why use it:** Keeps configuration access consistent and makes it explicit when code depends on configuration values.

### SessionManager

**Purpose:** Provide session storage primitives with interchangeable backends so that code can store per-user or per-request state without depending on ASP.NET internals.

**Common backends:**

- `HttpContextSessionManager`: stores session in the current `HttpContext` (suitable for single-host apps)
- `RedisSessionManager`: stores session data in Redis for distributed scenarios (suitable for load-balanced or multi-instance deployments)

**Primary responsibilities:**

- `Set<T>(string key, T value)` — store a typed value
- `T Get<T>(string key)` — retrieve a typed value
- `Remove(string key)` — remove a key

**Where to use:** Authentication tokens, short-lived user preferences, request-scoped state that needs to persist across requests for the same user.

**Example:**

```csharp
_sessionManager.Set("cart", cartDto);
var cart = _sessionManager.Get<CartDto>("cart");
```

### ProxyManager

**Purpose:** Facilitate HTTP communication with external APIs using typed requests/responses and centralized behaviors.

**What it helps with:**

- Creating typed wrappers for external API endpoints (e.g., `GetUserProfileAsync(userId)`).
- Centralizing common concerns: base addresses, default headers (API keys), timeouts, retry and transient-fault handling, logging of outgoing requests, deserialization.
- Using `IHttpClientFactory` under the hood to get pooled, well-configured `HttpClient` instances.

**Common capabilities:**

- `GetAsync<T>(string url)` — perform a GET and deserialize to `T`
- `PostAsync<TRequest,TResponse>(string url, TRequest data)` — POST and deserialize response
- Support for custom headers and cancellation tokens

**How teams use it:** Create a typed wrapper service that calls `IProxyManager` methods to perform external requests; the wrapper encapsulates the contract of the 
external system while `IProxyManager` handles transport concerns.

**Example:**

```csharp
var user = await _proxyManager.GetAsync<UserDto>("https://api.example.com/users/42");
var result = await _proxyManager.PostAsync<CreateOrderRequest, CreateOrderResponse>("/orders", orderRequest);
```

### CustomJsonSerializer

**Purpose:** Ensure that serialization and deserialization behavior is consistent across all managers and application code.

**Why it exists:** Centralizes settings like camel-case naming, custom converters, date handling, and null-handling so every component serializes objects the same way.

**Typical interface methods:** `Serialize<T>(T value)`, `Deserialize<T>(string json)`.

---

## Middleware & Health checks

- **TraceIdMiddleware**: Adds a `TraceId` to each incoming request and puts it in a scoped request context so loggers can include the ID for correlation.
- **Health checks**: Managers expose health-check methods that can be invoked at startup or periodically to confirm connectivity to downstream systems
(databases, Redis, external endpoints).

These facilities improve operability and make it easier to monitor application readiness and troubleshoot issues.

---

## Example usage snippets (consumer perspective)

**Logging:**

```csharp
// inside a controller or service
_logManager.LogInfo("Payment processing started");
try {
  // ... do work
} catch(Exception ex) {
  _logManager.LogError("Payment failed", ex);
}
```

**Session:**

```csharp
_sessionManager.Set("cart", cartDto);
var cart = _sessionManager.Get<CartDto>("cart");
```

**Proxy:**

```csharp
var weather = await _proxyManager.GetAsync<WeatherDto>("https://api.weather.com/v3/wx/conditions/current");
```

**Database:**

```csharp
var orders = await _databaseManager.QueryAsync<Order>("SELECT * FROM Orders WHERE UserId=@userId", new { userId = 42 });
```

---

Consume the managers from the **Application** and **API** layers. Keep domain models and domain interfaces free from infrastructure code; 
rely on these manager abstractions when you need cross-cutting or infrastructure features (data access, logging, sessions, external calls).

---
PROGRAM.CS EXAMPLE:

using Api.Extensions.Config;
using Api.Extensions.Proxy;
using Api.Extensions.Resilience;
using Api.Extensions.Session;
using Domain.Interfaces;
using Infrastructure.Services.Database;
using Infrastructure.Services.HealthCheck;
using Infrastructure.Services.JsonSerializer;
using Infrastructure.Services.Proxy;
using Infrastructure.Services.Session;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Custom service registrations
builder.Services.AddProxyManager();
builder.Services.AddConfigManager();
builder.Services.AddSingleton<IFeatureToggleService, FeatureToggleService>();
var tempProvider = builder.Services.BuildServiceProvider();
var featureToggle = tempProvider.GetRequiredService<IFeatureToggleService>();
var dbType = featureToggle.GetDatabaseProvider();
builder.Services.AddDatabaseManager(dbType);
builder.Services.AddSingleton<IHealthCheckable, DatabaseManager>();
//builder.Services.AddLoggingManager("console");
//builder.Services.AddLoggingManager("file");
builder.Services.AddLoggingManager("composite");
builder.Services.AddHttpContextSessionManager();
builder.Services.AddResilienceServiceRegistration();
builder.Services.AddSingleton<ICustomJsonSerializer, CustomJsonSerializer>();
builder.Services.AddSingleton<IFeatureToggleService, FeatureToggleService>();
builder.Services.AddSingleton<ISessionContextAccessor, SessionContextAccessor>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", opt =>
    {
        opt.PermitLimit = 10; // Örn: her 10 saniyede en fazla 10 istek
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });
});

// HttpClient + Proxy
builder.Services.AddHttpClient<IProxyManager, ProxyManager>();

// Redis Cache for session
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddRedisSessionManager(builder.Configuration);
builder.Services.AddSingleton<IHealthCheckable, RedisDatabaseManager>();

// Session middleware config
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MyApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

await HealthCheckStartup.CheckAllManagersHealthAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
