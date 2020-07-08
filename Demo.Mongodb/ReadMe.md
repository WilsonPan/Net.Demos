# Quick Start

1. config mongo conectionstring (config.json)

```js
{
    "ConnectionStrings": {
        "MongodbConnectionString" : "mongodb://127.0.0.1:27017/demo"
    }
}
```

# Usage

1. new repository
   
```cs
var repository = new MongoRepository();
```

- FirstOrDefaultAsync

```cs

// definition filters
var filters =  repository.Builder.Gt("age", 18);
filters &= repository.Builder.Eq("sex",true);

// find 
var data = await repository.FirstOrDefaultAsync("col", filters);

// get data
Console.WriteLine(data["Name"]);

```

- PageList

```cs

// definition filters
var filters =  repository.Builder.Gt("age", 18);
filters &= repository.Builder.Eq("sex",true);

// get page list
var result = await repository.PageList(name: "col",
                                       filters: filters,
                                       sort: new BsonDocument("timestamp", -1));

// get data
Console.WriteLine($"{result.Total}, {result.PageList.ToJson()}");
```