using IntooliG.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IntooliG.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICampaniaService, CampaniaService>();
        services.AddScoped<IRadarService, RadarService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IReporteService, ReporteService>();
        return services;
    }
}
