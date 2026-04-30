using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbCatCategoria")]
public class CatCategoria
{
    [Key]
    [Column("FiCategoriaid")]
    public int Id { get; set; }

    [Column("FiClienteid")]
    public int ClienteId { get; set; }

    [Column("FcNombreCategoria")]
    public string NombreCategoria { get; set; } = string.Empty;

    [Column("FcNombreCorto")]
    public string? NombreCorto { get; set; }

    [Column("FbEstatus")]
    public bool Activo { get; set; }
}
