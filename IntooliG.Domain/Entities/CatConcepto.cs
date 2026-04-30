namespace IntooliG.Domain.Entities;

/// <summary>tbCatConcepto</summary>
public class CatConcepto
{
    public long Id { get; set; }
    public int RubroGeneralId { get; set; }
    public int CategoriaId { get; set; }
    public string NombreConcepto { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool Top { get; set; }

    public CatRubroGeneral? RubroGeneral { get; set; }
    public CatCategoria? Categoria { get; set; }
}
