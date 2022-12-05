#nullable disable
namespace FineCollection.Worker.Schemas.Documents;

public record TrafficViolationDocument
{
    [BsonElement("TIMESTAMP"), BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset Timestamp { get; init; }

    [BsonElement("LANE_NUMBER")] public int LaneNumber { get; init; }

    [BsonElement("DETECTED_SPEED")] public float DetectedSpeed { get; init; }

    [BsonElement("MAXIMUM_SPEED_ALLOWED")] public float MaximumSpeedAllowed { get; init; }

    [BsonElement("VEHICLE")] public VehicleDocument Vehicle { get; init; }

    public static TrafficViolationDocument From(
        TrafficViolationMessage trafficViolation,
        VehicleRegistrationDetailResponse vehicle)
        => new()
        {
            Timestamp = trafficViolation.Timestamp,
            LaneNumber = trafficViolation.LaneNumber,
            DetectedSpeed = trafficViolation.DetectedSpeed,
            MaximumSpeedAllowed = trafficViolation.MaximumSpeedAllowed,
            Vehicle = VehicleDocument.From(vehicle)
        };
}

public record VehicleDocument
{
    [BsonElement("VEHICLE_ID"), BsonRepresentation(BsonType.String)]
    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    public Guid VehicleId { get; init; }

    [BsonElement("LICENCE_PLATE")] public string LicencePlate { get; init; }

    [BsonElement("MAKE")] public string Make { get; init; }

    [BsonElement("MODEL")] public string Model { get; init; }

    [BsonElement("COLOR")] public string Color { get; init; }

    [BsonElement("OWNER_NAME")] public string OwnerName { get; init; }

    [BsonElement("OWNER_EMAIL")] public string OwnerEmail { get; init; }

    public static VehicleDocument From(VehicleRegistrationDetailResponse vehicle)
        => new()
        {
            VehicleId = vehicle.Id,
            LicencePlate = vehicle.LicensePlate,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Color = vehicle.Color,
            OwnerName = vehicle.OwnerName,
            OwnerEmail = vehicle.OwnerEmail
        };
}