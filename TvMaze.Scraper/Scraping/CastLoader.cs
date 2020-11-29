using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using TvMaze.Scraper.Models;

namespace TvMaze.Scraper.Scraping
{
    public interface ICastLoader
    {
        Task LoadAsync(ISourceBlock<TvShow> source, ITargetBlock<TvShowAndCast> target, HttpClient httpClient,
            CancellationToken cancellationToken);
    }

    public class CastLoader : ICastLoader
    {
        private readonly ILogger<CastLoader> _logger;

        public CastLoader(ILogger<CastLoader> logger) => _logger = logger;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task LoadAsync(ISourceBlock<TvShow> source, ITargetBlock<TvShowAndCast> target, HttpClient httpClient,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LoadAsync)} START");
            var stopWatch = Stopwatch.StartNew();

            var showCount = 0;
            while (await source.OutputAvailableAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var tvShow = await source.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    var response = await httpClient.GetAsync($"shows/{tvShow.Id}/cast", cancellationToken)
                        .ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Scraping failed for cast, {response.StatusCode} - {response.ReasonPhrase}");

                    await using var json = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                    var cast = await JsonSerializer
                                   .DeserializeAsync<CastMember[]>(json, JsonSerializerOptions, cancellationToken)
                                   .ConfigureAwait(false) ??
                               new CastMember[0];

                    target.Post(new TvShowAndCast
                    {
                        TvShow = tvShow,
                        Cast = cast.Select(c => c.Person).ToArray()
                    });
                    showCount++;

                    if (showCount % 100 == 0)
                        _logger.LogInformation($"{showCount} TV shows with cast read");

                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Failed after {showCount} Tv Shows");
                }
            }

            target.Complete();

            stopWatch.Stop();
            _logger.LogInformation($"{nameof(LoadAsync)} FINISHED after {stopWatch.Elapsed.TotalSeconds} seconds.");
        }
    }
}
