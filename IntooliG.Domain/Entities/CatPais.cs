namespace IntooliG.Domain.Entities;

/// <summary>tbCatPais</summary>
public class CatPais
{
    public int Id { get; set; }
    public string NombrePais { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
