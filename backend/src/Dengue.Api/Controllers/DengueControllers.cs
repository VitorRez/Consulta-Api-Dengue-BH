using Dengue.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dengue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DengueController : ControllerBase{
    private readonly IDengueService _service;
    private readonly ILogger<DengueController>  _logger;

    public DengueController(IDengueService service, ILogger<DengueController> logger){
        _service = service;
        _logger = logger;
    }

    //Consulta os dados da dengue de uma semana epidemiológica específica
    [HttpGet]
    public async Task<IActionResult> GetByWeek([FromQuery] int ew, [FromQuery] int ey, CancellationToken cancellationToken){
        if(ew < 1 || ew > 53) return BadRequest("ew deve estar entre 1 e 53");
        if(ey < 2000 || ey > 2100) return BadRequest("ey inválido");

        var result = await _service.GetByWeekAsync(ew, ey, cancellationToken);

        if(result is null) return NotFound(new{message = $"Sem dados para a semana {ey}-{ew:D2}."});

        return Ok(result);
    }

    //Sincroniza od dados dos últims 6 meses a partir da API AlertaDengue
    [HttpPost("sync")]
    public async Task<IActionResult> Sync(CancellationToken cancellationToken){
        _logger.LogInformation("Iniciando sync de dados de dengue.");

        var count = await _service.SyncLastSixMonthsAsync(cancellationToken);

        return Ok(new {message = "Sincronização concluída.", recordsProcessed = count});
    }

    [HttpGet("extremos")]
    public async Task<IActionResult> GetExtremos(CancellationToken cancellationToken){
        var result = await _service.GetExtremosAsync(cancellationToken);

        if(result is null) return NotFound(new {message = "Nenhum dado no banco"});

        return Ok(result);
            
    }
}

