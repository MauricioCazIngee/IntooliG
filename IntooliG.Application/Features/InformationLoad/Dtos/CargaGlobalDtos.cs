namespace IntooliG.Application.Features.InformationLoad;

public class CargaGlobalFiltrosDto
{
    public List<PaisDto> Paises { get; set; } = [];
    public List<FormatoDto> Formatos { get; set; } = [];
    public List<VersionTVDto> VersionesTV { get; set; } = [];
    public List<MarcaDto> Marcas { get; set; } = [];
    public List<ProductoDto> Productos { get; set; } = [];
    public List<CiudadDto> Ciudades { get; set; } = [];
    public List<VehiculoDto> Vehiculos { get; set; } = [];
    public List<TargetDto> Targets { get; set; } = [];
    public List<MedicionDto> Mediciones { get; set; } = [];
}

public record PaisDto(int Id, string Nombre);
public record FormatoDto(int Id, string Nombre);
public record VersionTVDto(long Id, string Nombre);
public record MarcaDto(int Id, string Nombre);
public record ProductoDto(int Id, string Nombre);
public record CiudadDto(int Id, string Nombre, int? PaisId = null);
public record VehiculoDto(int Id, string Nombre);
public record TargetDto(int Id, string Nombre);
public record MedicionDto(int Id, string Nombre);

public class CargaGlobalUploadResultDto
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaTemporal { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public DateTime FechaCargaUtc { get; set; }
}

public class CargaGlobalProcessRequestDto
{
    public int PaisId { get; set; }
    public string Proceso { get; set; } = string.Empty;
}

public class CargaGlobalProcessResultDto
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaEjecucionUtc { get; set; }
}

public class CargaGlobalLogDto
{
    public long? Id { get; set; }
    public string Estatus { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime? FechaProceso { get; set; }
}

public class CargaGlobalEstadoCatalogacionDto
{
    public int PaisId { get; set; }
    public string Catalogo { get; set; } = string.Empty;
    public bool Completado { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
