using IntooliG.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IntooliG.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICampaniaService, CampaniaService>();
        services.AddScoped<IGenerarEvaluacionService, GenerarEvaluacionService>();
        services.AddScoped<IRadarService, RadarService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<ISectorService, SectorService>();
        services.AddScoped<IBUService, BUService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IMarcaService, MarcaService>();
        services.AddScoped<IRubroGeneralService, RubroGeneralService>();
        services.AddScoped<IRubroService, RubroService>();
        services.AddScoped<IConceptoService, ConceptoService>();
        services.AddScoped<IPaisService, PaisService>();
        services.AddScoped<IEstadoService, EstadoService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<ICiudadService, CiudadService>();
        services.AddScoped<IPoblacionService, PoblacionService>();
        services.AddScoped<IMedioService, MedioService>();
        services.AddScoped<IMedioClienteService, MedioClienteService>();
        services.AddScoped<IFuenteService, FuenteService>();
        services.AddScoped<IMarcaFuenteService, MarcaFuenteService>();
        services.AddScoped<IVersionFuenteService, VersionFuenteService>();
        services.AddScoped<ITipoCambioService, TipoCambioService>();
        services.AddScoped<IDayPartService, DayPartService>();
        services.AddScoped<IInversionesService, InversionesService>();
        services.AddScoped<IInformationLoadService, InformationLoadService>();
        return services;
    }
}
