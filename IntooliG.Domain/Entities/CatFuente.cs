namespace IntooliG.Domain.Entities;

/// <summary>tbCatFuente</summary>
public class CatFuente
{
    public int Id { get; set; }
    public string NombreFuente { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public int PaisId { get; set; }

    public CatPais? Pais { get; set; }
}
