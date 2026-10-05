
//using Confluent.Kafka;

//public class NotificationGate : IDisposable
//{
//    private readonly FileSystemWatcher _watcher;
//    private readonly IProducer<Null, string> _producer;
//    private readonly string _folderPath;
//    private readonly string _topic;
//    //public NotificationGate(string watchFolderPath)
//    //{

//    //    _watcher = new FileSystemWatcher(watchFolderPath)
//    //    {
//    //        NotifyFilter = NotifyFilters.FileName,
//    //        Filter = "*.ready"
//    //    };

//    //    _watcher.Created += OnReadyFileCreated;
//    //}
//    public NotificationGate(string folderPath, string kafkaServer, string topic)
//    {
//        _folderPath = folderPath;
//        _topic = topic;

//        var config = new ProducerConfig
//        {
//            BootstrapServers = kafkaServer
//        };

//        _producer = new ProducerBuilder<Null, string>(config).Build();

//        _watcher = new FileSystemWatcher(_folderPath)
//        {
//            Filter = "*.ready",
//            NotifyFilter = NotifyFilters.FileName
//        };

//        _watcher.Created += OnReadyFileCreated;
//        _watcher.EnableRaisingEvents = true;
//        _watcher.InternalBufferSize = 65536;
//    }


//    public void Start()
//    {
//        _watcher.EnableRaisingEvents = true;
//        Console.WriteLine("[NotificationGate] Listening for incoming alerts...");
//    }

//    private async void OnReadyFileCreated(object sender, FileSystemEventArgs e)
//    {
//        try
//        {
//            await Task.Delay(100);

//            var fileName = Path.GetFileNameWithoutExtension(e.Name);

//            var jsonPath = Path.Combine(_folderPath, $"{fileName}.json");


//            string? alertPath = null;

//            if (File.Exists(jsonPath))
//                alertPath = jsonPath;


//            if (alertPath == null)
//                return;

//            var content = await File.ReadAllTextAsync(alertPath);

//            await _producer.ProduceAsync(
//                _topic,
//                new Message<Null, string>
//                {
//                    Value = content
//                });

//            Console.WriteLine($"Notification sent to Kafka: {fileName}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }

//    private void ProcessAlert(string filePath)
//    {

//        string content = File.ReadAllText(filePath);
//        Console.WriteLine($"[ALERT DETECTED] File: {Path.GetFileName(filePath)}");

//    }
//    public void Dispose()
//    {
//        _watcher.EnableRaisingEvents = false;
//        _watcher.Dispose();
//        _producer.Flush(TimeSpan.FromSeconds(5));
//        _producer.Dispose();
//    }
//}

