using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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

        public FilterDefinitionBuilder<BsonDocument> Builder => Builders<BsonDocument>.Filter;

        public FilterDefinition<BsonDocument> GetEmptyFilters() => FilterDefinition<BsonDocument>.Empty;

        public IFindFluent<BsonDocument, BsonDocument> Find(string name, FilterDefinition<BsonDocument> filters = null, SortDefinition<BsonDocument> sort = null)
        {
            filters = filters ?? FilterDefinition<BsonDocument>.Empty;

            return _database.GetCollection<BsonDocument>(name)
                            .Find(filters)
                            .Sort(sort);
        }
        public Task<BsonDocument> FirstOrDefaultAsync(string name, FilterDefinition<BsonDocument> filters = null, SortDefinition<BsonDocument> sort = null)
        {
            return Find(name, filters, sort).FirstOrDefaultAsync();
        }

        public Task<PageListResult> PageList(string name,
                                                 FilterDefinition<BsonDocument> filters = null,
                                                 SortDefinition<BsonDocument> sort = null,
                                                 int offset = 0,
                                                 int limit = 10)
        {
            var pageTask = Find(name, filters, sort).Skip(offset)
                                                .Limit(limit)
                                                .ToListAsync();

            var countTask = Find(name, filters).CountDocumentsAsync();

            Task.WaitAll(pageTask, countTask);

            return Task.FromResult(new PageListResult()
            {
                Total = countTask.Result,
                PageList = pageTask.Result
            });
        }
    }
}