namespace IntooliG.Domain.Entities;

/// <summary>tbAgrupacionRegion (región ↔ ciudad)</summary>
public class AgrupacionRegion
{
    public int RegionId { get; set; }
    public int CiudadId { get; set; }

    public CatRegion? Region { get; set; }
    public CatCiudad? Ciudad { get; set; }
}
