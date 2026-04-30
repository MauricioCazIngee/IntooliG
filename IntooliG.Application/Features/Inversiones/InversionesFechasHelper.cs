namespace IntooliG.Application.Features.Inversiones;

public static class InversionesFechasHelper
{
    /// <summary>
    /// Monthly: final − 23 meses; Weekly: final − 709 días (definición del sistema legacy).
    /// </summary>
    public static (DateTime StartDate, DateTime FinalDate) CalcularRango(int periodo, DateTime fechaFinal)
    {
        var f = fechaFinal.Date;
        return periodo switch
        {
            1 => (f.AddMonths(-23), f),
            2 => (f.AddDays(-709), f),
            _ => throw new InvalidOperationException("Periodo no válido (use 1=MONTHLY, 2=WEEKLY).")
        };
    }
}
