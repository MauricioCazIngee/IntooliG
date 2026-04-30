namespace IntooliG.Domain.Entities;

/// <summary>tbRanking</summary>
public class CatRanking
{
    public int Id { get; set; }
    public long ConceptoId { get; set; }
    public int RubroGeneralId { get; set; }
    public int CategoriaId { get; set; }
    public int Posicion { get; set; }

    public CatConcepto? Concepto { get; set; }
}
