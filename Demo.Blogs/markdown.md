今天看AspNetCore源代码发现日志模块的设计模式（提供者模式），特此记录

# 类图 & 分析
---
![仅截取部分类&方法](https://upload-images.jianshu.io/upload_images/22730543-868560ff34b31fdc.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

**角色分析**

###### 日志工厂 （ LoggerFactory --> ILoggerFactory）
- 提供注册提供者
- 创建日志记录器（Logger）

###### 日志记录器（Logger --> ILogger）
- 写入日志记录（遍历所有日志提供者的Logger)
- 这里所有注册的日志提供者聚合

###### 日志提供者（ConsoleLoggerProvider --> ILoggerProvider）
- 创建具体日志记录器

###### 具体日志记录者（ConsoleLogger，EventLogLogger）
- 将日志写入具体媒介（控制台，Windows事件日志）

现在来看看这个模式
1. 提供标准的日志写入接口（ILogger）
2. 提供日志提供者接口（ILoggerProvider）
3. 提供注册提供者接口（ILoggerFactory.AddProvider）

这里只是列出部分类和方法，整个Logging要比这个还多，为什么写个日志要整那么多东西？

程序唯一不会变就是不断在变化，这个也是为什么要设计模式运用到程序当中的原因，让程序可扩展来应对这种变化。

