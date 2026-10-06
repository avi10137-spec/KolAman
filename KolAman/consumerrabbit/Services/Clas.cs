//using consumerrabbit.Maping;
//using consumerrabbit.Models;
//using consumerrabbit.Models;
//using consumerrabbit.Repository;
//using consumerrabbit.Repository;
//using consumerrabbit.Services;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using RabbitMQ.Client;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//namespace consumerrabbit.Services
//{
//    public class ConsumerService
//    {
//        private readonly AlertRepository _repository;

//        public ConsumerService(AlertRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task StartAsync()
//        {
//            using var connection = await new ConnectionFactory
//            {
//                HostName = "localhost",
//                UserName = "guest",
//                Password = "guest"
//            }.CreateConnectionAsync();

//            await using var channel = await connection.CreateChannelAsync();

//            var queues = new Dictionary<string, string>
//            {
//                { "alerts_north", "North" },
//                { "alerts_center", "Center" },
//                { "alerts_south", "South" },
//                { "alerts_homefront", "HomeFront" }
//            };

//            foreach (var queue in queues)
//            {
//                await CreateConsumerAsync(
//                    channel,
//                    queue.Key,
//                    queue.Value);
//            }

//            Console.WriteLine("Listening for alerts from all commands...");

//            await Task.Delay(Timeout.Infinite);
//        }

//        private async Task CreateConsumerAsync(
//            IChannel channel,
//            string queueName,
//            string command)
//        {
//            await channel.QueueDeclareAsync(
//                queueName,
//                durable: true,
//                exclusive: false,
//                autoDelete: false);

//            var consumer = new AsyncEventingBasicConsumer(channel);

//            consumer.ReceivedAsync += async (sender, eventArgs) =>
//            {
//                try
//                {
//                    string json = Encoding.UTF8.GetString(
//                        eventArgs.Body.ToArray());

//                    Alert? alert = JsonSerializer.Deserialize<Alert>(
//                        json,
//                        new JsonSerializerOptions
//                        {
//                            PropertyNameCaseInsensitive = true
//                        });

//                    if (alert == null || !Validate(alert))
//                    {
//                        await channel.BasicNackAsync(
//                            eventArgs.DeliveryTag,
//                            false,
//                            false);

//                        return;
//                    }

//                    alert.Command = command;

//                    await _repository.SaveAsync(alert);

//                    await channel.BasicAckAsync(
//                        eventArgs.DeliveryTag,
//                        false);

//                    Console.WriteLine(
//                        $"Alert {alert.AlertId} saved | Command: {command}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(
//                        $"Error processing alert: {ex.Message}");

//                    await channel.BasicNackAsync(
//                        eventArgs.DeliveryTag,
//                        false,
//                        false);
//                }
//            };

//            await channel.BasicConsumeAsync(
//                queueName,
//                autoAck: false,
//                consumer: consumer);
//        }

//        private static bool Validate(Alert alert)
//        {
//            if (string.IsNullOrWhiteSpace(alert.AlertId))
//                return false;

//            if (string.IsNullOrWhiteSpace(alert.Title))
//                return false;

//            if (alert.Lat < -90 || alert.Lat > 90)
//                return false;

//            if (alert.Lon < -180 || alert.Lon > 180)
//                return false;

//            return true;
//        }
//    }
//}
//using consumerrabbit.Repository;
//using Microsoft.Extensions.DependencyInjection;

//namespace consumerrabbit.Services
//{
//    public class AlertScannerService
//    {
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly TaskManagerService _taskManager;

//        public AlertScannerService(
//            IServiceScopeFactory scopeFactory,
//            TaskManagerService taskManager)
//        {
//            _scopeFactory = scopeFactory;
//            _taskManager = taskManager;
//        }

//        public async Task StartAsync()
//        {
//            while (true)
//            {
//                using (var scope = _scopeFactory.CreateScope())
//                {
//                    var repository = scope.ServiceProvider.GetRequiredService<AlertRepository>();
//                    var alerts = await repository.GetNewAlertsAsync();

//                    foreach (var alert in alerts)
//                    {
//                        await _taskManager.HandleAlertAsync(alert);
//                    }
//                }

//                await Task.Delay(1000);
//            }
//        }
//    }
//}









