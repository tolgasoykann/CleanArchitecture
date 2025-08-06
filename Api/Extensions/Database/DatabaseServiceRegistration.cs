using Infrastructure.Services.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Extensions.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddDatabaseManager(this IServiceCollection services, IConfiguration configuration)
        {
            // Connection string kayıt ediliyor
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Dapper ile çalışan custom bir manager servis ekleniyor
            services.AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
           // services.AddScoped<ICustomerRepository, CustomerRepository>(); // örnek repository

            return services;
        }
    }

}
