namespace IntooliG.Domain.Entities;

/// <summary>tbTipoCambio</summary>
public class CatTipoCambio
{
    public int Id { get; set; }
    public int PaisId { get; set; }
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal TipoCambio { get; set; }

    public CatPais? Pais { get; set; }
}
