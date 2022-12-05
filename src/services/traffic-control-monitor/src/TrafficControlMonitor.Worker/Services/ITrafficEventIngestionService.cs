namespace TrafficControlMonitor.Worker.Services;

public interface ITrafficEventIngestionService
{
    Task ExecuteAsync(
        string eventMessage,
        TrafficEventType trafficEventType,
        CancellationToken cancellationToken = default);
}