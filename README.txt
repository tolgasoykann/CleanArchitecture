Multi-Manager .NET Clean Architecture - Detailed README

This project follows a Clean Architecture and Onion Architecture structure and includes various modular and pluggable manager services 
for configuration, logging, database access, session handling, feature toggling, and proxy operations.

1. Feature Toggle Service

Overview:

FeatureToggleService dynamically reads configurations like DatabaseProvider and LogProvider from appsettings.json. 
This allows you to switch components (e.g., from SQL to Mongo or File logging to Composite logging) without touching code.

Configuration (appsettings.json):
{
  "DatabaseProvider": "SqlClient",
  "LogProvider": "Composite",
  "ConnectionStrings": {
    "SqlClient": "...",
    "MongoDb": "...",
    "Redis": "..."
  },
  "MongoDbSettings": {
    "Database": "MyDb",
    "UserCollection": "Users"
  },
  "RedisSettings": {
    "UserKeyPrefix": "user:"
  }
}

Usage:

var featureToggle = serviceProvider.GetRequiredService<IFeatureToggleService>();
var dbType = featureToggle.GetDatabaseProvider();

---------------------------------------------------------------------------------------------

2. DatabaseManager

Supported Backends:

SqlClient

MongoDB

Redis

Interface:

public interface IDatabaseManager
{
    Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters = null);
    Task<int> ExecuteAsync(string query, object parameters = null);
    Task<T> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null);
}

Example:

var users = await _databaseManager.QueryAsync<User>("SELECT * FROM Users");

Setup in Program.cs:

var dbType = featureToggle.GetDatabaseProvider();
services.AddDatabaseManager(dbType);

----------------------------------------------------------------------------------------------

3. LogManager

Types:

ConsoleLogManager

FileLogManager

CompositeLogManager (logs to multiple targets)

Interface:

public interface ILogManager
{
    void LogInfo(string message);
    void LogError(string message);
    void LogWarning(string message);
}

Example:

_logManager.LogInfo("Request received.");
_logManager.LogError("Something went wrong.");

Setup in Program.cs:

services.AddLoggingManager("composite");


----------------------------------------------------------------------------------------------

4. ConfigManager

Description:

Retrieves values from IConfiguration, optionally supports binding strongly-typed config classes.

Interface:

public interface IConfigManager
{
    T GetValue<T>(string key);
    T GetSection<T>(string sectionName);
}

Example:

var connectionString = _configManager.GetValue<string>("ConnectionStrings:SqlClient");
var redisSettings = _configManager.GetSection<RedisSettings>("RedisSettings");

-----------------------------------------------------------------

5. SessionManager

Types:

HttpContextSessionManager

RedisSessionManager

Interface:

public interface ISessionManager
{
    void Set<T>(string key, T value);
    T Get<T>(string key);
    void Remove(string key);
}

Example:

_sessionManager.Set("token", "abc123");
var token = _sessionManager.Get<string>("token");

Setup:

services.AddHttpContextSessionManager();
services.AddRedisSessionManager(configuration);

You can register only one of them based on your session preference.

-----------------------------------------------------------------------------------------------

6. ProxyManager

Description:

Handles HTTP proxying via HttpClient.

Interface:

public interface IProxyManager
{
    Task<T> GetAsync<T>(string url);
    Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data);
}

Example:

var response = await _proxyManager.GetAsync<MyResponse>("https://api.example.com/data");

Setup:

services.AddHttpClient<IProxyManager, ProxyManager>();

-----------------------------------------------------------------------------------------------

Program.cs Sample Setup

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Register feature toggle first
builder.Services.AddSingleton<IFeatureToggleService, FeatureToggleService>();
var tempProvider = builder.Services.BuildServiceProvider();
var featureToggle = tempProvider.GetRequiredService<IFeatureToggleService>();

// Use feature toggle values
builder.Services.AddDatabaseManager(featureToggle.GetDatabaseProvider());
builder.Services.AddLoggingManager(featureToggle.GetLogProvider());

// Other services
builder.Services.AddConfigManager();
builder.Services.AddHttpContextSessionManager(); // or AddRedisSessionManager
builder.Services.AddProxyManager();
builder.Services.AddSingleton<ICustomJsonSerializer, CustomJsonSerializer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseAuthorization();
app.MapControllers();
app.Run();

----------------------------------------------------------------------------------------------

Conclusion

This modular, extensible architecture provides full flexibility via feature toggles and multiple manager implementations.
You can swap out database engines, logging targets, or session strategies without touching any application logic—just update the configuration.

