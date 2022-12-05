namespace FineCollection.Worker.Interfaces;

public interface IVehicleRegistrationApiClient
{
    Task<VehicleRegistrationDetailResponse> GetDetailsByPlateAsync(
        string plate,
        CancellationToken cancellationToken = default);
}