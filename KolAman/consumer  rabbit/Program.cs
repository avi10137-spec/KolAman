//// See https://aka.ms/new-console-template for more information


//public class Alert
//{
//    public int Id { get; set; }
//    public string AlertId { get; set; } = "";
//    public string Source { get; set; } = "";
//    public string Title { get; set; } = "";
//    public string Content { get; set; } = "";
//    public string Priority { get; set; } = "";
//    public string Classification { get; set; } = "";
//    public double Lat { get; set; }
//    public double Lon { get; set; }
//    public DateTime Timestamp { get; set; }
//    public string Status { get; set; } = "";
//    public string Command { get; set; } = "";
//}

//using Microsoft.EntityFrameworkCore;

//public class AlertDbContext : DbContext
//{
//    public DbSet<Alert> Alerts => Set<Alert>();

//    protected override void OnConfiguring(
//        DbContextOptionsBuilder options)
//    {
//        options.UseMySql(
//            "Server=localhost;Port=3306;Database=alerts_db;User=root;Password=root;",
//            ServerVersion.AutoDetect(
//                "Server=localhost;Port=3306;Database=alerts_db;User=root;Password=root;"
//            )
//        );
//    }
//}
//public class AlertRepository
//{
//    public async Task SaveAsync(Alert alert)
//    {
//        await using var db = new AlertDbContext();

//        db.Alerts.Add(alert);

//        await db.SaveChangesAsync();
//    }
//}
//_____________________using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//var repository = new AlertRepository();

//await using var connection = await new ConnectionFactory
//{
//    HostName = "localhost",
//    UserName = "guest",
//    Password = "guest"
//}.CreateConnectionAsync();

//await using var channel = await connection.CreateChannelAsync();

//const string queueName = "alerts_north";

//await channel.QueueDeclareAsync(
//    queueName,
//    durable: true,
//    exclusive: false,
//    autoDelete: false
//);

//var consumer = new AsyncEventingBasicConsumer(channel);

//consumer.ReceivedAsync += async (sender, eventArgs) =>
//{
//    try
//    {
//        string json = Encoding.UTF8.GetString(
//            eventArgs.Body.ToArray());

//        Alert? alert = JsonSerializer.Deserialize<Alert>(
//            json,
//            new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            });

//        if (alert == null)
//        {
//            await channel.BasicNackAsync(
//                eventArgs.DeliveryTag,
//                false,
//                false);

//            return;
//        }

//        if (!Validate(alert))
//        {
//            Console.WriteLine(
//                $"Invalid alert: {alert.AlertId}");

//            await channel.BasicNackAsync(
//                eventArgs.DeliveryTag,
//                false,
//                false);

//            return;
//        }

//        await repository.SaveAsync(alert);

//        await channel.BasicAckAsync(
//            eventArgs.DeliveryTag,
//            false);

//        Console.WriteLine(
//            $"Alert {alert.AlertId} saved");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex.Message);

//        await channel.BasicNackAsync(
//            eventArgs.DeliveryTag,
//            false,
//            false);
//    }
//});

//await channel.BasicConsumeAsync(
//    queueName,
//    autoAck: false,
//    consumer: consumer);

//Console.WriteLine("Listening for alerts...");

//await Task.Delay(Timeout.Infinite);

//static bool Validate(Alert alert)
//{
//    if (string.IsNullOrWhiteSpace(alert.AlertId))
//        return false;

//    if (string.IsNullOrWhiteSpace(alert.Title))
//        return false;

//    if (alert.Lat < -90 || alert.Lat > 90)
//        return false;

//    if (alert.Lon < -180 || alert.Lon > 180)
//        return false;

//    return true;
//}
//בתודה וברכהforeach(var queue in queues)
//{
//    await channel.QueueDeclareAsync(
//        queue,
//        durable: true,
//        exclusive: false,
//        autoDelete: false);

//    var consumer = new AsyncEventingBasicConsumer(channel);

//    consumer.ReceivedAsync += async (sender, eventArgs) =>
//    {
//        try
//        {
//            string json = Encoding.UTF8.GetString(
//                eventArgs.Body.ToArray());

//            Alert? alert = JsonSerializer.Deserialize<Alert>(
//                json);

//            if (alert == null || !Validate(alert))
//            {
//                await channel.BasicNackAsync(
//                    eventArgs.DeliveryTag,
//                    false,
//                    false);

//                return;
//            }

//            await repository.SaveAsync(alert);

//            await channel.BasicAckAsync(
//                eventArgs.DeliveryTag,
//                false);

//            Console.WriteLine(
//                $"Alert {alert.AlertId} received from {queue}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message);

//            await channel.BasicNackAsync(
//                eventArgs.DeliveryTag,
//                false,
//                false);
//        }
//    };

//    await channel.BasicConsumeAsync(
//        queue,
//        autoAck: false,
//        consumer: consumer);
//}

