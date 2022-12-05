namespace FineCollection.Worker.Services;

public interface ITrafficViolationProcessor
{
    Task ProcessAsync(
        TrafficViolationMessage trafficViolationMessage,
        CancellationToken cancellationToken = default);
}