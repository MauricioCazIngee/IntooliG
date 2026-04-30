namespace IntooliG.Domain.Entities;

/// <summary>tbCatVersionFuente</summary>
public class CatVersionFuente
{
    public int Id { get; set; }
    public int FuenteId { get; set; }
    public string NombreVersionFuente { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public int ProductoId { get; set; }
    public long VersionTVId { get; set; }
    public int BUId { get; set; }
    public int CategoriaId { get; set; }

    public CatFuente? Fuente { get; set; }
    public CatProducto? Producto { get; set; }
    public CatVersionTV? VersionTV { get; set; }
    public CatBU? BU { get; set; }
    public CatCategoria? Categoria { get; set; }
}
