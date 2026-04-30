using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbCatProducto")]
public class CatProducto
{
    [Key]
    [Column("FiProductoid")]
    public int Id { get; set; }

    [Column("FiMarcaid")]
    public int MarcaId { get; set; }
    public CatMarca? Marca { get; set; }

    [Column("FcNombreProductoGenerico")]
    public string Nombre { get; set; } = string.Empty;

    [Column("FbActivo")]
    public bool Activo { get; set; }
}
