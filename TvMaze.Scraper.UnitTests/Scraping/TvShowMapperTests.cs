using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Moq;
using TvMaze.Data.Models;
using TvMaze.Scraper.Models;
using TvMaze.Scraper.Scraping;
using Xunit;
using TvShow = TvMaze.Scraper.Models.TvShow;

namespace TvMaze.Scraper.UnitTests
{
    public class TvShowMapperTests
    {
        private readonly TvShowMapper _target;
        public TvShowMapperTests() => _target = new TvShowMapper();

        [Fact]
        public void MapAsync_Success()
        {
            // Arrange
            var tvShowAndCast = new TvShowAndCast
            {
                TvShow = new TvShow {Id = 42, Name = "TvShow", Url = "Url",},
                Cast = new[]
                {
                    new Person {Id = 1, Name = "Name", Birthday = "1980-01-12"},
                    new Person {Id = 2, Name = "NoName", Birthday = "2000-09-01"},
                }
            };
            
            var expected = new Data.Models.TvShow {MazeTvId = 42, Name = "TvShow", TvMazeUrl = "Url",
                Cast = new []
                {
                    new Data.Models.CastMember {MazeTvId = 2, Name = "NoName", Birthday = new DateTime(2000, 9, 1)},
                    new Data.Models.CastMember {MazeTvId = 1, Name = "Name", Birthday = new DateTime(1980, 1, 12)},
                }
            };

            // Act
            var actual = _target.Map(tvShowAndCast);

            // Assert
            Assert.Equal(expected.MazeTvId, actual.MazeTvId);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.TvMazeUrl, actual.TvMazeUrl);
            Assert.Equal(expected.Cast.First(), actual.Cast.First());
            Assert.Equal(expected.Cast.Last(), actual.Cast.Last());
        }
    }
}
