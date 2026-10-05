using System;
using System.IO;
using System.Threading;
using Confluent.Kafka;
using FileWatch.Logger;
using FileWatch.Logger;
public class NotificationGate : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;
    private readonly ICustomLogger _logger;

    public NotificationGate(string folderPath, string kafkaServer, string topic, ICustomLogger logger)
    {
        _topic = topic;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaServer
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();

        _watcher = new FileSystemWatcher(folderPath)
        {
            Filter = "*.ready",
            NotifyFilter = NotifyFilters.FileName,
            InternalBufferSize = 65536,
            IncludeSubdirectories = true

        };

        _watcher.Created += OnReadyFileCreated;
    }

    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
        Console.WriteLine("[NotificationGate] Listening for incoming alerts...");
        _logger.LogInfoAsync("[NotificationGate] Listening for incoming alerts...");
    }

    private void OnReadyFileCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            string basePath = Path.ChangeExtension(e.FullPath, null);
            string jsonPath = basePath + ".json";        

          
            string targetPath = null;
            if (File.Exists(jsonPath))
            {
                targetPath = jsonPath;
            }
            if (targetPath != null)
            {
                ProcessAlert(targetPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error processing event for {e.FullPath}: {ex.Message}");
            _logger.LogErrorAsync($"[ERROR] Error processing event for {e.FullPath}: {ex.Message}");
        }
    }

    private void ProcessAlert(string filePath)
    {
        try
        {

            string content = File.ReadAllText(filePath);
            
            Console.WriteLine($"[ALERT DETECTED] File: {Path.GetFileName(filePath)}");
            _logger.LogInfoAsync($"[ALERT DETECTED] File: {Path.GetFileName(filePath)}");
            _producer.Produce(_topic, new Message<Null, string> { Value = content }, report =>
            {
                if (report.Error.IsError)
                {
                    Console.WriteLine($"[KAFKA ERROR] Delivery failed: {report.Error.Reason}");
                    _logger.LogErrorAsync($"[KAFKA ERROR] Delivery failed: {report.Error.Reason}");
                }
                else
                {
                    Console.WriteLine($"[KAFKA SUCCESS] Delivered to {_topic} [Offset: {report.Offset}]");
                    _logger.LogInfoAsync($"[KAFKA SUCCESS] Delivered to {_topic} [Offset: {report.Offset}]");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to process alert file {filePath}: {ex.Message}");
            _logger.LogErrorAsync($"[ERROR] Failed to process alert file {filePath}: {ex.Message}");
        }
    }
   

    public void Dispose()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}

