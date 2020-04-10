using System;
using System.Linq;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Demo.RedisCluster
{
    class Program
    {
        static readonly string configString = "127.0.0.1:6383";
        static void Main(string[] args)
        {
            var client = ConnectionMultiplexer.Connect(configString);

            // RedisStringOperation(client);

            // RedisHashOperation(client);

            // RedisListOperation(client);

            // RedisSetOperation(client);

            RedisSortedSetOperation(client);

            client.Dispose();
        }

        /// <summary>
        /// 字符串操作
        /// </summary>
        static void RedisStringOperation(ConnectionMultiplexer client)
        {
            var db = client.GetDatabase();

            //单个Key操作
            db.StringSet("Key", "Wilson");
            Console.WriteLine($"Get Key : {db.StringGet("Key")}");

            db.StringSet("Nums", 1);
            db.StringIncrement("Nums");
            Console.WriteLine($"Get Nums : {db.StringGet("Nums")}");

            db.StringDecrement("Nums");
            Console.WriteLine($"Get Nums : {db.StringGet("Nums")}");

            //多个Key操作
            db.StringSet(new KeyValuePair<RedisKey, RedisValue>[]
            {
                new KeyValuePair<RedisKey, RedisValue>("{user}Name","Wilson"),
                new KeyValuePair<RedisKey, RedisValue>("{user}Age",30)
            });

            foreach (var value in db.StringGet(new RedisKey[] { "{user}Name", "{user}Age" }))
            {
                Console.WriteLine($"{value}");
            }
        }

        static void RedisHashOperation(ConnectionMultiplexer client)
        {
            var db = client.GetDatabase();

            db.HashSet("person", "name", "Wilson");
            Console.WriteLine(db.HashGet("person", "name"));

            db.HashSet("person", new HashEntry[]
            {
                new HashEntry("name","Wilson"),
                new HashEntry("sex",1)
            });
            Console.WriteLine(string.Join("\n", db.HashGet("person", new RedisValue[] { "name", "sex" })));

            Console.WriteLine(string.Join("\n", db.HashGetAll("person")));
        }

        static void RedisListOperation(ConnectionMultiplexer client)
        {
            var db = client.GetDatabase();
            db.KeyDelete("list");

            db.ListLeftPush("list", 100);
            db.ListLeftPush("list", 200);
            db.ListLeftPush("list", 300);
            db.ListRightPush("list", 400);
            db.ListRightPush("list", 500);

            Console.WriteLine(db.ListLeftPop("list"));
            Console.WriteLine(db.ListRightPop("list"));
            Console.WriteLine(string.Join("\t", db.ListRange("list", 0, 2)));
            Console.WriteLine(db.ListLength("list"));
        }

        static void RedisSetOperation(ConnectionMultiplexer client)
        {
            var db = client.GetDatabase();
            db.KeyDelete("user");

            db.SetAdd("user", "Wilson");
            db.SetAdd("user", "Wilson");
            db.SetAdd("user", "Alice");

            Console.WriteLine(db.SetLength("user"));
            Console.WriteLine(db.SetPop("user"));
            Console.WriteLine(string.Join("\n", db.SetMembers("user")));
        }

        static void RedisSortedSetOperation(ConnectionMultiplexer client)
        {
            var db = client.GetDatabase();

            db.KeyDelete("user");

            db.SortedSetAdd("user", "Wilson", 90);
            db.SortedSetAdd("user", "Alice", 85);
            db.SortedSetAdd("user", "Trenary", 12);
            db.SortedSetAdd("user", "Nixon", 30);

            Console.WriteLine(db.SortedSetLength("user"));
            Console.WriteLine(db.SortedSetRemove("user", "Wilson"));
            Console.WriteLine(string.Join("\n", db.SortedSetRangeByRank("user", 0, 2)));
        }
    }
}