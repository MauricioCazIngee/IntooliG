namespace IntooliG.Domain.Entities;

/// <summary>tbPoblacion (histórico por año)</summary>
public class CatPoblacion
{
    public int Id { get; set; }
    public int Anio { get; set; }
    public int CiudadId { get; set; }
    public int Cantidad { get; set; }

    public CatCiudad? Ciudad { get; set; }
}
