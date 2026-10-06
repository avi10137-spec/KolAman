using consumerrabbit.Models.consumerrabbit.Models;
using consumerrabbit.Repository;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace consumerrabbit.Services
{
    public class ConsumerService
    {
        private readonly AlertRepository _repository;
        private readonly IConfiguration _configuration;

        public ConsumerService(
            AlertRepository repository,
            IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task StartAsync()
        {
            var host = _configuration["RabbitMQ:Host"] ?? "localhost";
            var username = _configuration["RabbitMQ:Username"] ?? "guest";
            var password = _configuration["RabbitMQ:Password"] ?? "guest";

            var connectionFactory = new ConnectionFactory
            {
                HostName = host,
                UserName = username,
                Password = password
            };

            using var connection = await connectionFactory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            var queues = new Dictionary<string, string>
            {
                { "North Command", "NorthCommand" },
                { "Center Command", "CenterCommand" },
                { "South Command", "SouthCommand" },
                { "Depth Command", "DepthCommand" }
            };

            foreach (var queue in queues)
            {
                await CreateConsumerAsync(channel, queue.Key, queue.Value);
            }

            Console.WriteLine("Consumer started. Listening for alerts from all commands...");

            await Task.Delay(Timeout.Infinite);
        }

        private async Task CreateConsumerAsync(
            IChannel channel,
            string queueName,
            string command)
        {
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            Console.WriteLine($"Starting consumer for queue: {queueName}");

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                try
                {
                    string json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    Console.WriteLine($"Received message from {command}");

                    Alert? alert = JsonSerializer.Deserialize<Alert>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (alert == null || !Validate(alert))
                    {
                        Console.WriteLine($"Invalid alert received from {command}");
                        await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                        return;
                    }

                    alert.Command = command;

                    await _repository.SaveAsync(alert);

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                    Console.WriteLine($"Alert {alert.AlertId} saved | Command: {command}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing alert: {ex.Message}");
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);
        }

        private static bool Validate(Alert alert)
        {
            if (string.IsNullOrWhiteSpace(alert.AlertId)) return false;
            if (string.IsNullOrWhiteSpace(alert.Title)) return false;
            if (string.IsNullOrWhiteSpace(alert.Content)) return false;
            if (alert.Lat < -90 || alert.Lat > 90) return false;
            if (alert.Lon < -180 || alert.Lon > 180) return false;

            return true;
        }
    }
}