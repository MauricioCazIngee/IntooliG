namespace IntooliG.Domain.Entities;

/// <summary>tbCatMedio</summary>
public class CatMedio
{
    public int Id { get; set; }
    public string NombreMedio { get; set; } = string.Empty;
    public string NombreMedioGenerico { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
