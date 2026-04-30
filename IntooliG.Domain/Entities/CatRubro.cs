namespace IntooliG.Domain.Entities;

/// <summary>tbCatRubro — combinación rubro general + categoría por cliente.</summary>
public class CatRubro
{
    public long Id { get; set; }
    public int RubroGeneralId { get; set; }
    public int CategoriaId { get; set; }
    public decimal ValorRubro { get; set; }
    public bool Activo { get; set; }
    public int ClienteId { get; set; }

    public CatRubroGeneral? RubroGeneral { get; set; }
    public CatCategoria? Categoria { get; set; }
}
