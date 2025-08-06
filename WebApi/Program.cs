using Api.Extensions.Config;
using Api.Extensions.Logging;
using Api.Extensions.Proxy;
using Api.Extensions.Resilience;
using Api.Extensions.Session;
using Domain.Interfaces;
using Infrastructure.Services.JsonSerializer;
using Infrastructure.Services.Proxy;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Custom service registrations
builder.Services.AddProxyManager();
builder.Services.AddConfigManager();
builder.Services.AddLoggingManager();
builder.Services.AddHttpContextSessionManager();
builder.Services.AddResilienceServiceRegistration();
builder.Services.AddRedisSessionManager(builder.Configuration);
builder.Services.AddSingleton<ICustomJsonSerializer, CustomJsonSerializer>();
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
