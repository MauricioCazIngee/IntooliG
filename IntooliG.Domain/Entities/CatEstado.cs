namespace IntooliG.Domain.Entities;

/// <summary>tbCatEstado</summary>
public class CatEstado
{
    public int Id { get; set; }
    public int PaisId { get; set; }
    public string NombreEstado { get; set; } = string.Empty;
    public int? CodigoMapaId { get; set; }
    public bool Activo { get; set; } = true;

    public CatPais? Pais { get; set; }
    public CatCodigoMapa? CodigoMapa { get; set; }
}
