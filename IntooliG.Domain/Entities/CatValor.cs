namespace IntooliG.Domain.Entities;

/// <summary>tbCatValor</summary>
public class CatValor
{
    public int Id { get; set; }
    public int RubroGeneralId { get; set; }
    public int CategoriaId { get; set; }
    public int Posicion { get; set; }
    public decimal Valor { get; set; }
}
