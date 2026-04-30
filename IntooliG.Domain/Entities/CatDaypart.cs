using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

/// <summary>tbCatDaypart</summary>
[Table("tbCatDaypart")]
public class CatDaypart
{
    [Key]
    [Column("FiDaypartid")]
    public long Id { get; set; }

    [Column("FiClienteid")]
    public int ClienteId { get; set; }

    [Column("FiPaisid")]
    public int PaisId { get; set; }

    [Column("FcNombreDaypart")]
    public string Nombre { get; set; } = string.Empty;

    [Column("FnHoraInicio")]
    public int HoraInicio { get; set; }

    [Column("FnHoraFin")]
    public int HoraFin { get; set; }

    [Column("FiMedioid")]
    public int MedioId { get; set; }

    public CatCliente? Cliente { get; set; }
    public CatPais? Pais { get; set; }
    public CatMedio? Medio { get; set; }
}
