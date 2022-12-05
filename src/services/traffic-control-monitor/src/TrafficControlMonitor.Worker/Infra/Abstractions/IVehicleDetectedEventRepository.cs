namespace TrafficControlMonitor.Worker.Infra.Abstractions;

public interface IVehicleDetectedEventRepository
{
    Task SaveAsync(
        VehicleDetectedEventDocument document,
        CancellationToken cancellationToken = default);
}