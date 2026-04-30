using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

/// <summary>tbCatVersionTV</summary>
[Table("tbCatVersionTV")]
public class CatVersionTV
{
    [Key]
    [Column("FiVersionTV")]
    public long Id { get; set; }

    [Column("FcNombreVersionTV")]
    public string Nombre { get; set; } = string.Empty;
}
