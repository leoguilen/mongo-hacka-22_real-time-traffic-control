#nullable disable
namespace TrafficControlMonitor.Worker.Events.VehicleDetected;

public sealed record VehicleDetectedEvent : IEvent
{
    public VehicleDetectedEvent()
    {
    }

    public VehicleDetectedEvent(string eventMessage)
    {
        var deserializedEvent = JsonSerializer.Deserialize<VehicleDetectedEvent>(eventMessage);
        ArgumentNullException.ThrowIfNull(deserializedEvent);

        Timestamp = deserializedEvent.Timestamp;
        LaneNumber = deserializedEvent.LaneNumber;
        VehiclePlate = deserializedEvent.VehiclePlate;
        DetectedSpeed = deserializedEvent.DetectedSpeed;
    }

    [JsonPropertyName("timestamp")] public DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("lane_number")] public int LaneNumber { get; init; }

    [JsonPropertyName("vehicle_plate")] public string VehiclePlate { get; init; }

    [JsonPropertyName("detected_speed")] public float DetectedSpeed { get; init; }
}