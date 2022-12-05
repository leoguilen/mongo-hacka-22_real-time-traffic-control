namespace FineCollection.Worker.Infra.ExternalServices.VehicleRegistrationApi;

internal class VehicleRegistrationApiHttpClient : IVehicleRegistrationApiClient
{
    private const string DefaultRequestUriFormat = "/api/vehicles/{0}";
    
    private readonly HttpClient _client;

    public VehicleRegistrationApiHttpClient(HttpClient client) => _client = client;

    public async Task<VehicleRegistrationDetailResponse> GetDetailsByPlateAsync(
        string plate,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetFromJsonAsync<VehicleRegistrationDetailResponse>(
                string.Format(DefaultRequestUriFormat, plate),
                cancellationToken);
        
        return response ?? throw new HttpRequestException();
    }
}