using Dengue.Application.DTOs;

namespace Dengue.Application.Interfaces;

public interface IDengueService{
    Task<int> SyncLastSixMonthsAsync(CancellationToken cancellationToken = default);
    Task<DengueAlertResponseDto?> GetByWeekAsync(int ew, int ey, CancellationToken cancellationToken = default);
    Task<DengueExtremosResponseDto?> GetExtremosAsync(CancellationToken cancellationToken = default);
}