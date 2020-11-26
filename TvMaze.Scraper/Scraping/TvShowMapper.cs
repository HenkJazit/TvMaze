using System;
using System.Globalization;
using System.Linq;
using TvMaze.Scraper.Models;
using DataTvShow = TvMaze.Data.Models.TvShow;

namespace TvMaze.Scraper.Scraping
{
    public interface ITvShowMapper
    {
        DataTvShow Map(TvShowAndCast tvShowAndCast);
    }

    public class TvShowMapper : ITvShowMapper
    {
        public DataTvShow Map(TvShowAndCast tvShowAndCast) =>
            new DataTvShow
            {
                MazeTvId = tvShowAndCast.TvShow.Id,
                Name = tvShowAndCast.TvShow.Name,
                TvMazeUrl = tvShowAndCast.TvShow.Url,
                Cast = tvShowAndCast.Cast.Select(person => new Data.Models.CastMember
                {
                    MazeTvId = person.Id,
                    Name = person.Name,
                    Birthday = string.IsNullOrWhiteSpace(person.Birthday)
                        ? default(DateTime?)
                        : DateTime.ParseExact(person.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture).Date
                }).OrderByDescending(member => member.Birthday).ToArray()
            };
    }
}
