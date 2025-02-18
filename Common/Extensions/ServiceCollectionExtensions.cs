using Blackwater.Core.Bson;
using Blackwater.Core.Database;
using Microsoft.Extensions.DependencyInjection;

// sentralized place to register application services

namespace Blackwater.Core.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlackwaterCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<IBsonConverter, BsonConverter>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            return services;
        }
    }
}
