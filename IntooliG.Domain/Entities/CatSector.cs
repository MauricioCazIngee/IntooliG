using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbCatSector")]
public class CatSector
{
    [Key]
    [Column("FiSectorid")]
    public int Id { get; set; }

    [Column("FiClienteid")]
    public int ClienteId { get; set; }

    [Column("FcNombreSector")]
    public string NombreSector { get; set; } = string.Empty;

    [Column("FbEstatus")]
    public bool Activo { get; set; }
}
