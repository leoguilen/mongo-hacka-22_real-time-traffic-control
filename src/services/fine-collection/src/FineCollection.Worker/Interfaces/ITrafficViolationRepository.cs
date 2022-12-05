namespace FineCollection.Worker.Interfaces;

public interface ITrafficViolationRepository
{
    Task SaveAsync(
        TrafficViolationDocument document,
        CancellationToken cancellationToken = default);
}