AspNetCore内置[8种日志记录提供程序](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#built-in-logging-providers)，但肯定还是远远不够，因为有的可能想把日志写在文本，有的想写在Mongodb，有的想写在ElasticSearch等等，Microsoft不可能把所有的都实现，就算实现也未必适合你的业务使用。
假设现在需要把日志写在Mongo，只需要
1. 实现Mongodb的ILogger - 将日志写到Mongodb
2. 实现Mongodb的ILoggerProvider - 创建Mongodb的Logger
3. 把Provider注册到AspNetCore - ILoggerFactory.AddProvider
这里都是新增代码达到实现把日志写入到Mongodb，这就是6大设计原则之一对扩展开放（可以添加自己的日志），对修改封闭（不需要修改到内部的方法）


######  AspNetCore代码实现（只列出接口）
**ILoggerFactory**
```cs
 ILogger CreateLogger(string categoryName);
 void AddProvider(ILoggerProvider provider);
```
```CreateLogger``` ： 这个和ILoggerProvider提供的CreateLogger虽然都是现实ILogger接口，但是做的事情不一样，LoggerFactory创建的是Logger实例，里面聚合了具体写日志的Logger，遍历它们输出。
```categoryName``` :  可以指定具体，若使用泛型相当于```typeof(T).FullName```，这个用于筛选过滤日志

```AddProvider``` ： 注册一个新的提供者，然后遍历现有的Logger，把新的Provider添加到现有logger里面

**ILoggerProvider**
```cs
 ILogger CreateLogger(string categoryName);
```
```CreateLogger``` : 用于创建具体写日志Logger（例如Console）

**ILogger**
```cs
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
bool IsEnabled(LogLevel logLevel);
IDisposable BeginScope<TState>(TState state);
```
```Log<TState>(....)```： 输出日志
```bool IsEnabled``` : 指定的日志级别是否可用
```IDisposable BeginScope<TState>()``` ： 开启日志作用域，将这个域范围的日志都放一起


# AspNetCore中使用Log4Net
---
AspNetCore使用Log4Net作为记录很简单，只需
1. 安装包：```dotnet install Microsoft.Extensions.Logging.Log4Net.AspNetCore```
2. Configure 添加：``` loggerFactory.AddLog4Net();```
3. 添加```log4net.config```配置文件

看看Microsoft.Extensions.Logging.Log4Net.AspNetCore如何实现ILogger和ILoggerProvider接口

**Log4NetProvider**
```
public ILogger CreateLogger(string categoryName)
    => this.loggers.GetOrAdd(categoryName, this.CreateLoggerImplementation);

private Log4NetLogger CreateLoggerImplementation(string name)
{
    var options = new Log4NetProviderOptions
    {
        Name = name,
        LoggerRepository = this.loggerRepository.Name
    };

    options.ScopeFactory = new Log4NetScopeFactory(new Log4NetScopeRegistry());

    return new Log4NetLogger(options);
}
```
**Log4NetLogger**
```
switch (logLevel)
		{
		case LogLevel.None:
			break;
		case LogLevel.Critical:
		{
			string overrideCriticalLevelWith = options.OverrideCriticalLevelWith;
			if (!string.IsNullOrEmpty(overrideCriticalLevelWith) && overrideCriticalLevelWith.Equals(LogLevel.Critical.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				log.Critical(text, exception);
			}
			else
			{
				log.Fatal(text, exception);
			}
			break;
		}
		case LogLevel.Debug:
			log.Debug(text, exception);
			break;
		case LogLevel.Error:
			log.Error(text, exception);
			break;
		case LogLevel.Information:
			log.Info(text, exception);
			break;
		case LogLevel.Warning:
			log.Warn(text, exception);
			break;
		case LogLevel.Trace:
			log.Trace(text, exception);
			break;
		default:
			log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
			log.Info(text, exception);
			break;
		}
```
log4net的ILog是没有Trace和Critical方法，这两个是扩展方法，调用log4net log4net.Repository.Hierarchy.Logger.Log()方法

log4net 里面有Fatal代表日志最高级别，AspNetCore的Critical是日志最高级别，习惯log4net可能习惯用Fatal，这个时候只需要在注册的时候
```cs
loggerFactory.AddLog4Net(new Log4NetProviderOptions()
{
    OverrideCriticalLevelWith = "Critical"
});
```
在Controller调用
```cs
 _logger.LogCritical("Log Critical");
```
看看效果
```
2020-04-27 13:42:05,042 [10] FATAL LoggingPattern.Controllers.WeatherForecastController (null) - Log Critical
```
奇怪，没有按预期发生。这个组件是开源的，可以下载下来调试看看
github克隆下来 [Microsoft.Extensions.Logging.Log4Net.AspNetCore](https://github.com/huorswords/Microsoft.Extensions.Logging.Log4Net.AspNetCore)

**调试过程**
1. 将Microsoft.Extensions.Logging.Log4Net.AspNetCore.csproj的SignAssembly设置false（这个是程序集强签名）
```
<SignAssembly>false</SignAssembly>
```
2. 将引用改成引用本地，我这里是放在跟项目平级
```xml
  <ItemGroup>
    <ProjectReference Include="..\Microsoft.Extensions.Logging.Log4Net.AspNetCore\src\Microsoft.Extensions.Logging.Log4Net.AspNetCore\Microsoft.Extensions.Logging.Log4Net.AspNetCore.csproj" />
  </ItemGroup>
```
我这里是用VSCode，如果用VS不用这么麻烦

3. 然后就可以打断点，在写日志和之前看到的那个判断打个断点
![这个值为空，导致都是写Faltal](https://upload-images.jianshu.io/upload_images/22730543-f1a68272e3f3caa8.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

4. 接下来就是看看这个值怎么来的
```cs
builder.Services.AddSingleton<ILoggerProvider>(new Log4NetProvider(options));
```
```
public Log4NetProvider(Log4NetProviderOptions options)
{
}
```
注册一个单例的Log4NetProvider，参入参数options，Logger是在Provider的CreateLogger创建，现在看看CreateLogger
```
public ILogger CreateLogger(string categoryName)
    => this.loggers.GetOrAdd(categoryName, this.CreateLoggerImplementation);
```
```cs
private Log4NetLogger CreateLoggerImplementation(string name)
{
    var options = new Log4NetProviderOptions
    {
        Name = name,
        LoggerRepository = this.loggerRepository.Name
    };
    options.ScopeFactory = new Log4NetScopeFactory(new Log4NetScopeRegistry());
    return new Log4NetLogger(options);
}
```
到这里就清楚了，CreateLoggerImplementation里面又new了一个options，然后没有给OverrideCriticalLevelWith赋值（我认为这是个Bug，应该也很少人会用这个功能）这里之所以没用单例的options，因为要给每个Logger的目录名称动态赋值。
给这个库作者提了Issues和[PR](https://github.com/huorswords/Microsoft.Extensions.Logging.Log4Net.AspNetCore/pull/84)

# 添加自定义的日志记录器
---
假设现在需要把日志加入到Mongodb
1. 添加Mongodb驱动
```
dotnet add package MongoDB.Driver
```
2. 实现接口ILogger
```cs
public class MongodbLogger : ILogger
{
    private readonly string _name;
    private MongoDB.Driver.IMongoDatabase _database;

    public MongodbLogger(string name, MongoDB.Driver.IMongoDatabase database)
    {
        _name = name;
        _database = database;
    }
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var collection = _database.GetCollection<dynamic>(logLevel.ToString().ToLower());

        string message = formatter(state, exception);

        collection.InsertOneAsync(new
        {
            time = DateTime.Now,
            name = _name,
            message,
            exception
        });
    }
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public System.IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
}
```
3. 实现ILoggerProvider接口
```cs
public class MongodbProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, MongodbLogger> _loggers = new ConcurrentDictionary<string, MongodbLogger>();
    private MongoDB.Driver.IMongoDatabase _database;
    public MongodbProvider(MongoDB.Driver.IMongoDatabase database)
    {
        _database = database;
    }
    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new MongodbLogger(categoryName, this._database));
    public void Dispose() => this._loggers.Clear();
}
```
4. 添加MongodbLogging扩展函数（非必须）
```cs
public static ILoggerFactory AddMongodb(this ILoggerFactory factory, string connetionString = "mongodb://127.0.0.1:27017/logging")
{
    var mongoUrl = new MongoDB.Driver.MongoUrl(connetionString);
    var client = new MongoDB.Driver.MongoClient(mongoUrl);

    factory.AddProvider(new MongodbProvider(client.GetDatabase(mongoUrl.DatabaseName)));

    return factory;
}
```
5. Configure注册MongodbLogging
```cs
loggerFactory.AddMongodb();
```
![运行效果](https://upload-images.jianshu.io/upload_images/22730543-9726fb781e0c5587.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

# 扩展
---
设计模式的好处是，我们可以容易扩展它达到我们要求，除了要知道如何扩展它，还应该在其他地方应用它，例如我们经常需要消息通知用户，但是通知渠道（提供者），在一开始未必全部知道，例如一开始只有短信，邮件通知，随着业务发展可能需要增加微信推送，提供者模式就很好应对这一种情况，很容易画出下面类图。
![消息发送简单类图](https://upload-images.jianshu.io/upload_images/22730543-407ca96ace4840d0.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

当需要扩展发送消息渠道，只需要实现ISenderProvider（哪个提供），ISender（如何发送）
