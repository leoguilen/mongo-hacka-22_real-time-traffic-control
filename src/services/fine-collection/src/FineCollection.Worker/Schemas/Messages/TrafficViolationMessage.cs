#nullable disable
namespace FineCollection.Worker.Schemas.Messages;

public record TrafficViolationMessage : IMessage
{
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("lane_number")]
    public int LaneNumber { get; init; }

    [JsonPropertyName("vehicle_plate")]
    public string VehiclePlate { get; init; }

    [JsonPropertyName("detected_speed")]
    public float DetectedSpeed { get; init; }

    [JsonPropertyName("maximum_speed_allowed")]
    public float MaximumSpeedAllowed { get; init; }
}