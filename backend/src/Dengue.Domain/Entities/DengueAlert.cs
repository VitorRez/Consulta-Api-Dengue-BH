namespace Dengue.Domain.Entities;

public class DengueAlert{
    public int Id {get; set;}

    //Chaves
    public int Ew {get; set;} //Semana epidemiológica
    public int Ey {get; set;} //Ano epidemiológico

    //Demais atributos
    public DateTime DataIniSE {get; set;} //Inicio da semana 
    public double CasosEst {get; set;} //Casos estimados
    public int Casos {get; set;} //Casos confirmados
    public int Nivel {get; set;}

    public double? PRt1 {get; set;}
    public double? PInc100k {get; set;}
    public double? Rt {get; set;}
    public double? Pop {get; set;}
    public int? Receptivo {get; set;}
    public int? Transmissao {get; set;}
    public int? NivelInc {get; set;}
    public int? NotifAccumYear {get; set;}

    public DateTime ImportedAt {get; set;}

}