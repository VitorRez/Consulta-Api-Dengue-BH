using System.Net.Http.Json;
using System.Xml;
using Dengue.Application.DTOs;
using Dengue.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dengue.Infrastructure.External;

public class AlertaDengueClient : IAlertaDengueClient{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlertaDengueClient> _logger;

    public AlertaDengueClient(HttpClient httpClient, ILogger<AlertaDengueClient> logger){
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<AlertaDengueApiDto>> GetAlertsAsync(
        int geocode,
        string disease,
        int ewStart,
        int ewEnd,
        int eyStart,
        int eyEnd,
        CancellationToken cancellationToken = default
    ){
        var url = $"?geocode={geocode}&disease={disease}&format=json&ew_start={ewStart}&ew_end={ewEnd}&ey_start={eyStart}&ey_end={eyEnd}";
        _logger.LogInformation("Consultando AlertaDengue: {Url}", url);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        var data = await response.Content.ReadFromJsonAsync<List<AlertaDengueApiDto>>(cancellationToken);

        return data ?? new List<AlertaDengueApiDto>();
    }
}