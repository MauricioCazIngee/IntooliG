using IntooliG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<CatRolPbi> CatRolesPbi => Set<CatRolPbi>();
    public DbSet<Campania> Campanias => Set<Campania>();

    public DbSet<CatCliente> CatClientes => Set<CatCliente>();
    public DbSet<CatSector> CatSectors => Set<CatSector>();
    public DbSet<CatBU> CatBUs => Set<CatBU>();
    public DbSet<CatCategoria> CatCategorias => Set<CatCategoria>();
    public DbSet<CatMarca> CatMarcas => Set<CatMarca>();
    public DbSet<CatProducto> CatProductos => Set<CatProducto>();

    public DbSet<CatRubroGeneral> CatRubroGenerales => Set<CatRubroGeneral>();
    public DbSet<CatRubro> CatRubros => Set<CatRubro>();
    public DbSet<CatConcepto> CatConceptos => Set<CatConcepto>();
    public DbSet<CatRanking> CatRankings => Set<CatRanking>();
    public DbSet<CatValor> CatValores => Set<CatValor>();

    public DbSet<CatPais> CatPaises => Set<CatPais>();
    public DbSet<CatCodigoMapa> CatCodigoMapas => Set<CatCodigoMapa>();
    public DbSet<CatEstado> CatEstados => Set<CatEstado>();
    public DbSet<CatRegion> CatRegiones => Set<CatRegion>();
    public DbSet<AgrupacionRegion> AgrupacionRegiones => Set<AgrupacionRegion>();
    public DbSet<CatCiudad> CatCiudades => Set<CatCiudad>();
    public DbSet<CatPoblacion> CatPoblaciones => Set<CatPoblacion>();

    public DbSet<CatMedio> CatMedios => Set<CatMedio>();
    public DbSet<MedioCliente> MedioClientes => Set<MedioCliente>();
    public DbSet<CatFuente> CatFuentes => Set<CatFuente>();
    public DbSet<CatMarcaProductoFuente> CatMarcaProductoFuentes => Set<CatMarcaProductoFuente>();
    public DbSet<CatVersionFuente> CatVersionFuentes => Set<CatVersionFuente>();
    public DbSet<CatVersionTV> CatVersionTVs => Set<CatVersionTV>();
    public DbSet<CatTipoCambio> CatTiposCambio => Set<CatTipoCambio>();
    public DbSet<CatDaypart> CatDayparts => Set<CatDaypart>();
    public DbSet<UserPaisRegion> UserPaisRegiones => Set<UserPaisRegion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("tbUser");
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

        modelBuilder.Entity<CatRolPbi>(e =>
        {
            e.ToTable("tbCatRolesPBI");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreRol).HasMaxLength(100).IsRequired();
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

        modelBuilder.Entity<CatCliente>(e =>
        {
            e.ToTable("tbCatCliente");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasColumnName("FcNombreCliente").HasMaxLength(256).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbEstatus");
            e.Property(x => x.AvisoPrivacidad).HasColumnName("FcAvisoPrivacidad");
        });

        modelBuilder.Entity<CatSector>(e =>
        {
            e.ToTable("tbCatSector");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreSector).HasColumnName("FcNombreSector").HasMaxLength(256).IsRequired();
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.Activo).HasColumnName("FbEstatus");
        });

        modelBuilder.Entity<CatBU>(e =>
        {
            e.ToTable("tbCatBU");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreBU).HasColumnName("FcNombreBU").HasMaxLength(256).IsRequired();
            e.Property(x => x.SectorId).HasColumnName("FiSectorid");
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.Activo).HasColumnName("FbEstatus");
        });

        modelBuilder.Entity<CatCategoria>(e =>
        {
            e.ToTable("tbCatCategoria");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreCategoria).HasColumnName("FcNombreCategoria").HasMaxLength(256).IsRequired();
            e.Property(x => x.NombreCorto).HasColumnName("FcNombreCorto").HasMaxLength(256);
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.Activo).HasColumnName("FbEstatus");
        });

        modelBuilder.Entity<CatMarca>(e =>
        {
            e.ToTable("tbCatMarca");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreMarca).HasColumnName("FcNombreMarcaCorto").HasMaxLength(256).IsRequired();
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.Activo).HasColumnName("FbEstatus");
            e.Property(x => x.Logo).HasColumnName("FbLogomarca").HasColumnType("varbinary(max)");
            e.Property(x => x.Color).HasColumnName("FcColor").HasMaxLength(64);
            e.Property(x => x.TipoMarca).HasColumnName("FcTipoMarca").HasMaxLength(8);
        });

        modelBuilder.Entity<CatProducto>(e =>
        {
            e.ToTable("tbCatProducto");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasColumnName("FcNombreProductoGenerico").HasMaxLength(256).IsRequired();
            e.Property(x => x.MarcaId).HasColumnName("FiMarcaid");
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.HasOne(x => x.Marca).WithMany().HasForeignKey(x => x.MarcaId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatRubroGeneral>(e =>
        {
            e.ToTable("tbCatRubroGeneral");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiRubroId");
            e.Property(x => x.NombreRubro).HasColumnName("FcRubroNombre").HasMaxLength(256).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbEstatus");
        });

        modelBuilder.Entity<CatRubro>(e =>
        {
            e.ToTable("tbCatRubro");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiCatRubroid");
            e.Property(x => x.RubroGeneralId).HasColumnName("FiRubroid");
            e.Property(x => x.CategoriaId).HasColumnName("FiCategoriaid");
            e.Property(x => x.ValorRubro).HasColumnName("FiValorRubro").HasColumnType("decimal(18,4)");
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.HasOne(x => x.RubroGeneral).WithMany().HasForeignKey(x => x.RubroGeneralId);
            e.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.CategoriaId);
        });

        modelBuilder.Entity<CatConcepto>(e =>
        {
            e.ToTable("tbCatConcepto");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiConceptoid");
            e.Property(x => x.RubroGeneralId).HasColumnName("FiRubroid");
            e.Property(x => x.CategoriaId).HasColumnName("FiCategoriaid");
            e.Property(x => x.NombreConcepto).HasColumnName("FcNombreConcepto").HasMaxLength(512).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.Property(x => x.Top).HasColumnName("FbTop");
            e.HasOne(x => x.RubroGeneral).WithMany().HasForeignKey(x => x.RubroGeneralId);
            e.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.CategoriaId);
        });

        modelBuilder.Entity<CatRanking>(e =>
        {
            e.ToTable("tbRanking");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiRankingid");
            e.Property(x => x.ConceptoId).HasColumnName("FiConceptoid");
            e.Property(x => x.RubroGeneralId).HasColumnName("FiRubroid");
            e.Property(x => x.CategoriaId).HasColumnName("FiCategoriaid");
            e.Property(x => x.Posicion).HasColumnName("FiPosicion");
            e.HasOne(x => x.Concepto).WithMany().HasForeignKey(x => x.ConceptoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CatValor>(e =>
        {
            e.ToTable("tbCatValor");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiValorid");
            e.Property(x => x.RubroGeneralId).HasColumnName("FiRubroid");
            e.Property(x => x.CategoriaId).HasColumnName("FiCategoriaid");
            e.Property(x => x.Posicion).HasColumnName("FiPosicion");
            e.Property(x => x.Valor).HasColumnName("FdValor").HasColumnType("decimal(18,4)");
        });

        modelBuilder.Entity<CatPais>(e =>
        {
            e.ToTable("tbCatPais");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiPaisid");
            e.Property(x => x.NombrePais).HasColumnName("FcNombrePais").HasMaxLength(256).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbActivo");
        });

        modelBuilder.Entity<CatCodigoMapa>(e =>
        {
            e.ToTable("tbCatCodigoMapa");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiCodigoMapaid");
            e.Property(x => x.NombreCodigoMapa).HasColumnName("FcNombreCodigoMapa").HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<CatEstado>(e =>
        {
            e.ToTable("tbCatEstado");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiEstadoid");
            e.Property(x => x.PaisId).HasColumnName("FiPaisid");
            e.Property(x => x.NombreEstado).HasColumnName("FcNombreEstado").HasMaxLength(256).IsRequired();
            e.Property(x => x.CodigoMapaId).HasColumnName("FiCodigoMapaid");
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.HasOne(x => x.Pais).WithMany().HasForeignKey(x => x.PaisId);
            e.HasOne(x => x.CodigoMapa).WithMany().HasForeignKey(x => x.CodigoMapaId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatRegion>(e =>
        {
            e.ToTable("tbCatRegion");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiRegionid");
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.NombreRegion).HasColumnName("FcNombreRegion").HasMaxLength(256).IsRequired();
            e.Property(x => x.EsNacional).HasColumnName("FbEsNacional");
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.Property(x => x.PaisId).HasColumnName("FiPaisid");
            e.HasOne(x => x.Pais).WithMany().HasForeignKey(x => x.PaisId);
        });

        modelBuilder.Entity<AgrupacionRegion>(e =>
        {
            e.ToTable("tbAgrupacionRegion");
            e.HasKey(x => new { x.RegionId, x.CiudadId });
            e.Property(x => x.RegionId).HasColumnName("FiRegionid");
            e.Property(x => x.CiudadId).HasColumnName("FiCiudadid");
            e.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Ciudad).WithMany().HasForeignKey(x => x.CiudadId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatCiudad>(e =>
        {
            e.ToTable("tbCatCiudad");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiCiudadid");
            e.Property(x => x.EstadoId).HasColumnName("FiEstadoid");
            e.Property(x => x.NombreCiudad).HasColumnName("FcNombreCiudad").HasMaxLength(256).IsRequired();
            e.Property(x => x.NombreCorto).HasColumnName("FcNombreCorto").HasMaxLength(256);
            e.Property(x => x.CiudadPrincipal).HasColumnName("FbCiudadPrincipal");
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.Property(x => x.Poblacion).HasColumnName("FnPoblacion");
            e.HasOne(x => x.Estado).WithMany().HasForeignKey(x => x.EstadoId);
        });

        modelBuilder.Entity<CatPoblacion>(e =>
        {
            e.ToTable("tbPoblacion");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiPoblacionid");
            e.Property(x => x.Anio).HasColumnName("FiAnio");
            e.Property(x => x.CiudadId).HasColumnName("FiCiudadid");
            e.Property(x => x.Cantidad).HasColumnName("FiPoblacion");
            e.HasOne(x => x.Ciudad).WithMany().HasForeignKey(x => x.CiudadId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CatMedio>(e =>
        {
            e.ToTable("tbCatMedio");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiMedioid");
            e.Property(x => x.NombreMedio).HasColumnName("FcNombreMedio").HasMaxLength(512).IsRequired();
            e.Property(x => x.NombreMedioGenerico).HasColumnName("FcNombreMedioGenerico").HasMaxLength(512).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbActivo");
        });

        modelBuilder.Entity<MedioCliente>(e =>
        {
            e.ToTable("tbMedioCliente");
            e.HasKey(x => new { x.MedioId, x.ClienteId });
            e.Property(x => x.MedioId).HasColumnName("FiMedioid");
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.EsNacional).HasColumnName("FbEsNacional");
            e.HasOne(x => x.Medio).WithMany().HasForeignKey(x => x.MedioId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatFuente>(e =>
        {
            e.ToTable("tbCatFuente");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiFuenteid");
            e.Property(x => x.NombreFuente).HasColumnName("FcNombreFuente").HasMaxLength(512).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.Property(x => x.PaisId).HasColumnName("FiPaisid");
            e.HasOne(x => x.Pais).WithMany().HasForeignKey(x => x.PaisId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatMarcaProductoFuente>(e =>
        {
            e.ToTable("tbCatMarcaProductoFuente");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiMarcaProductoFuenteid").ValueGeneratedOnAdd();
            e.Property(x => x.NombreMarcaFuente).HasColumnName("FcNombreMarcaFuente").HasMaxLength(512).IsRequired();
            e.Property(x => x.NombreProductoFuente).HasColumnName("FcNombreProductoFuente").HasMaxLength(512).IsRequired();
            e.Property(x => x.MarcaId).HasColumnName("FiMarcaid");
            e.Property(x => x.ProductoId).HasColumnName("FiProductoid");
            e.Property(x => x.FuenteId).HasColumnName("FiFuenteid");
            e.HasOne(x => x.Fuente).WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Marca).WithMany().HasForeignKey(x => x.MarcaId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatVersionTV>(e =>
        {
            e.ToTable("tbCatVersionTV");
            e.Property(x => x.Nombre).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<CatVersionFuente>(e =>
        {
            e.ToTable("tbCatVersionFuente");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiVersionFuenteid");
            e.Property(x => x.FuenteId).HasColumnName("FiFuenteid");
            e.Property(x => x.NombreVersionFuente).HasColumnName("FcNombreVersionFuente").HasMaxLength(512).IsRequired();
            e.Property(x => x.Activo).HasColumnName("FbActivo");
            e.Property(x => x.ProductoId).HasColumnName("FiProductoid");
            e.Property(x => x.VersionTVId).HasColumnName("FiVersionTV");
            e.Property(x => x.BUId).HasColumnName("FiBUid");
            e.Property(x => x.CategoriaId).HasColumnName("FiCategoriaid");
            e.HasOne(x => x.Fuente).WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.VersionTV).WithMany().HasForeignKey(x => x.VersionTVId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.BU).WithMany().HasForeignKey(x => x.BUId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CatTipoCambio>(e =>
        {
            e.ToTable("tbTipoCambio");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiTipoCambioid");
            e.Property(x => x.PaisId).HasColumnName("FiPaisid");
            e.Property(x => x.Anio).HasColumnName("FnAnio");
            e.Property(x => x.Mes).HasColumnName("FnMes");
            e.Property(x => x.TipoCambio).HasColumnName("FnTipoCambio").HasColumnType("decimal(18,4)");
            e.HasOne(x => x.Pais).WithMany().HasForeignKey(x => x.PaisId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => new { x.PaisId, x.Anio, x.Mes }).IsUnique();
        });

        modelBuilder.Entity<CatDaypart>(e =>
        {
            e.ToTable("tbCatDaypart");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("FiDaypartid");
            e.Property(x => x.ClienteId).HasColumnName("FiClienteid");
            e.Property(x => x.PaisId).HasColumnName("FiPaisid");
            e.Property(x => x.Nombre).HasColumnName("FcNombreDaypart").HasMaxLength(512).IsRequired();
            e.Property(x => x.HoraInicio).HasColumnName("FnHoraInicio");
            e.Property(x => x.HoraFin).HasColumnName("FnHoraFin");
            e.Property(x => x.MedioId).HasColumnName("FiMedioid");
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Pais).WithMany().HasForeignKey(x => x.PaisId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Medio).WithMany().HasForeignKey(x => x.MedioId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => new { x.ClienteId, x.PaisId, x.MedioId, x.Nombre }).IsUnique();
        });

        modelBuilder.Entity<UserPaisRegion>(e =>
        {
            e.ToTable("tbUserPaisRegion");
        });
    }
}
