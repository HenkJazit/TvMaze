using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using TvMaze.Scraper.Models;
using DataTvShow = TvMaze.Data.Models.TvShow;

namespace TvMaze.Scraper.Scraping
{
    public interface ITvShowPreparer
    {
        Task PrepareAsync(ISourceBlock<TvShowAndCast> source, ITargetBlock<DataTvShow> target, CancellationToken cancellationToken);
    }

    public class TvShowPreparer : ITvShowPreparer
    {
        private readonly ITvShowMapper _tvShowMapper;
        private readonly ILogger<TvShowPreparer> _logger;

        public TvShowPreparer(ITvShowMapper tvShowMapper, ILogger<TvShowPreparer> logger)
        {
            _tvShowMapper = tvShowMapper;
            _logger = logger;
        }

        public async Task PrepareAsync(ISourceBlock<TvShowAndCast> source, ITargetBlock<DataTvShow> target, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PrepareAsync)} START");
            var stopWatch = Stopwatch.StartNew();

            while (await source.OutputAvailableAsync(cancellationToken).ConfigureAwait(false))
            {
                var tvShowAndCast = await source.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                var tvShow = _tvShowMapper.Map(tvShowAndCast);

                target.Post(tvShow);
            }

            target.Complete();

            stopWatch.Stop();
            _logger.LogInformation($"{nameof(PrepareAsync)} FINISHED after {stopWatch.Elapsed.TotalSeconds} seconds.");
        }
    }
}
