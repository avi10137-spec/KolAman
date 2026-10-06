using consumerrabbit.Models.consumerrabbit.Models;
using consumerrabbit.Repository;
using Elastic.Clients.Elasticsearch;
using System.Diagnostics;

namespace consumerrabbit.Services
{
    public class TaskManagerService
    {
        private readonly AlertRepository _repository;
        private readonly ElasticsearchClient _elasticClient;

        public TaskManagerService(
            AlertRepository repository,
            ElasticsearchClient elasticClient)
        {
            _repository = repository;
            _elasticClient = elasticClient;
        }

        public async Task HandleAlertAsync(Alert alert)
        {
          
            if (alert.Priority == "LOW" && alert.Classification == "UNCLASSIFIED")
            {
                alert.Status = "CANCEL";
                await _repository.SaveAsync(alert);
                Console.WriteLine($"Alert {alert.AlertId} status updated to CANCEL.");
                return;
            }

         
            alert.Status = "INPROGRESS";
            await _repository.SaveAsync(alert);

         
            _ = Task.Run(async () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                Random random = new Random();
                int seconds = random.Next(2, 11);

                await Task.Delay(seconds * 1000);

                stopwatch.Stop();
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                alert.Status = "DONE";
                await _repository.SaveAsync(alert);

                Console.WriteLine($"Alert {alert.AlertId} finished in {elapsedSeconds:F2} seconds.");

              
                //try
                //{
                //    var logEntry = new
                //    {
                //        AlertId = alert.AlertId,
                //        Command = alert.Command,
                //        Priority = alert.Priority,
                //        Classification = alert.Classification,
                //        ProcessingTimeSeconds = elapsedSeconds,
                //        Timestamp = DateTime.UtcNow,
                //        Status = "DONE"
                //    };

                //    await _elasticClient.IndexAsync(logEntry);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"Failed to send log to Elasticsearch: {ex.Message}");
                //}
            });
        }
    }
}