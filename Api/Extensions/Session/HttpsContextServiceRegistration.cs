using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Domain.Interfaces;
using Infrastructure.Services.Session;

namespace Api.Extensions.Session
{
    public static class HttpsContextServiceRegistration
    {
        public static IServiceCollection AddHttpContextSessionManager(this IServiceCollection services)
        {
            // HttpContext erişimi için gerekli accessor
            services.AddHttpContextAccessor();

            // HttpContext tabanlı session manager
            services.AddScoped<ISessionManager, HttpContextSessionManager>();

            // Session servisini aktif etmek için (Program.cs içinde app.UseSession() kullanılmalı)
            services.AddSession();

            return services;
        }
    }
}
