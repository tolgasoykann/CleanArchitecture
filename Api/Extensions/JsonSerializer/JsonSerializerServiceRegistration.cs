using Domain.Interfaces;
using Infrastructure.Services.Config;
using Infrastructure.Services.JsonSerializer;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions.JsonSerializer
{
   public static class JsonSerializerServiceRegistration
    {
        public static IServiceCollection AddJosnSerializer(this IServiceCollection services)
        {
            services.AddScoped<ICustomJsonSerializer, CustomJsonSerializer>();
            return services;
        }
    }
}
