using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch;
using FileWatch.Logger;
using FileWatch.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System;
using System.IO;
using System.IO;
using System.Threading.Tasks;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .Build();



var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<ElasticsearchClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var url = config["Elasticsearch:Url"]
              ?? throw new Exception("Elasticsearch:Url is missing");

    var indexName = config["Elasticsearch:IndexName"]
                     ?? throw new Exception("Elasticsearch:LogIndexName is missing");

    var settings = new ElasticsearchClientSettings(new Uri(url))
        .DefaultIndex(indexName);

    return new ElasticsearchClient(settings);
});
services.AddSingleton<ElasticInitializer>();

services.AddSingleton<ICustomLogger, CustomLogger>();

using var serviceProvider = services.BuildServiceProvider();

var elasticInitializer =
    serviceProvider.GetRequiredService<ElasticInitializer>();

var indexName =
    configuration["Elasticsearch:IndexName"]
    ?? throw new Exception("Elasticsearch:LogIndexName is missing");

await elasticInitializer.EnsureIndexCreatedAsync(indexName);

var logger =
    serviceProvider.GetRequiredService<ICustomLogger>();

var kafkaServer =
    configuration["Kafka:Server"]
    ?? throw new Exception("Kafka:Server is missing");

var topic =
    configuration["Kafka:Topic"]
    ?? throw new Exception("Kafka:Topic is missing");

var folderPath =
    configuration["FileWatcher:FolderPath"]
    ?? throw new Exception("FileWatcher:FolderPath is missing");

using var notificationGate = new NotificationGate(
    folderPath,
    kafkaServer,
    topic,
    logger);

Console.WriteLine("NotificationGate is running...");

notificationGate.Start();

Console.ReadLine();









//services.AddTransient<ElasticInitializer>();



//services.AddSingleton<ICustomLogger, CustomLogger>();
//// 3. בניית ה-ServiceProvider
//using var serviceProvider = services.BuildServiceProvider();
//using (var scope = serviceProvider.CreateScope())
//{
//    //var elasticInitializer = scope.ServiceProvider.GetRequiredService<ElasticInitializer>();
//    await elasticInitializer.EnsureIndexCreatedAsync();
//}




//var folderPath = @"C:\users\user\final exem\alert-simulator\alerts";
//var kafkaServer = "localhost:9092";
//var topic = "notifications";

////using var notificationGate = new NotificationGate(
////    folderPath,
////    kafkaServer,
////    topic);
//using var notificationGate = new NotificationGate(folderPath, kafkaServer, topic);

//Console.WriteLine("NotificationGate is running...");
//notificationGate.Start();
//Console.ReadLine();



