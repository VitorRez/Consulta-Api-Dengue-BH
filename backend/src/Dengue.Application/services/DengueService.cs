using Dengue.Application.DTOs;
using Dengue.Application.Interfaces;
using Dengue.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Dengue.Application.Services;

public class DengueService : IDengueService{
    private readonly IDengueRepository _repository;
    private readonly IAlertaDengueClient _client;
    private readonly AlertaDengueOptions _options;

    public DengueService(
        IDengueRepository repository,
        IAlertaDengueClient client,
        IOptions<AlertaDengueOptions> options)
    {
        _repository = repository;
        _client = client;
        _options = options.Value;
    }

    public async Task<int> SyncLastSixMonthsAsync(CancellationToken cancellationToken = default){
        var (ewStart, eyStart, ewEnd, eyEnd) = CalculateLastSixMonthsEpiWeeks();

        var apiData = await _client.GetAlertsAsync(
            _options.Geocode,
            _options.Disease,
            ewStart, ewEnd, eyStart, eyEnd,
            cancellationToken);

        var alerts = apiData.Select(MapToEntity).ToList();

        if (alerts.Count == 0) return 0;

        await _repository.UpsertRangeAsync(alerts, cancellationToken);
        return alerts.Count;
    }

    public async Task<DengueAlertResponseDto?> GetByWeekAsync(int ew, int ey, CancellationToken cancellationToken = default){
        var alert = await _repository.GetByWeekAsync(ew, ey, cancellationToken);
        return alert is null ? null : MapToDto(alert);
    }

    private static (int ewStart, int eyStart, int ewEnd, int eyEnd) CalculateLastSixMonthsEpiWeeks(){
        var today = DateTime.UtcNow.Date;
        var start = today.AddMonths(-6);

        var startWeek = System.Globalization.ISOWeek.GetWeekOfYear(start);
        var startYear = System.Globalization.ISOWeek.GetYear(start);
        var endWeek = System.Globalization.ISOWeek.GetWeekOfYear(today);
        var endYear = System.Globalization.ISOWeek.GetYear(today);

        return (startWeek, startYear, endWeek, endYear);
    }

    private static DengueAlert MapToEntity(AlertaDengueApiDto dto){
        // SE vem como 202501 → ey = 2025, ew = 01
        var ey = dto.SE / 100;
        var ew = dto.SE % 100;

        return new DengueAlert{
            Ew = ew,
            Ey = ey,
            DataIniSE = DateTimeOffset.FromUnixTimeMilliseconds(dto.DataIniSE).UtcDateTime,
            CasosEst = dto.CasosEst,
            Casos = dto.Casos,
            Nivel = dto.Nivel,
            PRt1 = dto.PRt1,
            PInc100k = dto.PInc100k,
            Rt = dto.Rt,
            Pop = dto.Pop,
            Receptivo = dto.Receptivo,
            Transmissao = dto.Transmissao,
            NivelInc = dto.NivelInc,
            NotifAccumYear = dto.NotifAccumYear,
            ImportedAt = DateTime.UtcNow
        };
    }

    private static DengueAlertResponseDto MapToDto(DengueAlert entity){
        return new DengueAlertResponseDto{
            SemanaEpidemiologica = $"{entity.Ey}-{entity.Ew:D2}",
            DataInicioSemana = entity.DataIniSE,
            CasosEst = entity.CasosEst,
            CasosNotificados = entity.Casos,
            NivelAlerta = entity.Nivel
        };
    }

    public async Task<DengueExtremosResponseDto?> GetExtremosAsync(CancellationToken cancellationToken = default){
        var max = await _repository.GetMaxNivelAsync(cancellationToken);
        var min = await _repository.GetMinNivelAsync(cancellationToken);

        if(max is null && min is null) return null;

        return new DengueExtremosResponseDto{
            MaiorNivel = max is null ? null : MapToDto(max),
            MenorNivel = min is null ? null : MapToDto(min),
        };
    }
}