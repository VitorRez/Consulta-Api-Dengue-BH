namespace Dengue.Application.DTOs;

public class DengueAlertResponseDto{
    public string SemanaEpidemiologica {get; set;} = string.Empty;
    public double CasosEst {get; set;}
    public int CasosNotificados {get; set;}
    public int NivelAlerta {get; set;}
    public DateTime DataInicioSemana {get; set;}
}