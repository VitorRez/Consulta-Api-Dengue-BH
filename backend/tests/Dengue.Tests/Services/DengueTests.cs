using System.Buffers.Text;
using System.Runtime.InteropServices;
using Dengue.Application.DTOs;
using Dengue.Application.Interfaces;
using Dengue.Application.Services;
using Dengue.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace Dengue.Tests.Services;

public class DengueServicesTests{
    private readonly Mock<IDengueRepository> _repositoryMock = new();
    private readonly Mock<IAlertaDengueClient> _clientMock = new();
    private readonly IOptions<AlertaDengueOptions> _options;

    public DengueServicesTests(){
        _options = Options.Create(new AlertaDengueOptions{
            BaseUrl = "https://info.Dengue.mat.br/api/alertcity",
            Geocode = 3106200,
            Disease = "dengue"
        });
    }

    private DengueService CreateService() => new(_repositoryMock.Object, _clientMock.Object, _options);

    //SyncLastSixMonthsAsync

    [Fact]
    public async Task SyncLastSixMonthsAsync_ShouldReturnZero_WhenApiReturnEmpty(){
        _clientMock.Setup(c => c.GetAlertsAsync(
            It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(Enumerable.Empty<AlertaDengueApiDto>());

        var service = CreateService();

        //Act
        var result = await service.SyncLastSixMonthsAsync();

        //Assert
        Assert.Equal(0, result);
        _repositoryMock.Verify(r => r.UpsertRangeAsync(It.IsAny<IEnumerable<DengueAlert>>(),It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SyncLastSixMonthsAsync_ShouldCallUpsert_WhenApiReturnsData(){
        var apiData = new List<AlertaDengueApiDto>{
            new() { SE = 202636, CasosEst = 722, Casos = 37, Nivel = 3, DataIniSE = 1756944000000 },
            new() { SE = 202635, CasosEst = 574, Casos = 144, Nivel = 3, DataIniSE = 1756339200000 },
        };

        _clientMock.Setup(c => c.GetAlertsAsync(
            It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(apiData);

        var service = CreateService();

        //Act
        var result = await service.SyncLastSixMonthsAsync();

        //Assert
        Assert.Equal(2, result);
        _repositoryMock.Verify(r => r.UpsertRangeAsync(It.Is<IEnumerable<DengueAlert>>(List => List.Count() == 2),It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SyncLastSixMonthsAsync_ShouldMapSeToEwAndEy_Correctly(){
        var apiData = new List<AlertaDengueApiDto>{
            new() {SE = 202636, CasosEst = 732, Casos = 37, Nivel = 3, DataIniSE = 176944000000}
        };

        _clientMock.Setup(c => c.GetAlertsAsync(
            It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(apiData);

        IEnumerable<DengueAlert>? captured = null;
        _repositoryMock.Setup(r => r.UpsertRangeAsync(
            It.IsAny<IEnumerable<DengueAlert>>(),
            It.IsAny<CancellationToken>()
        )).Callback<IEnumerable<DengueAlert>, CancellationToken>((list, _) => captured = list);

        var service = CreateService();

        //Act
        await service.SyncLastSixMonthsAsync();

        //Assert
        Assert.NotNull(captured);
        var alert = captured!.Single();
        Assert.Equal(36, alert.Ew);
        Assert.Equal(2026, alert.Ey);
        Assert.Equal(732, alert.CasosEst);
        Assert.Equal(37, alert.Casos);
        Assert.Equal(3, alert.Nivel);
    }

    //GetByWeekAsync

    [Fact]
    public async Task GetByWeekAsync_ShouldReturnNull_WhenRepositoryReturnNull(){
        _repositoryMock.Setup(r => r.GetByWeekAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()
        )).ReturnsAsync((DengueAlert?)null);

        var service = CreateService();

        //Act
        var result = await service.GetByWeekAsync(38, 2026); //Semana ainda não possui dados

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByWeekAsync_ShouldReturnMappedTo_WhenRepositoryReturnsEntity(){
        var entity = new DengueAlert{
            Ew = 36,
            Ey = 2026,
            DataIniSE = new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc),
            CasosEst = 722,
            Casos = 37,
            Nivel = 3
        };

        _repositoryMock.Setup(r => r.GetByWeekAsync(
            36, 2026, It.IsAny<CancellationToken>()
        )).ReturnsAsync(entity);

        var service = CreateService();

        //Act
        var result = await service.GetByWeekAsync(36, 2026);

        //Assert
        Assert.NotNull(result);
        Assert.Equal("2026-36", result!.SemanaEpidemiologica);
        Assert.Equal(722, result.CasosEst);
        Assert.Equal(37, result.CasosNotificados);
        Assert.Equal(3, result.NivelAlerta);
        Assert.Equal(new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc), result.DataInicioSemana);
    }

    //GetExtremosAsync
    [Fact]
    public async Task GetExtremosAsync_ShouldReturnNull_WhenBothRepositoriesReturnNull(){
        _repositoryMock.Setup(r => r.GetMaxNivelAsync(
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((DengueAlert?)null);

        _repositoryMock.Setup(r => r.GetMinNivelAsync(
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((DengueAlert?)null);

        var service = CreateService();

        // Act
        var result = await service.GetExtremosAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetExtremosAsync_ShouldReturnBothExtremes_WhenDataExists(){
        var max = new DengueAlert { Ew = 12, Ey = 2026, Casos = 1307, Nivel = 3 };
        var min = new DengueAlert { Ew = 34, Ey = 2026, Casos = 236, Nivel = 1 };

        _repositoryMock.Setup(r => r.GetMaxNivelAsync(
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(max);
        
        _repositoryMock.Setup(r => r.GetMinNivelAsync(
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(min);

        var service = CreateService();

        // Act
        var result = await service.GetExtremosAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("2026-12", result!.MaiorNivel!.SemanaEpidemiologica);
        Assert.Equal("2026-34", result.MenorNivel!.SemanaEpidemiologica);
    }
}