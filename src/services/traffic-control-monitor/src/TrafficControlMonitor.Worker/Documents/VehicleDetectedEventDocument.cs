#nullable disable
namespace TrafficControlMonitor.Worker.Documents;

public record VehicleDetectedEventDocument
{
    [BsonElement("TIMESTAMP"), BsonRepresentation(BsonType.DateTime)] public DateTimeOffset Timestamp { get; init; }

    [BsonElement("LANE_NUMBER")] public int LaneNumber { get; init; }

    [BsonElement("VEHICLE_PLATE")] public string VehiclePlate { get; init; }

    [BsonElement("DETECTED_SPEED")] public float DetectedSpeed { get; init; }
    
    [BsonElement("IS_SPEEDING")] public bool IsSpeeding { get; init; }

    public static implicit operator VehicleDetectedEventDocument(VehicleDetectedEvent @event)
        => new()
        {
            Timestamp = @event.Timestamp,
            LaneNumber = @event.LaneNumber,
            VehiclePlate = @event.VehiclePlate,
            DetectedSpeed = @event.DetectedSpeed,
            IsSpeeding = @event.DetectedSpeed > 90
        };
}