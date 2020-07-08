using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Demo.Mongodb
{
    public class MongoRepository
    {
        private static IConfigurationRoot _configuration;
        private static IMongoClient _client;
        private static IMongoDatabase _database;
        static MongoRepository()
        {
            var builder = new ConfigurationBuilder();

            _configuration = builder.SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                                    .AddJsonFile("local.json", optional: false, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();

            var connectionString = _configuration.GetConnectionString("MongodbConnectionString");

            var mongoUrl = new MongoUrl(connectionString);

            _client = new MongoClient(mongoUrl);

            _database = _client.GetDatabase(mongoUrl.DatabaseName);
        }

        public Task<T> FirstOrDefaultAsync<T>(string name, FilterDefinition<T> filters)
        {
            return _database.GetCollection<T>(name)
                            .Find(filters)
                            .FirstOrDefaultAsync();
        }
    }
}