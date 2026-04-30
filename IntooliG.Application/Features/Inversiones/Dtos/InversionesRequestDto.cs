namespace IntooliG.Application.Features.Inversiones;

/// <summary>Parámetros de consulta para inversiones corporativas (alineado a SPs legacy).</summary>
public sealed class InversionesRequestDto
{
    /// <summary>1 = MONTHLY, 2 = WEEKLY</summary>
    public int Periodo { get; set; }

    public DateTime FechaFinal { get; set; }

    /// <summary>Opcional: validación de permisos de país.</summary>
    public int? PaisId { get; set; }

    public int? CategoriaId { get; set; }

    /// <summary>1 = FULL, 2 = AVERAGE</summary>
    public int TipoTarifa { get; set; } = 1;

    /// <summary>1 = MM, 2 = K, 3 = NORMAL</summary>
    public int Vista { get; set; } = 1;

    public int? RegionId { get; set; }

    public int? CiudadId { get; set; }

    /// <summary>1 = LOCAL, 2 = USD</summary>
    public int ExchangeRate { get; set; } = 1;

    public int? MarcaId { get; set; }

    public int? SectorId { get; set; }

    public int? BuId { get; set; }
}
