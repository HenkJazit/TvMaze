using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TvMaze.Data;
using TvMaze.Scraper.Scraping;

namespace TvMaze.Scraper
{
    public class HostedService : IHostedService
    {
        private readonly IScrapeService _scrapeService;

        public HostedService(IScrapeService scrapeService)
        {
            _scrapeService = scrapeService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scrapeService.ScrapeAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
