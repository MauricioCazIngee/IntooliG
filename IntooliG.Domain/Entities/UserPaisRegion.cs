using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

/// <summary>Permisos de país/región por usuario (tbUserPaisRegion).</summary>
[Table("tbUserPaisRegion")]
public class UserPaisRegion
{
    [Key]
    [Column("FiUserPaisRegionid")]
    public int Id { get; set; }

    [Column("FiUsuarioId")]
    public int UsuarioId { get; set; }

    [Column("FiPaisid")]
    public int PaisId { get; set; }

    /// <summary>
    /// Si es null, el usuario puede ver todas las regiones del país indicado;
    /// si tiene valor, solo esa región.
    /// </summary>
    [Column("FiRegionid")]
    public int? RegionId { get; set; }
}
