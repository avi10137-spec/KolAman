using Elastic.Clients.Elasticsearch;
using FileWatch.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FileWatch.Logger
{
    public class CustomLogger : ICustomLogger
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly string _logIndexName;

        public CustomLogger(ElasticsearchClient elasticClient, IConfiguration configuration)
        {
            _elasticClient = elasticClient;
            _logIndexName = configuration["Elasticsearch:LogIndexName"] ?? "notification-logs";
        }

        public Task LogInfoAsync(string message)
            => WriteLogAsync("INFO", message,null);

        public Task LogWarningAsync(string message)
            => WriteLogAsync("WARN", message, null);

        public Task LogErrorAsync(string message, Exception? exception = null)
            => WriteLogAsync("ERROR", message, exception);

        private async Task WriteLogAsync(string level, string message, Exception? exception)
        {
            var timestamp = DateTime.UtcNow;

            Console.WriteLine($"[{timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");

            try
            {
                var logEntry = new NotificationLog
                {
                    Timestamp = timestamp,
                    Level = level,
                    Message = message,
                    Exception = exception?.ToString()
                   
                };

                await _elasticClient.IndexAsync(logEntry, idx => idx.Index(_logIndexName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Logger Error] Failed to send log to Elastic: {ex.Message}");
            }
        }
    }
}
