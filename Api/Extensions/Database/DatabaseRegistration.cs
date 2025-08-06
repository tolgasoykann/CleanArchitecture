using Domain.Interfaces;
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
    public static class DatabaseRegistration
    {
        public static IServiceCollection AddDatabaseManager2(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDatabaseManager, DatabaseManager>();
            // diğer managerlar...
            return services;
        }
    }

}
