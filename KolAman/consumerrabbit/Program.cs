using consumerrabbit.Maping;
using consumerrabbit.Models;
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
            IConfiguration configuration =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(
                        "appsettings.json",
                        optional: false,
                        reloadOnChange: true)
                    .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AlertDbContext>(options =>
                options.UseMySql(
                    connectionString,
            ServerVersion.AutoDetect(connectionString)));

            services.AddScoped<AlertRepository>();

            services.AddSingleton(sp =>
            {
                var url =
                    configuration["Elasticsearch:Url"]
                    ?? "http://localhost:9200";

                var index =
                    configuration["Elasticsearch:IndexName"]
                    ?? "command-logs";

                var settings =
                    new ElasticsearchClientSettings(new Uri(url))
                        .DefaultIndex(index);

                return new ElasticsearchClient(settings);
            });

            services.AddSingleton<ConsumerService>();

            using var serviceProvider =
                services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var db =
                    scope.ServiceProvider
                        .GetRequiredService<AlertDbContext>();

                await db.Database.EnsureCreatedAsync();

                Console.WriteLine(
                    "Database and tables are ready.");
            }

            var consumer =
                serviceProvider
                    .GetRequiredService<ConsumerService>();

            await consumer.StartAsync();
        }
    }
}

