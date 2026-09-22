using Dengue.Domain.Entities;

namespace Dengue.Application.Interfaces;

public interface IDengueRepository{
    Task<DengueAlert?> GetByWeekAsync(int ew, int ey, CancellationToken cancellationToken = default);
    Task<List<DengueAlert>> GetLastWeeksAsync(int count, CancellationToken cancellationToken = default);
    Task UpsertRangeAsync(IEnumerable<DengueAlert> alerts, CancellationToken cancellationToken = default);
    Task<DengueAlert?> GetMaxNivelAsync(CancellationToken cancellationToken = default);
    Task<DengueAlert?> GetMinNivelAsync(CancellationToken cancellationToken = default);
}