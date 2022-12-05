namespace TrafficControlMonitor.Worker.Services.Impl;

internal class TrafficEventIngestionService : ITrafficEventIngestionService
{
    private readonly Func<TrafficEventType, Expression<Func<IEvent, CancellationToken, Task>>> _eventHandlerResolver;

    public TrafficEventIngestionService(Func<TrafficEventType, Expression<Func<IEvent, CancellationToken, Task>>> eventHandlerResolver)
        => _eventHandlerResolver = eventHandlerResolver;

    public async Task ExecuteAsync(
        string eventMessage,
        TrafficEventType trafficEventType,
        CancellationToken cancellationToken = default)
    {
        var (@event, eventHandlerDelegate) = trafficEventType switch
        {
            TrafficEventType.VehicleDetected => ValueTuple.Create<IEvent, Func<IEvent, CancellationToken, Task>>(
                new VehicleDetectedEvent(eventMessage),
                _eventHandlerResolver(trafficEventType).Compile()),
            _ => throw new ArgumentOutOfRangeException(nameof(trafficEventType), trafficEventType, null)
        };

        await eventHandlerDelegate(@event, cancellationToken);
    }
}