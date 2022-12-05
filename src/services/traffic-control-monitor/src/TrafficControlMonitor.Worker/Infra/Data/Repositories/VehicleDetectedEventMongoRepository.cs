namespace TrafficControlMonitor.Worker.Infra.Data.Repositories;

internal class VehicleDetectedEventMongoRepository : IVehicleDetectedEventRepository
{
    private readonly IMongoCollection<VehicleDetectedEventDocument> _collection;

    public VehicleDetectedEventMongoRepository(IMongoCollection<VehicleDetectedEventDocument> collection)
        => _collection = collection;

    public async Task SaveAsync(VehicleDetectedEventDocument document, CancellationToken cancellationToken = default)
        => await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
}