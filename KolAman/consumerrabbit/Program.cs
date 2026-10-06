using consumerrabbit.Maping;
using consumerrabbit.Repository;
using consumerrabbit.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace consumerrabbit
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            
            services.AddDbContextFactory<AlertDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)));

            services.AddSingleton<AlertRepository>();

            services.AddSingleton(sp =>
            {
                var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
                var index = configuration["Elasticsearch:IndexName"] ?? "command-logs";

                var settings = new ElasticsearchClientSettings(new Uri(url))
                    .DefaultIndex(index);

                return new ElasticsearchClient(settings);
            });

            services.AddSingleton<TaskManagerService>();
            services.AddSingleton<ConsumerService>();
            services.AddSingleton<AlertScannerService>();

            using var serviceProvider = services.BuildServiceProvider();

          
            var factory = serviceProvider.GetRequiredService<IDbContextFactory<AlertDbContext>>();
            using (var db = await factory.CreateDbContextAsync())
            {
                await db.Database.EnsureCreatedAsync();
                Console.WriteLine("Database and tables are ready.");
            }

            var consumer = serviceProvider.GetRequiredService<ConsumerService>();
            var scanner = serviceProvider.GetRequiredService<AlertScannerService>();

            
            var consumerTask = consumer.StartAsync();
            var scannerTask = scanner.StartAsync();

            await Task.WhenAll(consumerTask, scannerTask);
        }
    }
}