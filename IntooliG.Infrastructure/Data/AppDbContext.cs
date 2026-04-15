using IntooliG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Campania> Campanias => Set<Campania>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.ClienteId).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            e.Property(x => x.Apellidos).HasMaxLength(150).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            e.Property(x => x.CorreoVerificado).IsRequired();
            e.Property(x => x.CodigoActivacion).IsRequired();
            e.Property(x => x.ResetPasswordCode).HasMaxLength(512);
            e.Property(x => x.PrimerAcceso).IsRequired();
            e.Property(x => x.FechaCreacion).IsRequired();
            e.Property(x => x.RolId).IsRequired();
            e.Property(x => x.Estatus).IsRequired();
            e.Ignore(x => x.NombreCompleto);
        });

        modelBuilder.Entity<Campania>(e =>
        {
            e.ToTable("tbCatCampania");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasMaxLength(256).IsRequired();
            e.Property(x => x.AnioCreacion).IsRequired();
            e.Property(x => x.ClienteId).IsRequired();
            e.Property(x => x.PaisId).IsRequired();
            e.Ignore(x => x.Codigo);
            e.Ignore(x => x.Descripcion);
            e.Ignore(x => x.FechaInicio);
            e.Ignore(x => x.FechaFin);
            e.Ignore(x => x.FechaCreacionUtc);
        });
    }
}
