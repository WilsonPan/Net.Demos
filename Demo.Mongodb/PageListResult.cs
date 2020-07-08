using System.Collections.Generic;
using MongoDB.Bson;

namespace Demo.Mongodb
{
    public class PageListResult
    {
        public long Total { get; set; }

        public List<BsonDocument> PageList { get; set; }
    }
}