using System;
using System.IO;
using System.Reflection;
using LiteDB;
using Microsoft.Extensions.Options;

namespace TvMaze.Data
{
    public interface IConnectionFactory
    {
        ILiteDatabase GetDataBase();
    }

    public class LiteDbFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public LiteDbFactory(IOptions<DataBaseConfiguration> config)
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uriBuilder = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uriBuilder.Path);
            var directoryName = Path.GetDirectoryName(path) ?? throw new DirectoryNotFoundException();
            
            _connectionString = Path.Combine(directoryName, config.Value.ConnectionString); ;
        }

        public ILiteDatabase GetDataBase() => new LiteDatabase(_connectionString);
    }
}
