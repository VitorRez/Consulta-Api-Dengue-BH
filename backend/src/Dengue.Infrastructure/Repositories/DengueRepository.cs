using System.Xml;
using Dengue.Application.Interfaces;
using Dengue.Domain.Entities;
using Dengue.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dengue.Infrastructure.Repositories;

public class DengueRepository : IDengueRepository{
    private readonly DengueDbContext _context;

    public DengueRepository(DengueDbContext context){
        _context = context;
    }

    public async Task<DengueAlert?> GetByWeekAsync(int ew, int ey, CancellationToken cancellationToken = default){
        return await _context.DengueAlerts.AsNoTracking().FirstOrDefaultAsync(x => x.Ew == ew && x.Ey == ey, cancellationToken);
    }

    public async Task<List<DengueAlert>> GetLastWeeksAsync(int count, CancellationToken cancellationToken = default){
        return await _context.DengueAlerts
            .AsNoTracking()
            .OrderByDescending(x => x.Ey)
            .ThenByDescending(x => x.Ew)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<DengueAlert?> GetMaxNivelAsync(CancellationToken cancellationToken = default){
        return await _context.DengueAlerts
            .AsNoTracking()
            .OrderByDescending(x => x.Casos)
            .ThenByDescending(x => x.Ey)
            .ThenByDescending(x => x.Ew)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DengueAlert?> GetMinNivelAsync(CancellationToken cancellationToken = default){
        return await _context.DengueAlerts
            .AsNoTracking()
            .OrderBy(x => x.Casos)
            .ThenByDescending(x => x.Ey)
            .ThenByDescending(x => x.Ew)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpsertRangeAsync(IEnumerable<DengueAlert> alerts, CancellationToken cancellationToken = default){
        var list = alerts.ToList();
        if (list.Count == 0) return;

        var ews = list.Select(x => x.Ew).Distinct().ToList();
        var eys = list.Select(x => x.Ey).Distinct().ToList();

        var existing = await _context.DengueAlerts.Where(x => ews.Contains(x.Ew) && eys.Contains(x.Ey)).ToDictionaryAsync(x => (x.Ew, x.Ey), cancellationToken);

        foreach(var alert in list){
            if(existing.TryGetValue((alert.Ew, alert.Ey), out var current)){
                // Atualiza os campos que mudam
                current.DataIniSE = alert.DataIniSE;
                current.CasosEst = alert.CasosEst;
                current.Casos = alert.Casos;
                current.Nivel = alert.Nivel;
                current.PRt1 = alert.PRt1;
                current.PInc100k = alert.PInc100k;
                current.Rt = alert.Rt;
                current.Pop = alert.Pop;
                current.Receptivo = alert.Receptivo;
                current.Transmissao = alert.Transmissao;
                current.NivelInc = alert.NivelInc;
                current.NotifAccumYear = alert.NotifAccumYear;
                current.ImportedAt = DateTime.UtcNow;
            } else {
                await _context.DengueAlerts.AddAsync(alert, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
    
}