using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Demo.Mongodb
{
    public interface IRepository
    {
        FilterDefinitionBuilder<BsonDocument> Builder { get; }

        Task<BsonDocument> FirstOrDefaultAsync(string name, FilterDefinition<BsonDocument> filters = null, SortDefinition<BsonDocument> sort = null);

        Task<PageListResult> PageList(string name,
                                      FilterDefinition<BsonDocument> filters = null,
                                      SortDefinition<BsonDocument> sort = null,
                                      int offset = 0,
                                      int limit = 10);
    }
}