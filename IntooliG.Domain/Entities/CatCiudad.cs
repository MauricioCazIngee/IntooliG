namespace IntooliG.Domain.Entities;

/// <summary>tbCatCiudad</summary>
public class CatCiudad
{
    public int Id { get; set; }
    public int EstadoId { get; set; }
    public string NombreCiudad { get; set; } = string.Empty;
    public string? NombreCorto { get; set; }
    public bool CiudadPrincipal { get; set; }
    public bool Activo { get; set; } = true;
    public int? Poblacion { get; set; }

    public CatEstado? Estado { get; set; }
}
