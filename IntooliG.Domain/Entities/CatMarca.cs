using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbCatMarca")]
public class CatMarca
{
    [Key]
    [Column("FiMarcaid")]
    public int Id { get; set; }

    [Column("FiClienteid")]
    public int ClienteId { get; set; }

    [Column("FcNombreMarcaCorto")]
    public string NombreMarca { get; set; } = string.Empty;

    [Column("FbEstatus")]
    public bool Activo { get; set; }

    [Column("FbLogomarca")]
    public byte[]? Logo { get; set; }

    [Column("FcColor")]
    public string? Color { get; set; }

    [Column("FcTipoMarca")]
    public string? TipoMarca { get; set; }
}
