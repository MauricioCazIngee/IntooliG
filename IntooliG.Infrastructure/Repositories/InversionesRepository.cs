using System.Data;
using System.Globalization;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Inversiones;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class InversionesRepository : IInversionesRepository
{
    private const string SpMain = "SPWEBInversionesCorporativoRegionMonthlyUNPIVOT";
    private const string SpBarras = "SPWEBInversionesCorporativoRegionPorCategoriaMarcaMonth";
    private const string SpPieCategoria = "SPWEBInversionesCorporativoRegionPorCategoriaMonthPie";
    private const string SpPieMarca = "SPWEBInversionesCorporativoRegionPorMarcaMonthPie";
    private const string SpPieMedio = "SPWEBInversionesCorporativoRegionPorMedioMonthPie";

    private readonly AppDbContext _db;

    public InversionesRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<InversionesFiltrosDto> GetFiltrosAsync(
        int clienteId,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        var upr = await _db.UserPaisRegiones
            .AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);

        IReadOnlyList<PaisDto> paises;
        IReadOnlyList<RegionDto> regiones;
        if (upr.Count == 0)
        {
            paises = Array.Empty<PaisDto>();
            regiones = Array.Empty<RegionDto>();
        }
        else
        {
            var paisIds = upr.Select(x => x.PaisId).Distinct().ToHashSet();
            paises = await _db.CatPaises.AsNoTracking()
                .Where(p => paisIds.Contains(p.Id) && p.Activo)
                .OrderBy(p => p.NombrePais)
                .Select(p => new PaisDto(p.Id, p.NombrePais))
                .ToListAsync(cancellationToken);

            var regionRows = await _db.CatRegiones.AsNoTracking()
                .Where(r => r.ClienteId == clienteId && r.Activo && paisIds.Contains(r.PaisId))
                .ToListAsync(cancellationToken);

            var filtradas = regionRows
                .Where(r => upr.Any(u => u.PaisId == r.PaisId && (u.RegionId == null || u.RegionId == r.Id)))
                .OrderBy(r => r.PaisId)
                .ThenBy(r => r.NombreRegion)
                .Select(r => new RegionDto(r.Id, r.PaisId, r.NombreRegion))
                .ToList();
            regiones = filtradas;
        }

        var sectores = await _db.CatSectors.AsNoTracking()
            .Where(s => s.ClienteId == clienteId && s.Activo)
            .OrderBy(s => s.NombreSector)
            .Select(s => new SectorDto(s.Id, s.NombreSector))
            .ToListAsync(cancellationToken);

        var bus = await _db.CatBUs.AsNoTracking()
            .Where(b => b.ClienteId == clienteId && b.Activo)
            .OrderBy(b => b.SectorId)
            .ThenBy(b => b.NombreBU)
            .Select(b => new BUDto(b.Id, b.SectorId, b.NombreBU))
            .ToListAsync(cancellationToken);

        var categorias = await _db.CatCategorias.AsNoTracking()
            .Where(c => c.ClienteId == clienteId && c.Activo)
            .OrderBy(c => c.NombreCategoria)
            .Select(c => new CategoriaDto(c.Id, c.NombreCategoria))
            .ToListAsync(cancellationToken);

        var marcas = await _db.CatMarcas.AsNoTracking()
            .Where(m => m.ClienteId == clienteId && m.Activo)
            .OrderBy(m => m.NombreMarca)
            .Select(m => new MarcaFiltroDto(m.Id, m.NombreMarca))
            .ToListAsync(cancellationToken);

        return new InversionesFiltrosDto
        {
            Paises = paises,
            Regiones = regiones,
            Sectores = sectores,
            BUs = bus,
            Categorias = categorias,
            Marcas = marcas,
            Periodos = new[]
            {
                new PeriodTypeDto(1, "MONTHLY", "MONTHLY"),
                new PeriodTypeDto(2, "WEEKLY", "WEEKLY")
            },
            Tarifas = new[]
            {
                new TarifaDto(1, "FULL", "FULL"),
                new TarifaDto(2, "AVERAGE", "AVERAGE")
            },
            Vistas = new[]
            {
                new VistaDto(1, "MM", "MM"),
                new VistaDto(2, "K", "K"),
                new VistaDto(3, "NORMAL", "NORMAL")
            },
            ExchangeRates = new[]
            {
                new ExchangeRateDto(1, "LOCAL", "LOCAL"),
                new ExchangeRateDto(2, "USD", "USD")
            }
        };
    }

    public async Task<IReadOnlyList<CiudadFiltroDto>> GetCiudadesPorRegionAsync(
        int clienteId,
        int usuarioId,
        int regionId,
        CancellationToken cancellationToken = default)
    {
        var region = await _db.CatRegiones.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == regionId && r.ClienteId == clienteId, cancellationToken);
        if (region is null)
            return Array.Empty<CiudadFiltroDto>();

        if (!await PuedeVerRegionAsync(usuarioId, region, cancellationToken))
            return Array.Empty<CiudadFiltroDto>();

        return await (
                from a in _db.AgrupacionRegiones.AsNoTracking()
                join c in _db.CatCiudades.AsNoTracking() on a.CiudadId equals c.Id
                where a.RegionId == regionId && c.Activo
                orderby c.NombreCiudad
                select new CiudadFiltroDto(c.Id, regionId, c.NombreCiudad))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InversionesResultDto>> GetDatosAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default)
    {
        await ValidarPeticionAsync(clienteId, usuarioId, query, cancellationToken);
        var s = await ResolveParametrosSqlAsync(clienteId, query, cancellationToken);
        return await ExecuteStoredProcedureAsync(
            SpMain,
            cmd => AddParametersMain(cmd, s),
            MapInversionesFila,
            cancellationToken);
    }

    public async Task<InversionesTablaDinamicaDto> GetDatosBarrasAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default)
    {
        await ValidarPeticionAsync(clienteId, usuarioId, query, cancellationToken);
        var s = await ResolveParametrosSqlAsync(clienteId, query, cancellationToken);
        return await ReadTablaDinamicaAsync(
            SpBarras,
            cmd => AddParametersBarras(cmd, s),
            cancellationToken);
    }

    public async Task<InversionesTablaDinamicaDto> GetDatosPieCategoriaAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default)
    {
        await ValidarPeticionAsync(clienteId, usuarioId, query, cancellationToken);
        var s = await ResolveParametrosSqlAsync(clienteId, query, cancellationToken);
        return await ReadTablaDinamicaAsync(
            SpPieCategoria,
            cmd => AddParametersPieCategoriaOMedio(cmd, s),
            cancellationToken);
    }

    public async Task<InversionesTablaDinamicaDto> GetDatosPieMarcaAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default)
    {
        await ValidarPeticionAsync(clienteId, usuarioId, query, cancellationToken);
        var s = await ResolveParametrosSqlAsync(clienteId, query, cancellationToken);
        return await ReadTablaDinamicaAsync(
            SpPieMarca,
            cmd => AddParametersPieMarca(cmd, s),
            cancellationToken);
    }

    public async Task<InversionesTablaDinamicaDto> GetDatosPieMedioAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default)
    {
        await ValidarPeticionAsync(clienteId, usuarioId, query, cancellationToken);
        var s = await ResolveParametrosSqlAsync(clienteId, query, cancellationToken);
        return await ReadTablaDinamicaAsync(
            SpPieMedio,
            cmd => AddParametersPieCategoriaOMedio(cmd, s),
            cancellationToken);
    }

    /// <summary>Valores alineados a las firmas varchar de los SP (nombres de catálogo, fechas en texto).</summary>
    private sealed class InversionesParametrosSql
    {
        public int PeriodType { get; init; }
        public string StartDate { get; init; } = string.Empty;
        public string FinalDate { get; init; } = string.Empty;
        public string Categoria { get; init; } = string.Empty;
        public int Rate { get; init; }
        public int View { get; init; }
        public string Region { get; init; } = string.Empty;
        public string Ciudad { get; init; } = string.Empty;
        public string Marca { get; init; } = string.Empty;
        public int ExchangeRate { get; init; }
    }

    private async Task<InversionesParametrosSql> ResolveParametrosSqlAsync(
        int clienteId,
        InversionesQueryContext q,
        CancellationToken cancellationToken)
    {
        var start = q.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var final = q.FinalDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var categoria = string.Empty;
        if (q.CategoriaId is int cId && cId > 0)
        {
            categoria = await _db.CatCategorias.AsNoTracking()
                .Where(x => x.Id == cId && x.ClienteId == clienteId)
                .Select(x => x.NombreCategoria)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        var region = string.Empty;
        if (q.RegionId is int rId && rId > 0)
        {
            region = await _db.CatRegiones.AsNoTracking()
                .Where(x => x.Id == rId && x.ClienteId == clienteId)
                .Select(x => x.NombreRegion)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        var ciudad = string.Empty;
        if (q.CiudadId is int ciuId && ciuId > 0)
        {
            ciudad = await _db.CatCiudades.AsNoTracking()
                .Where(x => x.Id == ciuId)
                .Select(x => x.NombreCiudad)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        var marca = string.Empty;
        if (q.MarcaId is int mId && mId > 0)
        {
            marca = await _db.CatMarcas.AsNoTracking()
                .Where(x => x.Id == mId && x.ClienteId == clienteId)
                .Select(x => x.NombreMarca)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        return new InversionesParametrosSql
        {
            PeriodType = q.Periodo,
            StartDate = TruncVarchar(start, 100),
            FinalDate = TruncVarchar(final, 100),
            Categoria = TruncVarchar(categoria, 100),
            Rate = q.TipoTarifa,
            View = q.Vista,
            Region = TruncVarchar(region, 200),
            Ciudad = ciudad,
            Marca = TruncVarchar(marca, 200),
            ExchangeRate = q.ExchangeRate
        };
    }

    private static string TruncVarchar(string? value, int maxLen)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        var t = value.Trim();
        return t.Length <= maxLen ? t : t[..maxLen];
    }

    /// <summary>SPWEBInversionesCorporativoRegionMonthlyUNPIVOT</summary>
    private static void AddParametersMain(SqlCommand cmd, InversionesParametrosSql s)
    {
        cmd.Parameters.Add(new SqlParameter("@periodType", SqlDbType.Int) { Value = s.PeriodType });
        cmd.Parameters.Add(new SqlParameter("@startDate", SqlDbType.VarChar, 100) { Value = s.StartDate });
        cmd.Parameters.Add(new SqlParameter("@finalDate", SqlDbType.VarChar, 100) { Value = s.FinalDate });
        cmd.Parameters.Add(new SqlParameter("@categoria", SqlDbType.VarChar, 100) { Value = s.Categoria });
        cmd.Parameters.Add(new SqlParameter("@rate", SqlDbType.Int) { Value = s.Rate });
        cmd.Parameters.Add(new SqlParameter("@view", SqlDbType.Int) { Value = s.View });
        cmd.Parameters.Add(new SqlParameter("@region", SqlDbType.VarChar, 200) { Value = s.Region });
        cmd.Parameters.Add(new SqlParameter("@ciudad", SqlDbType.VarChar, -1) { Value = s.Ciudad });
        cmd.Parameters.Add(new SqlParameter("@exchangeRate", SqlDbType.Int) { Value = s.ExchangeRate });
    }

    /// <summary>SPWEBInversionesCorporativoRegionPorCategoriaMarcaMonth</summary>
    private static void AddParametersBarras(SqlCommand cmd, InversionesParametrosSql s)
    {
        const int orden = 0;
        cmd.Parameters.Add(new SqlParameter("@periodType", SqlDbType.Int) { Value = s.PeriodType });
        cmd.Parameters.Add(new SqlParameter("@finalDate", SqlDbType.VarChar, 100) { Value = s.FinalDate });
        cmd.Parameters.Add(new SqlParameter("@categoria", SqlDbType.VarChar, 100) { Value = s.Categoria });
        cmd.Parameters.Add(new SqlParameter("@rate", SqlDbType.Int) { Value = s.Rate });
        cmd.Parameters.Add(new SqlParameter("@view", SqlDbType.Int) { Value = s.View });
        cmd.Parameters.Add(new SqlParameter("@region", SqlDbType.VarChar, 200) { Value = s.Region });
        cmd.Parameters.Add(new SqlParameter("@ciudad", SqlDbType.VarChar, -1) { Value = s.Ciudad });
        cmd.Parameters.Add(new SqlParameter("@Marca", SqlDbType.VarChar, 200) { Value = s.Marca });
        cmd.Parameters.Add(new SqlParameter("@orden", SqlDbType.Int) { Value = orden });
        cmd.Parameters.Add(new SqlParameter("@exchangeRate", SqlDbType.Int) { Value = s.ExchangeRate });
    }

    /// <summary>SP pie categoría y SP pie medio (misma firma).</summary>
    private static void AddParametersPieCategoriaOMedio(SqlCommand cmd, InversionesParametrosSql s)
    {
        cmd.Parameters.Add(new SqlParameter("@periodType", SqlDbType.Int) { Value = s.PeriodType });
        cmd.Parameters.Add(new SqlParameter("@finalDate", SqlDbType.VarChar, 100) { Value = s.FinalDate });
        cmd.Parameters.Add(new SqlParameter("@categoria", SqlDbType.VarChar, 100) { Value = s.Categoria });
        cmd.Parameters.Add(new SqlParameter("@rate", SqlDbType.Int) { Value = s.Rate });
        cmd.Parameters.Add(new SqlParameter("@view", SqlDbType.Int) { Value = s.View });
        cmd.Parameters.Add(new SqlParameter("@region", SqlDbType.VarChar, 200) { Value = s.Region });
        cmd.Parameters.Add(new SqlParameter("@ciudad", SqlDbType.VarChar, -1) { Value = s.Ciudad });
        cmd.Parameters.Add(new SqlParameter("@Marca", SqlDbType.VarChar, 200) { Value = s.Marca });
        cmd.Parameters.Add(new SqlParameter("@exchangeRate", SqlDbType.Int) { Value = s.ExchangeRate });
    }

    /// <summary>SPWEBInversionesCorporativoRegionPorMarcaMonthPie</summary>
    private static void AddParametersPieMarca(SqlCommand cmd, InversionesParametrosSql s)
    {
        const int orden = 0;
        cmd.Parameters.Add(new SqlParameter("@periodType", SqlDbType.Int) { Value = s.PeriodType });
        cmd.Parameters.Add(new SqlParameter("@finalDate", SqlDbType.VarChar, 100) { Value = s.FinalDate });
        cmd.Parameters.Add(new SqlParameter("@categoria", SqlDbType.VarChar, 100) { Value = s.Categoria });
        cmd.Parameters.Add(new SqlParameter("@rate", SqlDbType.Int) { Value = s.Rate });
        cmd.Parameters.Add(new SqlParameter("@view", SqlDbType.Int) { Value = s.View });
        cmd.Parameters.Add(new SqlParameter("@region", SqlDbType.VarChar, 200) { Value = s.Region });
        cmd.Parameters.Add(new SqlParameter("@ciudad", SqlDbType.VarChar, -1) { Value = s.Ciudad });
        cmd.Parameters.Add(new SqlParameter("@Marca", SqlDbType.VarChar, 200) { Value = s.Marca });
        cmd.Parameters.Add(new SqlParameter("@orden", SqlDbType.Int) { Value = orden });
        cmd.Parameters.Add(new SqlParameter("@exchangeRate", SqlDbType.Int) { Value = s.ExchangeRate });
    }

    private async Task ValidarPeticionAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext q,
        CancellationToken cancellationToken)
    {
        if (q.CategoriaId is { } cId && cId > 0)
        {
            var okC = await _db.CatCategorias.AnyAsync(
                x => x.Id == cId && x.ClienteId == clienteId && x.Activo, cancellationToken);
            if (!okC)
                throw new InvalidOperationException("La categoría no es válida para su cliente.");
        }

        if (q.MarcaId is { } mId && mId > 0)
        {
            var okM = await _db.CatMarcas.AnyAsync(
                x => x.Id == mId && x.ClienteId == clienteId && x.Activo, cancellationToken);
            if (!okM)
                throw new InvalidOperationException("La marca no es válida para su cliente.");
        }

        if (q.SectorId is { } sId && sId > 0)
        {
            var okS = await _db.CatSectors.AnyAsync(
                x => x.Id == sId && x.ClienteId == clienteId && x.Activo, cancellationToken);
            if (!okS)
                throw new InvalidOperationException("El sector no es válido para su cliente.");
        }

        if (q.BuId is { } bId && bId > 0)
        {
            var okB = await _db.CatBUs.AnyAsync(
                x => x.Id == bId && x.ClienteId == clienteId && x.Activo, cancellationToken);
            if (!okB)
                throw new InvalidOperationException("La BU no es válida para su cliente.");
        }

        if (q.PaisId is { } pId)
        {
            if (!await PuedeVerPaisAsync(usuarioId, pId, cancellationToken))
                throw new InvalidOperationException("Sin permisos para el país seleccionado.");
        }

        if (q.RegionId is { } rId)
        {
            var reg = await _db.CatRegiones.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == rId && x.ClienteId == clienteId, cancellationToken);
            if (reg is null)
                throw new InvalidOperationException("Región no encontrada.");
            if (!await PuedeVerRegionAsync(usuarioId, reg, cancellationToken))
                throw new InvalidOperationException("Sin permisos para la región seleccionada.");
        }

        if (q.CiudadId is { } ciudId)
        {
            if (q.RegionId is not { } reqReg)
                throw new InvalidOperationException("Debe seleccionar región para filtrar por ciudad.");

            var link = await _db.AgrupacionRegiones
                .AsNoTracking()
                .AnyAsync(x => x.RegionId == reqReg && x.CiudadId == ciudId, cancellationToken);
            if (!link)
                throw new InvalidOperationException("La ciudad no pertenece a la región indicada.");
        }
    }

    private async Task<bool> PuedeVerPaisAsync(int usuarioId, int paisId, CancellationToken cancellationToken)
    {
        return await _db.UserPaisRegiones.AsNoTracking()
            .AnyAsync(x => x.UsuarioId == usuarioId && x.PaisId == paisId, cancellationToken);
    }

    private async Task<bool> PuedeVerRegionAsync(int usuarioId, CatRegion region, CancellationToken cancellationToken)
    {
        return await _db.UserPaisRegiones.AsNoTracking()
            .AnyAsync(
                u => u.UsuarioId == usuarioId
                     && u.PaisId == region.PaisId
                     && (u.RegionId == null || u.RegionId == region.Id),
                cancellationToken);
    }

    private async Task<IReadOnlyList<T>> ExecuteStoredProcedureAsync<T>(
        string spName,
        Action<SqlCommand> parameterBuilder,
        Func<SqlDataReader, T> mapRow,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("InversionesRepository requiere conexión SQL Server (SqlConnection).");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(spName, sqlConnection) { CommandType = CommandType.StoredProcedure };
        parameterBuilder(command);

        var list = new List<T>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            list.Add(mapRow(reader));
        return list;
    }

    private async Task<InversionesTablaDinamicaDto> ReadTablaDinamicaAsync(
        string spName,
        Action<SqlCommand> parameterBuilder,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("InversionesRepository requiere conexión SQL Server (SqlConnection).");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(spName, sqlConnection) { CommandType = CommandType.StoredProcedure };
        parameterBuilder(command);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var colCount = reader.FieldCount;
        var columnas = new string[colCount];
        for (var i = 0; i < colCount; i++)
            columnas[i] = reader.GetName(i);

        var filas = new List<IReadOnlyDictionary<string, object?>>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var d = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < colCount; i++)
            {
                d[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            filas.Add(d);
        }

        return new InversionesTablaDinamicaDto
        {
            Columnas = columnas,
            Filas = filas
        };
    }

    private static InversionesResultDto MapInversionesFila(SqlDataReader r)
    {
        var marca = GetFirstMatchingString(
            r,
            "Marca", "FcNombreMarca", "NombreMarca", "Categoria", "Descripcion", "Grupo", "LabelMarca");
        var periodo = GetFirstMatchingString(
            r,
            "MesAnio", "MesYAnio", "Periodo", "MesAño", "MesAno", "Label", "AñoMes", "Columna");
        var total = GetFirstMatchingDecimal(r, "Total", "Valor", "Importe", "FnTotal", "Monto", "Dato", "Suma");
        if (string.IsNullOrWhiteSpace(periodo) && r.FieldCount >= 2)
        {
            for (var i = 0; i < r.FieldCount; i++)
            {
                if (r.GetName(i).Contains("Mes", StringComparison.OrdinalIgnoreCase)
                    || r.GetName(i).Contains("Periodo", StringComparison.OrdinalIgnoreCase)
                    || r.GetName(i).Contains("Anio", StringComparison.OrdinalIgnoreCase)
                    || r.GetName(i).Contains("Año", StringComparison.OrdinalIgnoreCase))
                {
                    if (!r.IsDBNull(i))
                    {
                        periodo = Convert.ToString(r.GetValue(i), CultureInfo.InvariantCulture) ?? string.Empty;
                        break;
                    }
                }
            }
        }

        return new InversionesResultDto
        {
            Marca = marca,
            MesAnio = periodo,
            Total = total
        };
    }

    private static string GetFirstMatchingString(SqlDataReader r, params string[] names)
    {
        foreach (var n in names)
        {
            var v = TryGetString(r, n);
            if (v is not null)
                return v;
        }

        for (var i = 0; i < r.FieldCount; i++)
        {
            if (r.IsDBNull(i))
                continue;
            var t = r.GetFieldType(i);
            if (t == typeof(string) || t == typeof(Guid))
            {
                var s = r.GetValue(i)?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(s))
                    return s;
            }
        }

        return string.Empty;
    }

    private static string? TryGetString(SqlDataReader r, string column)
    {
        try
        {
            var o = r[column];
            return o == null || o is DBNull ? null : Convert.ToString(o, CultureInfo.InvariantCulture);
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    private static decimal? GetFirstMatchingDecimal(SqlDataReader r, params string[] names)
    {
        foreach (var n in names)
        {
            try
            {
                var o = r[n];
                if (o is DBNull)
                    return null;
                if (o is null)
                    return null;
                return Convert.ToDecimal(o, CultureInfo.InvariantCulture);
            }
            catch (IndexOutOfRangeException)
            {
                // siguiente nombre
            }
        }

        for (var i = 0; i < r.FieldCount; i++)
        {
            if (r.IsDBNull(i))
                continue;
            var t = r.GetFieldType(i);
            if (t == typeof(decimal) || t == typeof(double) || t == typeof(float) || t == typeof(int) || t == typeof(long))
            {
                return Convert.ToDecimal(r.GetValue(i), CultureInfo.InvariantCulture);
            }
        }

        return null;
    }
}
