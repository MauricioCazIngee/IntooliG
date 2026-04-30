using System.Data;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.InformationLoad;
using IntooliG.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class InformationLoadRepository : IInformationLoadRepository
{
    private readonly AppDbContext _db;

    public InformationLoadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CargaGlobalFiltrosDto> GetCargaGlobalFiltrosAsync(
        int clienteId,
        int? paisId,
        CancellationToken cancellationToken = default)
    {
        var dto = new CargaGlobalFiltrosDto();

        dto.Paises = await QueryListSafeAsync(
            """
            SELECT FiPaisid AS Id, FcNombrePais AS Nombre
            FROM tbCatPais
            WHERE FbActivo = 1
            ORDER BY FcNombrePais;
            """,
            null,
            r => new PaisDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.Formatos = await QueryListSafeAsync(
            """
            SELECT FiFormatoid AS Id, FcNombreFormato AS Nombre
            FROM TbCatFormato
            WHERE ISNULL(FbEstatus, 1) = 1
            ORDER BY FcNombreFormato;
            """,
            null,
            r => new FormatoDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.VersionesTV = await QueryListSafeAsync(
            """
            SELECT FiVersionTV AS Id, FcNombreVersionTV AS Nombre
            FROM tbCatVersionTV
            ORDER BY FcNombreVersionTV;
            """,
            null,
            r => new VersionTVDto(GetLong(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.Marcas = await QueryListSafeAsync(
            """
            SELECT FiMarcaid AS Id, FcNombreMarcaCorto AS Nombre
            FROM tbCatMarca
            WHERE FiClienteid = @clienteId AND ISNULL(FbEstatus, 1) = 1
            ORDER BY FcNombreMarcaCorto;
            """,
            [new SqlParameter("@clienteId", SqlDbType.Int) { Value = clienteId }],
            r => new MarcaDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.Productos = await QueryListSafeAsync(
            """
            SELECT FiProductoid AS Id, FcNombreProducto AS Nombre
            FROM tbCatProducto
            WHERE FiClienteid = @clienteId AND ISNULL(FbEstatus, 1) = 1
            ORDER BY FcNombreProducto;
            """,
            [new SqlParameter("@clienteId", SqlDbType.Int) { Value = clienteId }],
            r => new ProductoDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.Ciudades = await QueryListSafeAsync(
            """
            SELECT c.FiCiudadid AS Id, c.FcNombreCiudad AS Nombre, e.FiPaisid AS PaisId
            FROM tbCatCiudad c
            INNER JOIN tbCatEstado e ON c.FiEstadoid = e.FiEstadoid
            WHERE ISNULL(c.FbActivo, 1) = 1
              AND (@paisId IS NULL OR e.FiPaisid = @paisId)
            ORDER BY c.FcNombreCiudad;
            """,
            [new SqlParameter("@paisId", SqlDbType.Int) { Value = (object?)paisId ?? DBNull.Value }],
            r => new CiudadDto(GetInt(r, "Id"), GetString(r, "Nombre"), GetNullableInt(r, "PaisId")),
            cancellationToken);

        dto.Vehiculos = await QueryListSafeAsync(
            """
            SELECT FiVehiculoid AS Id, FcNombreVehiculo AS Nombre
            FROM tbCatVehiculo
            WHERE ISNULL(FbEstatus, 1) = 1
            ORDER BY FcNombreVehiculo;
            """,
            null,
            r => new VehiculoDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.Targets = await QueryListSafeAsync(
            """
            SELECT FiTargetid AS Id, FcNombreTarget AS Nombre
            FROM TbCatTargets
            WHERE ISNULL(FbEstatus, 1) = 1
            ORDER BY FcNombreTarget;
            """,
            null,
            r => new TargetDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        dto.Mediciones = await QueryListSafeAsync(
            """
            SELECT FiMedicionid AS Id, FcNombreMedicion AS Nombre
            FROM TbCatMediciones
            WHERE ISNULL(FbEstatus, 1) = 1
            ORDER BY FcNombreMedicion;
            """,
            null,
            r => new MedicionDto(GetInt(r, "Id"), GetString(r, "Nombre")),
            cancellationToken);

        return dto;
    }

    public async Task<CargaGlobalLogDto?> GetUltimoLogProcesoAsync(
        int clienteId,
        int paisId,
        CancellationToken cancellationToken = default)
    {
        var rows = await QueryListSafeAsync(
            """
            SELECT TOP (1)
                CAST(FiLogCargaid AS bigint) AS Id,
                CAST(ISNULL(FcEstatus, '') AS varchar(200)) AS Estatus,
                CAST(ISNULL(FcMensaje, '') AS varchar(max)) AS Mensaje,
                ISNULL(FdFechaProceso, FdFechaCreacion) AS FechaProceso
            FROM TbLogCarga
            WHERE FiClienteid = @clienteId
              AND (@paisId = 0 OR FiPaisid = @paisId)
            ORDER BY ISNULL(FdFechaProceso, FdFechaCreacion) DESC, FiLogCargaid DESC;
            """,
            [
                new SqlParameter("@clienteId", SqlDbType.Int) { Value = clienteId },
                new SqlParameter("@paisId", SqlDbType.Int) { Value = paisId }
            ],
            r => new CargaGlobalLogDto
            {
                Id = GetNullableLong(r, "Id"),
                Estatus = GetString(r, "Estatus"),
                Mensaje = GetString(r, "Mensaje"),
                FechaProceso = GetNullableDateTime(r, "FechaProceso")
            },
            cancellationToken);

        return rows.FirstOrDefault();
    }

    public async Task<IReadOnlyList<CargaGlobalEstadoCatalogacionDto>> GetEstadoCatalogacionPaisAsync(
        int paisId,
        CancellationToken cancellationToken = default)
    {
        return await QueryListSafeAsync(
            """
            SELECT
                FiPaisid AS PaisId,
                ISNULL(FcCatalogo, '') AS Catalogo,
                CAST(ISNULL(FbCompletado, 0) AS bit) AS Completado,
                ISNULL(FdFechaActualizacion, FdFechaCreacion) AS FechaActualizacion
            FROM tbLogCargaGlobalPaises
            WHERE FiPaisid = @paisId
            ORDER BY FcCatalogo;
            """,
            [new SqlParameter("@paisId", SqlDbType.Int) { Value = paisId }],
            r => new CargaGlobalEstadoCatalogacionDto
            {
                PaisId = GetInt(r, "PaisId"),
                Catalogo = GetString(r, "Catalogo"),
                Completado = GetBool(r, "Completado"),
                FechaActualizacion = GetNullableDateTime(r, "FechaActualizacion")
            },
            cancellationToken);
    }

    public async Task<CargaGlobalProcessResultDto> ExecuteProcessAsync(
        int clienteId,
        int usuarioId,
        int paisId,
        string proceso,
        CancellationToken cancellationToken = default)
    {
        // Seguridad básica: solo letras, números y "_".
        if (!proceso.All(ch => char.IsLetterOrDigit(ch) || ch == '_'))
            throw new InvalidOperationException("Nombre de proceso inválido.");

        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("InformationLoadRepository requiere SqlConnection.");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        // Compatibilidad legacy: se ejecuta el SP por nombre. Si el SP no existe,
        // el error SQL se captura y se regresa al front como mensaje.
        await using var command = new SqlCommand($"EXEC [{proceso}]", sqlConnection)
        {
            CommandType = CommandType.Text
        };

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
            return new CargaGlobalProcessResultDto
            {
                Exito = true,
                Mensaje = $"Proceso ejecutado: {proceso}",
                FechaEjecucionUtc = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new CargaGlobalProcessResultDto
            {
                Exito = false,
                Mensaje = $"Error al ejecutar '{proceso}': {ex.Message}",
                FechaEjecucionUtc = DateTime.UtcNow
            };
        }
    }

    public async Task<bool> FinalizarCatalogacionPaisAsync(
        int paisId,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("InformationLoadRepository requiere SqlConnection.");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        // Intento de finalización estándar; si no existe estructura exacta, no rompe el módulo.
        const string sql = """
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                       WHERE TABLE_NAME = 'tbLogCargaGlobalPaises'
                         AND COLUMN_NAME = 'FbCatalogacionFinalizada')
            BEGIN
                UPDATE tbLogCargaGlobalPaises
                SET FbCatalogacionFinalizada = 1
                WHERE FiPaisid = @paisId;
            END
            """;

        await using var command = new SqlCommand(sql, sqlConnection);
        command.Parameters.Add(new SqlParameter("@paisId", SqlDbType.Int) { Value = paisId });
        await command.ExecuteNonQueryAsync(cancellationToken);
        return true;
    }

    private async Task<List<T>> QueryListSafeAsync<T>(
        string sql,
        SqlParameter[]? parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("InformationLoadRepository requiere SqlConnection.");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, sqlConnection) { CommandType = CommandType.Text };
        if (parameters is { Length: > 0 })
            command.Parameters.AddRange(parameters);

        var result = new List<T>();
        try
        {
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(map(reader));
            }
        }
        catch (SqlException)
        {
            // Tabla/columna no disponible en este ambiente -> devolver vacío.
            return [];
        }

        return result;
    }

    private static int GetOrdinalSafe(IDataRecord r, string col)
    {
        for (var i = 0; i < r.FieldCount; i++)
        {
            if (r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    private static string GetString(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return string.Empty;
        return Convert.ToString(r.GetValue(ord)) ?? string.Empty;
    }

    private static int GetInt(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return 0;
        return Convert.ToInt32(r.GetValue(ord));
    }

    private static int? GetNullableInt(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return null;
        return Convert.ToInt32(r.GetValue(ord));
    }

    private static long GetLong(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return 0;
        return Convert.ToInt64(r.GetValue(ord));
    }

    private static long? GetNullableLong(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return null;
        return Convert.ToInt64(r.GetValue(ord));
    }

    private static bool GetBool(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return false;
        return Convert.ToBoolean(r.GetValue(ord));
    }

    private static DateTime? GetNullableDateTime(IDataRecord r, string col)
    {
        var ord = GetOrdinalSafe(r, col);
        if (ord < 0 || r.IsDBNull(ord))
            return null;
        return Convert.ToDateTime(r.GetValue(ord));
    }
}
