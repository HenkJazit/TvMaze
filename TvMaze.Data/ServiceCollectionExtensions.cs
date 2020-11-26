using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TvMaze.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataBaseConfiguration>(configuration.GetSection(nameof(DataBaseConfiguration)));

            services.TryAddSingleton<IConnectionFactory, LiteDbFactory>();
            services.TryAddSingleton<ITvShowRepository, TvShowRepository>();
        }
    }
}
