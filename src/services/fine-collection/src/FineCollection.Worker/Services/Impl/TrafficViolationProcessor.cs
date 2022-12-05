namespace FineCollection.Worker.Services.Impl;

internal class TrafficViolationProcessor : ITrafficViolationProcessor
{
    private readonly ILogger<TrafficViolationProcessor> _logger;
    private readonly ITrafficViolationRepository _trafficViolationRepository;
    private readonly IVehicleRegistrationApiClient _vehicleRegistrationApiClient;

    public TrafficViolationProcessor(
        ILogger<TrafficViolationProcessor> logger,
        ITrafficViolationRepository trafficViolationRepository,
        IVehicleRegistrationApiClient vehicleRegistrationApiClient)
    {
        _logger = logger;
        _trafficViolationRepository = trafficViolationRepository;
        _vehicleRegistrationApiClient = vehicleRegistrationApiClient;
    }

    public async Task ProcessAsync(
        TrafficViolationMessage trafficViolationMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var vehicleDetails = await _vehicleRegistrationApiClient
                .GetDetailsByPlateAsync(trafficViolationMessage.VehiclePlate, cancellationToken);

            var trafficViolation = TrafficViolationDocument.From(trafficViolationMessage, vehicleDetails);
            await _trafficViolationRepository.SaveAsync(trafficViolation, cancellationToken);
            
            // TODO: Send email to the vehicle owner
            
            _logger.LogInformation("Traffic violation processed successfully");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error processing traffic violation message");
        }
    }
}