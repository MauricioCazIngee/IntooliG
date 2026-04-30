using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbCatBU")]
public class CatBU
{
    [Key]
    [Column("FiBUid")]
    public int Id { get; set; }

    [Column("FiSectorid")]
    public int SectorId { get; set; }

    [Column("FiClienteid")]
    public int ClienteId { get; set; }

    [Column("FcNombreBU")]
    public string NombreBU { get; set; } = string.Empty;

    [Column("FbEstatus")]
    public bool Activo { get; set; }
}
