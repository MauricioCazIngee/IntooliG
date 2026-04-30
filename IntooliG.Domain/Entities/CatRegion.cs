namespace IntooliG.Domain.Entities;

/// <summary>tbCatRegion</summary>
public class CatRegion
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreRegion { get; set; } = string.Empty;
    public bool EsNacional { get; set; }
    public bool Activo { get; set; } = true;
    public int PaisId { get; set; }

    public CatPais? Pais { get; set; }
}
