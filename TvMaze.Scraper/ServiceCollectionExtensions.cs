using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using TvMaze.Data;
using TvMaze.Scraper.Scraping;

namespace TvMaze.Scraper
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScraping(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataStorage(configuration);

            services.AddSingleton<ITvShowLoader, TvShowLoader>();
            services.AddSingleton<ICastLoader, CastLoader>();
            services.AddSingleton<ITvShowPreparer, TvShowPreparer>();
            services.AddSingleton<ITvShowMapper, TvShowMapper>();
            services.AddSingleton<ITvShowWriter, TvShowWriter>();

            var config = configuration.GetSection(nameof(ScrapeConfiguration)).Get<ScrapeConfiguration>();
            
            var throttlePolicy = Policy<HttpResponseMessage>
                .HandleResult(response => response.StatusCode == (HttpStatusCode) 429)
                .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(config.WaitOnTooManyRequests));

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(config.MaxRetriesOnErrorOrTimeOut,
                    attempt => TimeSpan.FromSeconds((attempt ^ attempt) * config.WaitOnErrorOrTimeOut));

            var timeOutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(config.RequestTimeOut);
            
            services.AddHttpClient<IScrapeService, ScrapeService>(client => client.BaseAddress = new Uri(config.BaseUrl, UriKind.Absolute))
                .AddPolicyHandler(throttlePolicy)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeOutPolicy);
        }
    }
}
