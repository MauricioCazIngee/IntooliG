using System.Data;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Radar;
using IntooliG.Application.Features.Radar.Dtos;
using IntooliG.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class RadarRepository : IRadarRepository
{
    private readonly AppDbContext _db;

    public RadarRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RadarEstadoDto>> GetEstadosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarEstados",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@marca", marca ?? string.Empty);
                cmd.Parameters.AddWithValue("@pais", pais);
            },
            reader => new RadarEstadoDto(
                FcNombreCategoria: GetString(reader, "FcNombreCategoria"),
                Semana: GetShort(reader, "Semana"),
                FcNombreEstado: GetString(reader, "FcNombreEstado"),
                FcNombreCodigoMapa: GetString(reader, "FcNombreCodigoMapa"),
                FnPrecio: GetDoubleNullable(reader, "FnPrecio")),
            cancellationToken);
    }

    public async Task<IReadOnlyList<RadarInversionMarcaMediosDto>> GetInversionMarcaMediosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarInversionMarcaMedios",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@marca", marca ?? string.Empty);
                cmd.Parameters.AddWithValue("@pais", pais);
            },
            reader => new RadarInversionMarcaMediosDto(
                FcNombreCategoria: GetString(reader, "FcNombreCategoria"),
                Anio: GetShort(reader, "Anio"),
                Semana: GetShort(reader, "semana"),
                FcNombreMedio: GetString(reader, "FcNombreMedio"),
                FnPrecio: GetDoubleNullable(reader, "FnPrecio")),
            cancellationToken);
    }

    public async Task<IReadOnlyList<RadarInversionMarcaMediosVersionDto>> GetInversionMarcaMediosVersionAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        string medio,
        int pais,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarInversionMarcaMediosVersion",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@marca", marca ?? string.Empty);
                cmd.Parameters.AddWithValue("@medio", medio ?? string.Empty);
                cmd.Parameters.AddWithValue("@pais", pais);
            },
            reader => new RadarInversionMarcaMediosVersionDto(
                FcNombreCliente: GetString(reader, "FcNombreCliente"),
                FcNombreMedio: GetString(reader, "FcNombreMedio"),
                FcNombreVersionFuente: GetString(reader, "FcNombreVersionFuente"),
                Ins: GetInt(reader, "Ins")),
            cancellationToken);
    }

    public async Task<IReadOnlyList<RadarMarcaFactoresRiesgoDto>> GetMarcaFactoresRiesgoAsync(
        int anio,
        int semana,
        int categoria,
        int rubro,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarMarcaFactoresRiesgo",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@rubro", rubro);
                cmd.Parameters.AddWithValue("@marca", marca ?? string.Empty);
                cmd.Parameters.AddWithValue("@pais", pais);
            },
            reader => new RadarMarcaFactoresRiesgoDto(
                FiAnio: GetInt(reader, "FiAnio"),
                FiSemana: GetInt(reader, "FiSemana"),
                FcNombreMarca: GetString(reader, "FcNombreMarca"),
                Valor: GetDecimalNullable(reader, "Valor")),
            cancellationToken);
    }

    public async Task<IReadOnlyList<RadarMarcaValoresAgregadosDto>> GetMarcaValoresAgregadosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarMarcaValoresAgregados",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@marca", marca ?? string.Empty);
                cmd.Parameters.AddWithValue("@pais", pais);
            },
            reader => new RadarMarcaValoresAgregadosDto(
                FiRubroid: GetInt(reader, "FiRubroid"),
                FcNombreConcepto: GetString(reader, "FcNombreConcepto"),
                FbEstatus: GetInt(reader, "FbEstatus")),
            cancellationToken);
    }

    public async Task<IReadOnlyList<RadarMrcaWeeksDto>> GetMrcaWeeksAsync(
        int anio,
        int semana,
        int categoria,
        int pais,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarMrcaWeeks",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
                cmd.Parameters.AddWithValue("@pais", pais);
            },
            reader => new RadarMrcaWeeksDto(
                FechaOrden: GetString(reader, "FechaOrden"),
                Anio: GetInt(reader, "Anio"),
                Semana: GetInt(reader, "Semana"),
                FcNombreMarca: GetString(reader, "FcNombreMarca"),
                Valor: GetDoubleNullable(reader, "valor")),
            cancellationToken);
    }

    public async Task<IReadOnlyList<RadarTop5Dto>> GetTop5Async(
        int anio,
        int semana,
        int categoria,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            "SPWEBRadarTop5",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@semana", semana);
                cmd.Parameters.AddWithValue("@categoria", categoria);
            },
            reader => new RadarTop5Dto(
                FcNombreConcepto: GetString(reader, "FcNombreConcepto")),
            cancellationToken);
    }

    private async Task<IReadOnlyList<T>> ExecuteStoredProcedureAsync<T>(
        string spName,
        Action<SqlCommand> parameterBuilder,
        Func<SqlDataReader, T> mapper,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
            throw new InvalidOperationException("RadarRepository requiere conexión SQL Server (SqlConnection).");

        if (sqlConnection.State != ConnectionState.Open)
            await sqlConnection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(spName, sqlConnection)
        {
            CommandType = CommandType.StoredProcedure
        };
        parameterBuilder(command);

        var result = new List<T>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(mapper(reader));
        }
        return result;
    }

    private static int GetOrdinal(IDataRecord reader, string columnName)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        throw new InvalidOperationException($"La columna '{columnName}' no existe en el resultado del SP.");
    }

    private static string GetString(IDataRecord reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? string.Empty : Convert.ToString(reader.GetValue(ord)) ?? string.Empty;
    }

    private static int GetInt(IDataRecord reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? 0 : Convert.ToInt32(reader.GetValue(ord));
    }

    private static short GetShort(IDataRecord reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? (short)0 : Convert.ToInt16(reader.GetValue(ord));
    }

    private static double? GetDoubleNullable(IDataRecord reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? null : Convert.ToDouble(reader.GetValue(ord));
    }

    private static decimal? GetDecimalNullable(IDataRecord reader, string columnName)
    {
        var ord = GetOrdinal(reader, columnName);
        return reader.IsDBNull(ord) ? null : Convert.ToDecimal(reader.GetValue(ord));
    }
}
