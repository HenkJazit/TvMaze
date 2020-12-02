using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using TvMaze.Data;
using TvMaze.Scraper.Models;

namespace TvMaze.Scraper.Scraping
{
    public interface ITvShowLoader
    {
        Task LoadAsync(ITargetBlock<TvShow> target, HttpClient httpClient, CancellationToken cancellationToken);
    }

    public class TvShowLoader : ITvShowLoader
    {
        private readonly ITvShowRepository _tvShowRepository;
        private readonly ILogger<TvShowLoader> _logger;

        public TvShowLoader(ITvShowRepository tvShowRepository, ILogger<TvShowLoader> logger)
        {
            _tvShowRepository = tvShowRepository;
            _logger = logger;
        }

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task LoadAsync(ITargetBlock<TvShow> target, HttpClient httpClient, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LoadAsync)} START");
            var stopWatch = Stopwatch.StartNew();

            var (lastId, page) = GetStartInfo();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var response = await httpClient.GetAsync($"shows?page={page}", cancellationToken);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                        break;

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Scraping failed for tv shows, {response.StatusCode} - {response.ReasonPhrase}");

                    await using var json = await response.Content.ReadAsStreamAsync();

                    var tvShows =
                        await JsonSerializer.DeserializeAsync<TvShow[]>(json, JsonSerializerOptions, cancellationToken) ??
                        new TvShow[0];

                    foreach (var tvShow in tvShows.Where(show => show.Id > lastId))
                    {
                        target.Post(tvShow);
                    }

                    page++;

                    _logger.LogInformation($"{page} TV show pages read");

                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Failed after {page} Tv Show pages");
                }
            }

            target.Complete();

            stopWatch.Stop();
            _logger.LogInformation($"{nameof(LoadAsync)} FINISHED after {stopWatch.Elapsed.TotalSeconds} seconds.");
        }

        private const int IdsPerPage = 250;
        private (int lastId, int startPage) GetStartInfo()
        {
            var lastId = _tvShowRepository.GetLastLoadedId();
            var startPage = lastId / IdsPerPage;

            return (lastId, startPage);
        }
    }
}
