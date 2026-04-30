using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbCatCliente")]
public class CatCliente
{
    [Key]
    [Column("FiClienteid")]
    public int Id { get; set; }

    [Column("FcNombreCliente")]
    public string Nombre { get; set; } = string.Empty;

    [Column("FbEstatus")]
    public bool Activo { get; set; }

    [Column("FcAvisoPrivacidad")]
    public string? AvisoPrivacidad { get; set; }
}
