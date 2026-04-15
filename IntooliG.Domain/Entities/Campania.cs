using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

public class Campania
{
    [Key]
    [Column("FiCampaniaid")]
    public int Id { get; set; }

    [Column("FcNombreCampania")]
    public string Nombre { get; set; } = string.Empty;

    [Column("FbActivo")]
    public bool Activa { get; set; } = true;

    [Column("FiAnioCreacion")]
    public int AnioCreacion { get; set; }

    [Column("FiClienteid")]
    public int ClienteId { get; set; }

    [Column("FiPaisid")]
    public int PaisId { get; set; }

    [NotMapped]
    public string Codigo
    {
        get => Id.ToString();
        set { }
    }

    [NotMapped]
    public string? Descripcion { get; set; }

    [NotMapped]
    public DateTime? FechaInicio { get; set; }

    [NotMapped]
    public DateTime? FechaFin { get; set; }

    [NotMapped]
    public DateTime FechaCreacionUtc
    {
        get => new DateTime(Math.Max(AnioCreacion, 1900), 1, 1);
        set => AnioCreacion = value.Year;
    }
}
