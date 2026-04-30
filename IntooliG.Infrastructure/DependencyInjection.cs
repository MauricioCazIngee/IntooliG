using System.Text;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Abstractions.Security;
using IntooliG.Application.Abstractions.Services;
using IntooliG.Application.Options;
using IntooliG.Application.Services;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using IntooliG.Infrastructure.Repositories;
using IntooliG.Infrastructure.Security;
using IntooliG.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IntooliG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<PowerBiOptions>(configuration.GetSection(PowerBiOptions.SectionName));
        services.AddHttpClient<IPowerBiEmbedService, PowerBiEmbedService>();

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'ConnectionStrings:Default'.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ICatRolPbiRepository, CatRolPbiRepository>();
        services.AddScoped<ICampaniaRepository, CampaniaRepository>();
        services.AddScoped<IGenerarEvaluacionRepository, GenerarEvaluacionRepository>();
        services.AddScoped<IRadarRepository, RadarRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISectorRepository, SectorRepository>();
        services.AddScoped<IBURepository, BURepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<MarcaRepository>();
        services.AddScoped<IMarcaRepository>(sp => sp.GetRequiredService<MarcaRepository>());
        services.AddScoped<IProductoRepository>(sp => sp.GetRequiredService<MarcaRepository>());
        services.AddScoped<IRubroGeneralRepository, RubroGeneralRepository>();
        services.AddScoped<IRubroRepository, RubroRepository>();
        services.AddScoped<IConceptoRepository, ConceptoRepository>();
        services.AddScoped<IPaisRepository, PaisRepository>();
        services.AddScoped<IEstadoRepository, EstadoRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<ICiudadRepository, CiudadRepository>();
        services.AddScoped<IPoblacionRepository, PoblacionRepository>();
        services.AddScoped<IMedioRepository, MedioRepository>();
        services.AddScoped<IMedioClienteRepository, MedioClienteRepository>();
        services.AddScoped<IFuenteRepository, FuenteRepository>();
        services.AddScoped<IMarcaFuenteRepository, MarcaFuenteRepository>();
        services.AddScoped<IVersionFuenteRepository, VersionFuenteRepository>();
        services.AddScoped<IVersionTVRepository, VersionTVRepository>();
        services.AddScoped<ITipoCambioRepository, TipoCambioRepository>();
        services.AddScoped<IDayPartRepository, DayPartRepository>();
        services.AddScoped<IInversionesRepository, InversionesRepository>();
        services.AddScoped<IInformationLoadRepository, InformationLoadRepository>();
        services.AddScoped<IClienteCatalogRepository, ClienteCatalogRepository>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IAuthService, AuthService>();

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException($"Falta la sección de configuración '{JwtOptions.SectionName}'.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }
}
