namespace IntooliG.Application.Features.Radar.Dtos;

public class CampaniaEvaluacionDto
{
    public int CampaniaId { get; set; }
    public string Campania { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string BU { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public int Anio { get; set; }
    public bool Activo { get; set; }
}
