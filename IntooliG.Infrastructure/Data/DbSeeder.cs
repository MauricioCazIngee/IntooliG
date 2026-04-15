using IntooliG.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntooliG.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher<Usuario> passwordHasher, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (!await db.Usuarios.AnyAsync(cancellationToken))
        {
            var ahora = DateTime.UtcNow;
            var admin = new Usuario
            {
                Email = "admin@demo.local",
                Nombre = "Administrador",
                Apellidos = "Sistema",
                ClienteId = 1,
                RolId = 1,
                RolPbiId = null,
                Estatus = true,
                CodigoActivacion = Guid.NewGuid(),
                CorreoVerificado = true,
                Intentos = null,
                ResetPasswordCode = null,
                PrimerAcceso = false,
                ExpiracionPassword = null,
                FechaNacimiento = null,
                FechaCreacion = ahora
            };
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Demo123!");
            db.Usuarios.Add(admin);
            logger.LogInformation("Usuario semilla creado: {Email}", admin.Email);
        }

        if (!await db.Campanias.AnyAsync(cancellationToken))
        {
            db.Campanias.AddRange(
                new Campania
                {
                    Nombre = "Campaña de bienvenida",
                    Activa = true,
                    AnioCreacion = DateTime.UtcNow.Year,
                    ClienteId = 1,
                    PaisId = 1
                },
                new Campania
                {
                    Nombre = "Campaña inactiva",
                    Activa = false,
                    AnioCreacion = DateTime.UtcNow.Year,
                    ClienteId = 1,
                    PaisId = 1
                });
            logger.LogInformation("Campañas semilla creadas.");
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
