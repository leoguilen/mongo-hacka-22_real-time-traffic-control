namespace FineCollection.Worker.Infra.Brokers.RabbitMq;

internal class RabbitMqMessageHandler : IMessageHandler
{
    private readonly ILogger<RabbitMqMessageHandler> _logger;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqMessageHandler(
        ILogger<RabbitMqMessageHandler> logger,
        IModel channel,
        IOptions<AmqpBrokerConfig> amqpBrokerConfig)
    {
        _logger = logger;
        _channel = channel;
        _queueName = amqpBrokerConfig.Value.QueueName;
    }

    public Task ConsumeAsync(
        Func<object, BasicDeliverEventArgs, Task> consumerHandler,
        CancellationToken cancellationToken = default)
    {
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, ea) => consumerHandler(sender, ea);

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        _logger.LogInformation("RabbitMqMessageHandler started consuming messages from queue {QueueName}", _queueName);
        return Task.CompletedTask;
    }
}