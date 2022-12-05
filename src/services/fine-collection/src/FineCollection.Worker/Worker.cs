namespace FineCollection.Worker;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessageHandler _messageHandler;
    private readonly ITrafficViolationProcessor _trafficViolationProcessor;

    public Worker(
        ILogger<Worker> logger,
        IMessageHandler messageHandler,
        ITrafficViolationProcessor trafficViolationProcessor)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _trafficViolationProcessor = trafficViolationProcessor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageHandler.ConsumeAsync(HandleMessageAsync, cancellationToken);

        async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var model = ((EventingBasicConsumer) sender).Model;
            var message = eventArgs.Body.ToArray();
            var messageDecoded = Encoding.UTF8.GetString(message);
            _logger.LogInformation("Message received: {MessageDecoded}", messageDecoded);

            try
            {
                var trafficViolation = JsonSerializer.Deserialize<TrafficViolationMessage>(messageDecoded);
                ArgumentNullException.ThrowIfNull(trafficViolation);

                await _trafficViolationProcessor.ProcessAsync(trafficViolation, cancellationToken);
                
                model.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing message: {MessageDecoded}", messageDecoded);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopped");
        await Task.CompletedTask;
    }
}