using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

/// <summary>Catálogo de roles PBI (RLS), tabla legado tbCatRolesPBI.</summary>
[Table("tbCatRolesPBI")]
public class CatRolPbi
{
    [Key]
    [Column("FiRolPbiID")]
    public int Id { get; set; }

    [Column("FcNombreRolPBI")]
    [MaxLength(100)]
    public string NombreRol { get; set; } = string.Empty;
}
