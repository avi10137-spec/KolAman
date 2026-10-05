//using FileWatch.Logger;
//using System;
//using System.IO;

//using Elastic.Clients.Elasticsearch;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.IO;
//using System.Threading.Tasks;


//IConfiguration configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();


//var services = new ServiceCollection();

//services.AddSingleton<IConfiguration>(configuration);




//services.AddSingleton(sp =>
//{
//    var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
//    var defaultIndex = configuration["Elasticsearch:IndexName"] ?? "notifications";

//    var settings = new ElasticsearchClientSettings(new Uri(url))
//        .DefaultIndex(defaultIndex);

//    return new ElasticsearchClient(settings);
//});





////services.AddTransient<ElasticInitializer>();



//services.AddSingleton<ICustomLogger, CustomLogger>();
//// 3. בניית ה-ServiceProvider
//using var serviceProvider = services.BuildServiceProvider();
//using (var scope = serviceProvider.CreateScope())
//{
//    //var elasticInitializer = scope.ServiceProvider.GetRequiredService<ElasticInitializer>();
//    await elasticInitializer.EnsureIndexCreatedAsync();
//}




var folderPath = @"C:\users\user\final exem\alert-simulator\alerts";
var kafkaServer = "localhost:9092";
var topic = "notifications";

//using var notificationGate = new NotificationGate(
//    folderPath,
//    kafkaServer,
//    topic);
using var notificationGate = new NotificationGate(folderPath, kafkaServer, topic);

Console.WriteLine("NotificationGate is running...");
notificationGate.Start();
Console.ReadLine();


