using System.Text.Json.Serialization;

namespace Dengue.Application.DTOs;

public class AlertaDengueApiDto{
    [JsonPropertyName("data_iniSE")]
    public long DataIniSE { get; set; }

    [JsonPropertyName("SE")]
    public int SE { get; set; }

    [JsonPropertyName("casos_est")]
    public double CasosEst { get; set; }

    [JsonPropertyName("casos")]
    public int Casos { get; set; }

    [JsonPropertyName("nivel")]
    public int Nivel { get; set; }

    [JsonPropertyName("p_rt1")]
    public double? PRt1 { get; set; }

    [JsonPropertyName("p_inc100k")]
    public double? PInc100k { get; set; }

    [JsonPropertyName("Rt")]
    public double? Rt { get; set; }

    [JsonPropertyName("pop")]
    public double? Pop { get; set; }

    [JsonPropertyName("receptivo")]
    public int? Receptivo { get; set; }

    [JsonPropertyName("transmissao")]
    public int? Transmissao { get; set; }

    [JsonPropertyName("nivel_inc")]
    public int? NivelInc { get; set; }

    [JsonPropertyName("notif_accum_year")]
    public int? NotifAccumYear { get; set; }
}