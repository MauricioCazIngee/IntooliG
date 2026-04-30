namespace IntooliG.Domain.Entities;

/// <summary>tbCatMarcaProductoFuente</summary>
public class CatMarcaProductoFuente
{
    public long Id { get; set; }
    public string NombreMarcaFuente { get; set; } = string.Empty;
    public string NombreProductoFuente { get; set; } = string.Empty;
    public int MarcaId { get; set; }
    public int ProductoId { get; set; }
    public int FuenteId { get; set; }

    public CatFuente? Fuente { get; set; }
    public CatMarca? Marca { get; set; }
    public CatProducto? Producto { get; set; }
}
