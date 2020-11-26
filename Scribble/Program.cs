using System;
using System.Net.Http;
using System.Threading.Tasks;
using TvMaze.Data;
using TvMaze.Scraper;

namespace Scribble
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await TestReadTvShows();
        }

        private static async Task TestReadTvShows()
        {
            var client = new HttpClient{BaseAddress = new Uri("https://api.tvmaze.com", UriKind.Absolute) };

            //var service = new ScrapeService(new TvShowLoader(), new CastLoader(), new TvShowMapper(), new TvShowWriter(new TvShowRepository()), client, null);

            //await service.ScrapeAsync(default);
        }
    }
}
