using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TomeApp.API.Services;

public class RabbitMQService : IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private const string SyncQueue = "tome.sync.sessions";
    private readonly ILogger<RabbitMQService> _logger;
    private readonly IConfiguration _config;

    public RabbitMQService(ILogger<RabbitMQService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    private async Task EnsureConnectedAsync()
    {
        if (_connection?.IsOpen == true) return;

        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "rabbitmq",
            UserName = _config["RabbitMQ:Username"] ?? "tomeapp",
            Password = _config["RabbitMQ:Password"] ?? "tomeapp_secret",
            AutomaticRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(SyncQueue, durable: true, exclusive: false, autoDelete: false);
    }

    public async Task PublishSyncBatchAsync<T>(T payload)
    {
        await EnsureConnectedAsync();

        var json = JsonSerializer.Serialize(payload);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties { Persistent = true };
        await _channel!.BasicPublishAsync("", SyncQueue, mandatory: false, basicProperties: props, body: body);
        _logger.LogInformation("Published sync batch to RabbitMQ");
    }

    public async Task StartConsumingAsync(Func<string, Task> handler, CancellationToken ct)
    {
        await EnsureConnectedAsync();

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            try
            {
                await handler(body);
                await _channel!.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process sync message");
                await _channel!.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel!.BasicConsumeAsync(SyncQueue, autoAck: false, consumer: consumer, cancellationToken: ct);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
