using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
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

        public Task LogInfoAsync(string message, object? extraData = null)
            => WriteLogAsync("INFO", message, null, extraData);

        public Task LogWarningAsync(string message, object? extraData = null)
            => WriteLogAsync("WARN", message, null, extraData);

        public Task LogErrorAsync(string message, Exception? exception = null, object? extraData = null)
            => WriteLogAsync("ERROR", message, exception, extraData);

        private async Task WriteLogAsync(string level, string message, Exception? exception, object? extraData)
        {
            var timestamp = DateTime.UtcNow;

            Console.WriteLine($"[{timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");

            try
            {
                var logEntry = new
                {
                    Timestamp = timestamp,
                    Level = level,
                    Message = message,
                    Exception = exception?.ToString(),
                    Data = extraData
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
