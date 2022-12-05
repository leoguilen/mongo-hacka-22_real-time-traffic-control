#nullable disable
namespace FineCollection.Worker.Schemas.Responses;

public record VehicleRegistrationDetailResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    
    [JsonPropertyName("licensePlate")]
    public string LicensePlate { get; init; }
    
    [JsonPropertyName("make")]
    public string Make { get; init; }
    
    [JsonPropertyName("model")]
    public string Model { get; init; }
    
    [JsonPropertyName("color")]
    public string Color { get; init; }
    
    [JsonPropertyName("ownerName")]
    public string OwnerName { get; init; }

    [JsonPropertyName("ownerEmail")]
    public string OwnerEmail { get; init; }
}