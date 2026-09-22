namespace Dengue.Application.Services;

public class AlertaDengueOptions{
    public const string SectionName = "AlertaDengue";
    public string BaseUrl {get; set;} = string.Empty;
    public int Geocode {get; set;} 

    public string Disease {get; set;} = "dengue";
}
