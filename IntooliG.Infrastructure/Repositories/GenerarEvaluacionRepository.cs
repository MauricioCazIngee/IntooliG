using System.Data;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Radar.Dtos;
using IntooliG.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class GenerarEvaluacionRepository : IGenerarEvaluacionRepository
{
    private readonly AppDbContext _db;

    public GenerarEvaluacionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CampaniaEvaluacionDto>> GetCampaniasEvaluacionAsync(
        int? anio = null,
        int? categoriaId = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var offset = (page - 1) * pageSize;

        const string sql = """
            SELECT
                c.FiCampaniaid AS CampaniaId,
                c.FcNombreCampania AS Campania,
                ISNULL(m.FcNombreMarcaCorto, N'') AS Marca,
                ISNULL(cat.FcNombreCategoria, N'') AS Categoria,
                ISNULL(bu.FcNombreBU, N'') AS BU,
                ISNULL(sec.FcNombreSector, N'') AS Sector,
                c.FiAnioCreacion AS Anio,
                c.FbActivo AS Activo
            FROM dbo.tbCatCampania c
            INNER JOIN dbo.tbCatMarca m ON m.FiMarcaid = c.FiMarcaid
            LEFT JOIN (
                SELECT v.*, ROW_NUMBER() OVER (PARTITION BY v.FiCampaniaid, v.FiMarcaid ORDER BY v.FiMarcaProductoVersionFuenteid) AS rn
                FROM dbo.tbCatMarcaProductoVersionFuente v
            ) v ON v.FiCampaniaid = c.FiCampaniaid AND v.FiMarcaid = c.FiMarcaid AND v.rn = 1
            LEFT JOIN dbo.tbCatCategoria cat ON cat.FiCategoriaid = v.FiCategoriaid
            LEFT JOIN dbo.tbCatBU bu ON bu.FiBUid = v.FiBUid
            LEFT JOIN dbo.tbCatSector sec ON sec.FiSectorid = bu.FiSectorid AND sec.FiClienteid = bu.FiClienteid
            WHERE (@anio IS NULL OR c.FiAnioCreacion = @anio)
              AND (@activo IS NULL OR c.FbActivo = @activo)
              AND (@categoriaId IS NULL OR EXISTS (
                    SELECT 1
                    FROM dbo.tbCatMarcaProductoVersionFuente x
                    WHERE x.FiCampaniaid = c.FiCampaniaid
                      AND x.FiMarcaid = c.FiMarcaid
                      AND x.FiCategoriaid = @categoriaId))
              AND (@search IS NULL OR @search = N'' OR c.FcNombreCampania LIKE N'%' + @search + N'%'
                   OR m.FcNombreMarcaCorto LIKE N'%' + @search + N'%')
            ORDER BY c.FcNombreCampania
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
            """;

        return await ExecuteQueryAsync(sql, MapCampaniaEvaluacion,
            p =>
            {
                p.Add(new SqlParameter("@anio", SqlDbType.Int) { Value = (object?)anio ?? DBNull.Value });
                p.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = (object?)activo ?? DBNull.Value });
                p.Add(new SqlParameter("@categoriaId", SqlDbType.Int) { Value = (object?)categoriaId ?? DBNull.Value });
                p.Add(new SqlParameter("@search", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim() });
                p.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
                p.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
            },
            CancellationToken.None);
    }

    public async Task<int> GetCampaniasEvaluacionCountAsync(
        int? anio = null,
        int? categoriaId = null,
        bool? activo = null,
        string? search = null)
    {
        const string sql = """
            SELECT COUNT_BIG(1)
            FROM dbo.tbCatCampania c
            INNER JOIN dbo.tbCatMarca m ON m.FiMarcaid = c.FiMarcaid
            WHERE (@anio IS NULL OR c.FiAnioCreacion = @anio)
              AND (@activo IS NULL OR c.FbActivo = @activo)
              AND (@categoriaId IS NULL OR EXISTS (
                    SELECT 1
                    FROM dbo.tbCatMarcaProductoVersionFuente x
                    WHERE x.FiCampaniaid = c.FiCampaniaid
                      AND x.FiMarcaid = c.FiMarcaid
                      AND x.FiCategoriaid = @categoriaId))
              AND (@search IS NULL OR @search = N'' OR c.FcNombreCampania LIKE N'%' + @search + N'%'
                   OR m.FcNombreMarcaCorto LIKE N'%' + @search + N'%')
            """;

        return await ExecuteScalarIntAsync(sql,
            p =>
            {
                p.Add(new SqlParameter("@anio", SqlDbType.Int) { Value = (object?)anio ?? DBNull.Value });
                p.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = (object?)activo ?? DBNull.Value });
                p.Add(new SqlParameter("@categoriaId", SqlDbType.Int) { Value = (object?)categoriaId ?? DBNull.Value });
                p.Add(new SqlParameter("@search", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim() });
            },
            CancellationToken.None);
    }

    public async Task<List<AdministracionCampaniaDto>> GetAdministracionCampaniasAsync(
        int? anio = null,
        int? sectorId = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var offset = (page - 1) * pageSize;

        const string sql = """
            SELECT
                ad.FiAdministracionCampaniaid AS AdministracionId,
                ISNULL(sec.FcNombreSector, N'') AS Sector,
                ISNULL(bu.FcNombreBU, N'') AS UnidadNegocio,
                ISNULL(prod.FcNombreProductoGenerico, N'') AS Producto,
                ISNULL(m.FcNombreMarcaCorto, N'') AS Marca,
                ISNULL(vehNomb.FcNombreVehiculo, N'') AS Vehiculo,
                camp.FcNombreCampania AS Campania,
                ad.FiAnio AS Anio,
                ISNULL(sema.Semanas, N'') AS Semanas,
                ad.FbEstatus AS Activo
            FROM dbo.tbAdministracionCampania ad
            INNER JOIN dbo.tbCatCampania camp ON camp.FiCampaniaid = ad.FiCampaniaid
            INNER JOIN dbo.tbCatMarca m ON m.FiMarcaid = camp.FiMarcaid
            LEFT JOIN (
                SELECT v.*, ROW_NUMBER() OVER (PARTITION BY v.FiCampaniaid, v.FiMarcaid ORDER BY v.FiMarcaProductoVersionFuenteid) AS rn
                FROM dbo.tbCatMarcaProductoVersionFuente v
            ) v ON v.FiCampaniaid = camp.FiCampaniaid AND v.FiMarcaid = camp.FiMarcaid AND v.rn = 1
            LEFT JOIN dbo.tbCatBU bu ON bu.FiBUid = v.FiBUid
            LEFT JOIN dbo.tbCatSector sec ON sec.FiSectorid = bu.FiSectorid AND sec.FiClienteid = bu.FiClienteid
            LEFT JOIN dbo.tbCatProducto prod ON prod.FiProductoid = v.FiProductoid
            OUTER APPLY (
                SELECT TOP (1) veh.FcNombreVehiculo
                FROM dbo.tbAdministracionCampaniaMedio acm
                INNER JOIN dbo.tbCatVehiculo veh ON veh.FiMedioid = acm.FiMedioid AND veh.FiPaisid = camp.FiPaisid
                WHERE acm.FiAdministracionCampaniaid = ad.FiAdministracionCampaniaid
                ORDER BY veh.FiVehiculoid
            ) AS vehNomb
            OUTER APPLY (
                SELECT STRING_AGG(CAST(sem.FiSemana AS nvarchar(20)), N', ') WITHIN GROUP (ORDER BY sem.FiSemana) AS Semanas
                FROM dbo.tbAdministracionCampaniaSemanas acs
                INNER JOIN dbo.tbCatSemanas sem ON sem.FiSemanaid = acs.FiSemanaid
                WHERE acs.FiAdministracionCampaniaid = ad.FiAdministracionCampaniaid
            ) AS sema
            WHERE (@anio IS NULL OR ad.FiAnio = @anio)
              AND (@sectorId IS NULL OR sec.FiSectorid = @sectorId)
              AND (@search IS NULL OR @search = N'' OR camp.FcNombreCampania LIKE N'%' + @search + N'%'
                   OR m.FcNombreMarcaCorto LIKE N'%' + @search + N'%'
                   OR sec.FcNombreSector LIKE N'%' + @search + N'%'
                   OR bu.FcNombreBU LIKE N'%' + @search + N'%')
            ORDER BY camp.FcNombreCampania, ad.FiAnio
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
            """;

        return await ExecuteQueryAsync(sql, MapAdministracionCampania,
            p =>
            {
                p.Add(new SqlParameter("@anio", SqlDbType.Int) { Value = (object?)anio ?? DBNull.Value });
                p.Add(new SqlParameter("@sectorId", SqlDbType.Int) { Value = (object?)sectorId ?? DBNull.Value });
                p.Add(new SqlParameter("@search", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim() });
                p.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
                p.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
            },
            CancellationToken.None);
    }

    public async Task<int> GetAdministracionCampaniasCountAsync(
        int? anio = null,
        int? sectorId = null,
        string? search = null)
    {
        const string sql = """
            SELECT COUNT_BIG(1)
            FROM dbo.tbAdministracionCampania ad
            INNER JOIN dbo.tbCatCampania camp ON camp.FiCampaniaid = ad.FiCampaniaid
            INNER JOIN dbo.tbCatMarca m ON m.FiMarcaid = camp.FiMarcaid
            LEFT JOIN (
                SELECT v.*, ROW_NUMBER() OVER (PARTITION BY v.FiCampaniaid, v.FiMarcaid ORDER BY v.FiMarcaProductoVersionFuenteid) AS rn
                FROM dbo.tbCatMarcaProductoVersionFuente v
            ) v ON v.FiCampaniaid = camp.FiCampaniaid AND v.FiMarcaid = camp.FiMarcaid AND v.rn = 1
            LEFT JOIN dbo.tbCatBU bu ON bu.FiBUid = v.FiBUid
            LEFT JOIN dbo.tbCatSector sec ON sec.FiSectorid = bu.FiSectorid AND sec.FiClienteid = bu.FiClienteid
            WHERE (@anio IS NULL OR ad.FiAnio = @anio)
              AND (@sectorId IS NULL OR sec.FiSectorid = @sectorId)
              AND (@search IS NULL OR @search = N'' OR camp.FcNombreCampania LIKE N'%' + @search + N'%'
                   OR m.FcNombreMarcaCorto LIKE N'%' + @search + N'%'
                   OR sec.FcNombreSector LIKE N'%' + @search + N'%'
                   OR bu.FcNombreBU LIKE N'%' + @search + N'%')
            """;

        return await ExecuteScalarIntAsync(sql,
            p =>
            {
                p.Add(new SqlParameter("@anio", SqlDbType.Int) { Value = (object?)anio ?? DBNull.Value });
                p.Add(new SqlParameter("@sectorId", SqlDbType.Int) { Value = (object?)sectorId ?? DBNull.Value });
                p.Add(new SqlParameter("@search", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim() });
            },
            CancellationToken.None);
    }

    private static CampaniaEvaluacionDto MapCampaniaEvaluacion(SqlDataReader reader) => new()
    {
        CampaniaId = GetInt(reader, "CampaniaId"),
        Campania = GetString(reader, "Campania"),
        Marca = GetString(reader, "Marca"),
        Categoria = GetString(reader, "Categoria"),
        BU = GetString(reader, "BU"),
        Sector = GetString(reader, "Sector"),
        Anio = GetInt(reader, "Anio"),
        Activo = GetBool(reader, "Activo")
    };

    private static AdministracionCampaniaDto MapAdministracionCampania(SqlDataReader reader) => new()
    {
        AdministracionId = GetInt(reader, "AdministracionId"),
        Sector = GetString(reader, "Sector"),
        UnidadNegocio = GetString(reader, "UnidadNegocio"),
        Producto = GetString(reader, "Producto"),
        Marca = GetString(reader, "Marca"),
        Vehiculo = GetString(reader, "Vehiculo"),
        Campania = GetString(reader, "Campania"),
        Anio = GetInt(reader, "Anio"),
        Semanas = GetString(reader, "Semanas"),
        Activo = GetBool(reader, "Activo")
    };

    private async Task<List<T>> ExecuteQueryAsync<T>(
        string sql,
        Func<SqlDataReader, T> map,
        Action<SqlParameterCollection> addParameters,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("GenerarEvaluacionRepository requiere conexión SQL Server (SqlConnection).");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, sqlConnection) { CommandType = CommandType.Text };
        addParameters(command.Parameters);

        var result = new List<T>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            result.Add(map(reader));
        return result;
    }

    private async Task<int> ExecuteScalarIntAsync(
        string sql,
        Action<SqlParameterCollection> addParameters,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("GenerarEvaluacionRepository requiere conexión SQL Server (SqlConnection).");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, sqlConnection) { CommandType = CommandType.Text };
        addParameters(command.Parameters);

        var scalar = await command.ExecuteScalarAsync(cancellationToken);
        return scalar is long l ? (int)l : Convert.ToInt32(scalar);
    }

    private static int GetOrdinal(SqlDataReader reader, string columnName)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        throw new InvalidOperationException($"La columna '{columnName}' no existe en el resultado.");
    }

    private static string GetString(SqlDataReader reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? string.Empty : Convert.ToString(reader.GetValue(ord)) ?? string.Empty;
    }

    private static int GetInt(SqlDataReader reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? 0 : Convert.ToInt32(reader.GetValue(ord));
    }

    private static bool GetBool(SqlDataReader reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return !reader.IsDBNull(ord) && Convert.ToBoolean(reader.GetValue(ord));
    }
}
