namespace IntooliG.Domain.Entities;

/// <summary>tbMedioCliente</summary>
public class MedioCliente
{
    public int MedioId { get; set; }
    public int ClienteId { get; set; }
    public bool EsNacional { get; set; }

    public CatMedio? Medio { get; set; }
    public CatCliente? Cliente { get; set; }
}
