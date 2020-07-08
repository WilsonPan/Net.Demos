using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Demo.Mongodb
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var repository = new MongoRepository();
            var builder = Builders<BsonDocument>.Filter;

            var filters = FilterDefinition<BsonDocument>.Empty;

            filters &= builder.Eq("Age", 18);

            var data = await repository.FirstOrDefaultAsync("col", filters);

            Console.WriteLine(data["Name"]);
        }
    }
}
