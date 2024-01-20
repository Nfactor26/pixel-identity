using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenIddict.MongoDb;


using IHost host = Host.CreateDefaultBuilder(args).Build();
var switchMappings = new Dictionary<string, string>()
           {
               { "--mongo-host", "mongo-host" },
               { "--mongo-db", "mongo-db" }           };

IConfiguration config = new ConfigurationBuilder()
    .AddCommandLine(args, switchMappings)
    .AddJsonFile("appsettings.json", true, false)
    .AddEnvironmentVariables()
    .Build();

string mongoServer = config.GetValue<string>("mongo-host") ?? throw new Exception("mongo-host is required");
string mongoDb = config.GetValue<string>("mongo-db") ?? throw new Exception("mongo-db is required");

var services = new ServiceCollection();
services.AddOpenIddict()
    .AddCore()
    .UseMongoDb()
    .UseDatabase(new MongoClient(mongoServer).GetDatabase(mongoDb));

await using var provider = services.BuildServiceProvider();
var context = provider.GetRequiredService<IOpenIddictMongoDbContext>();
var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictMongoDbOptions>>().CurrentValue;
var database = await context.GetDatabaseAsync(CancellationToken.None);

var applications = database.GetCollection<BsonDocument>(options.ApplicationsCollectionName);
await applications.UpdateManyAsync(
    filter: Builders<BsonDocument>.Filter.Empty,
    update: Builders<BsonDocument>.Update.Rename("type", "client_type"));

Console.WriteLine("Completed !!");