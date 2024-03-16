using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BloodCenter.Search.Client
{
    public static class RegisterClient
    {
        public static IServiceCollection AddSearchClient(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddClient<ISearchClient, SearchClient>(configuration, "search-client");
        }

        private static IServiceCollection AddClient<T, TImpl>(this IServiceCollection services,
            IConfiguration configuration, string httpClientName)
            where TImpl : CustomHttpClient where T : class
        {
            services.AddHttpClient(httpClientName);

            services.TryAddSingleton<T>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();

                var baseUri = new Uri(configuration["endpoints:search:baseUri"] ?? "http://localhost:5246");
                if (TimeSpan.TryParse(configuration["endpoints:search:timeout"], out var timeout))
                    return (T)Activator.CreateInstance(typeof(TImpl), factory.CreateClient(httpClientName), baseUri, timeout)!;

                return (T)Activator.CreateInstance(typeof(TImpl), factory.CreateClient(httpClientName), baseUri)!;
            });

            return services;
        }
    }
}
