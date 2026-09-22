using Dengue.Application.DTOs;

namespace Dengue.Application.Interfaces;

public interface IAlertaDengueClient{
    Task<IEnumerable<AlertaDengueApiDto>> GetAlertsAsync(
        int geoocode,
        string disease,
        int ewStart,
        int ewEnd,
        int eyStart,
        int eyEnd,
        CancellationToken cancellationToken = default
    );
}