using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using TvMaze.Data;
using TvMaze.Data.Models;

namespace TvMaze.Scraper.Scraping
{
    public interface ITvShowWriter
    {
        Task WriteAsync(ISourceBlock<TvShow> source, CancellationToken cancellationToken);
    }

    public class TvShowWriter : ITvShowWriter
    {
        private readonly ITvShowRepository _tvShowRepository;
        private readonly ILogger<TvShowWriter> _logger;

        public TvShowWriter(ITvShowRepository tvShowRepository, ILogger<TvShowWriter> logger)
        {
            _tvShowRepository = tvShowRepository;
            _logger = logger;
        }

        public async Task WriteAsync(ISourceBlock<TvShow> source, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(WriteAsync)} START");
            var stopWatch = Stopwatch.StartNew();

            while (await source.OutputAvailableAsync(cancellationToken).ConfigureAwait(false))
            {
                var tvShow = await source.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                _tvShowRepository.Insert(tvShow);
            }

            stopWatch.Stop();
            _logger.LogInformation($"{nameof(WriteAsync)} FINISHED after {stopWatch.Elapsed.TotalSeconds} seconds.");
        }
    }
}
