namespace TrafficControlMonitor.Worker.Events.VehicleDetected;

internal class VehicleDetectedEventHandler : IEventHandler<VehicleDetectedEvent>
{
    private const int MaximumSpeedAllowed = 110;

    private readonly ILogger<VehicleDetectedEventHandler> _logger;
    private readonly IVehicleDetectedEventRepository _vehicleDetectedEventRepository;
    private readonly IModel _channel;

    public VehicleDetectedEventHandler(
        ILogger<VehicleDetectedEventHandler> logger,
        IVehicleDetectedEventRepository vehicleDetectedEventRepository,
        IModel channel)
    {
        _logger = logger;
        _vehicleDetectedEventRepository = vehicleDetectedEventRepository;
        _channel = channel;
    }

    public async Task HandleAsync(VehicleDetectedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling VehicleDetectedEvent for Vehicle {VehicleLicensePlate}",
            @event.VehiclePlate);
        
        await Task.WhenAll(
            _vehicleDetectedEventRepository.SaveAsync(@event, cancellationToken),
            PublishIfHasTrafficViolation(@event));
    }

    private Task PublishIfHasTrafficViolation(VehicleDetectedEvent @event)
    {
        if (@event.DetectedSpeed > MaximumSpeedAllowed)
        {
            _logger.LogInformation(
                "Vehicle with plate {VehiclePlate} violated the permitted speed limit ({MaximumSpeedAllowed}KM/h) of the track passing to {DetectedSpeed}KM/h",
                @event.VehiclePlate,
                MaximumSpeedAllowed,
                @event.DetectedSpeed);

            var message = JsonSerializer.Serialize(new
            {
                timestamp = @event.Timestamp,
                lane_number = @event.LaneNumber,
                vehicle_plate = @event.VehiclePlate,
                detected_speed = @event.DetectedSpeed,
                maximum_speed_allowed = MaximumSpeedAllowed,
            });
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "traffic-violations",
                basicProperties: null,
                body: body);
        }

        return Task.CompletedTask;
    }
}