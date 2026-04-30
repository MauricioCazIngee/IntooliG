namespace IntooliG.Application.Features.Radar.Dtos;

public class AdministracionCampaniaDto
{
    public int AdministracionId { get; set; }
    public string Sector { get; set; } = string.Empty;
    public string UnidadNegocio { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Vehiculo { get; set; } = string.Empty;
    public string Campania { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Semanas { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
