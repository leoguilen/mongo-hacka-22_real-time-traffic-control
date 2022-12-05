namespace TrafficControlMonitor.Worker.Infra.Abstractions;

public interface IEventBus
{
    Task StartConsumeAsync(
        Func<string, TrafficEventType, CancellationToken, Task> messageReceivedHandler,
        CancellationToken cancellationToken);

    Task StopConsumeAsync(CancellationToken cancellationToken);
}