namespace FineCollection.Worker.Infra.Data.Repositories;

internal class TrafficViolationMongoRepository : ITrafficViolationRepository
{
    private readonly IMongoCollection<TrafficViolationDocument> _collection;

    public TrafficViolationMongoRepository(IMongoCollection<TrafficViolationDocument> collection) 
        => _collection = collection;

    public async Task SaveAsync(
        TrafficViolationDocument document,
        CancellationToken cancellationToken = default)
        => await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
}