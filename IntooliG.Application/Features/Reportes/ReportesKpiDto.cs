namespace IntooliG.Application.Features.Reportes;

public record ReportesKpiDto(
    int TotalCampanias,
    int CampaniasActivas,
    int CampaniasInactivas,
    int TotalUsuarios,
    int UsuariosAdmin,
    int UsuariosNormales);
