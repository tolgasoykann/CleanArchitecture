using Api.Extensions.Config;
using Api.Extensions.Proxy;
using Api.Extensions.Resilience;
using Api.Extensions.Session;
using Domain.Interfaces;
using Infrastructure.Middleware;
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
//builder.Services.AddLoggingManager("console");
//builder.Services.AddLoggingManager("file");
builder.Services.AddLoggingManager("composite");
builder.Services.AddHttpContextSessionManager();
builder.Services.AddResilienceServiceRegistration();
builder.Services.AddRedisSessionManager(builder.Configuration);
builder.Services.AddSingleton<ICustomJsonSerializer, CustomJsonSerializer>();
builder.Services.AddSingleton<IFeatureToggleService, FeatureToggleService>();
builder.Services.AddSingleton<ISessionContextAccessor, SessionContextAccessor>();
builder.Services.AddSingleton<ILogManager, TraceIdAwareLogManager>();



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

app.UseMiddleware<TraceIdMiddleware>();


app.UseSession();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();