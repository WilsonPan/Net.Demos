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

            var filters =  repository.Builder.Gt("age", 18);
            filters &= repository.Builder.Eq("sex",true);

            var data = await repository.FirstOrDefaultAsync("col", filters);

            Console.WriteLine(data?["Name"]);
        }
    }
}
