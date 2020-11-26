using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using TvMaze.Data;
using TvMaze.Scraper.Models;
using DataTvShow = TvMaze.Data.Models.TvShow;

namespace TvMaze.Scraper.Scraping
{
    public interface IScrapeService
    {
        Task ScrapeAsync(CancellationToken cancellationToken);
    }

    public class ScrapeService : IScrapeService
    {
        private readonly ITvShowLoader _tvShowLoader;
        private readonly ICastLoader _castLoader;
        private readonly ITvShowPreparer _tvShowPreparer;
        private readonly ITvShowWriter _tvShowWriter;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScrapeService> _logger;

        public ScrapeService(ITvShowLoader tvShowLoader, ICastLoader castLoader, ITvShowPreparer tvShowPreparer,
            ITvShowWriter tvShowWriter, HttpClient httpClient, ILogger<ScrapeService> logger)
        {
            _tvShowLoader = tvShowLoader;
            _castLoader = castLoader;
            _tvShowPreparer = tvShowPreparer;
            _tvShowWriter = tvShowWriter;
            _httpClient = httpClient;

            _logger = logger;
        }

        public async Task ScrapeAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start scraping");

            var dataTvShows = new BufferBlock<DataTvShow>();
            var mazeCombined = new BufferBlock<TvShowAndCast>();
            var mazeTvShows = new BufferBlock<TvShow>();

            var tasks = new[]
            {
                _tvShowWriter.WriteAsync(dataTvShows, cancellationToken),
                _tvShowPreparer.PrepareAsync(mazeCombined, dataTvShows, cancellationToken),
                _castLoader.LoadAsync(mazeTvShows, mazeCombined, _httpClient, cancellationToken),
                _tvShowLoader.LoadAsync(mazeTvShows, _httpClient, cancellationToken),
            };
            await Task.WhenAll(tasks).ConfigureAwait(false);

            _logger.LogInformation("Finished scraping");
        }
    }
}
