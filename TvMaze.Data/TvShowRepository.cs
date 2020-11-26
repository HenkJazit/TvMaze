using System.Collections.Generic;
using TvMaze.Data.Models;

namespace TvMaze.Data
{
    public interface ITvShowRepository
    {
        void Insert(TvShow tvShow);
        IEnumerable<TvShow> GetTvShowsPaged(int start = 0, int rows = 10);
        void DeleteAll();
        int GetLastLoadedId();
    }

    public class TvShowRepository : ITvShowRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private const string CollectionName = "TvShows";

        public TvShowRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Insert(TvShow tvShow)
        {
            using var db = _connectionFactory.GetDataBase();
            db.GetCollection<TvShow>(CollectionName).Insert(tvShow);
        }

        public IEnumerable<TvShow> GetTvShowsPaged(int start = 0, int rows = 10)
        {
            start = start < 0 ? 0 : start;
            rows = rows <= 0 || rows > 100  ? 10 : rows;

            using var db = _connectionFactory.GetDataBase();
            var collection = db.GetCollection<TvShow>(CollectionName);
            collection.EnsureIndex(tvShow => tvShow.MazeTvId);

            return collection.Query()
                .OrderBy(tvShow => tvShow.MazeTvId)
                .Skip(start)
                .Limit(rows).ToArray();
        }

        public int GetLastLoadedId()
        {
            using var db = _connectionFactory.GetDataBase();
            var collection = db.GetCollection<TvShow>(CollectionName);
            collection.EnsureIndex(tvShow => tvShow.MazeTvId);

            return collection.Query()
                .OrderByDescending(tvShow => tvShow.MazeTvId)
                .Limit(1).FirstOrDefault()?.MazeTvId ?? 0;
        }

        public void DeleteAll()
        {
            using var db = _connectionFactory.GetDataBase();
            db.GetCollection<TvShow>(CollectionName).DeleteAll();
        }
    }
}
