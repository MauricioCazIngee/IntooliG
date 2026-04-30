namespace IntooliG.Application.Features.Medios;

public record MedioLookupDto(int Id, string Nombre);

public record MedioDto(int Id, string NombreMedio, string NombreMedioGenerico, bool Activo);

public record MedioCreateRequest(string NombreMedio, string NombreMedioGenerico, bool Activo);

public record MedioUpdateRequest(string NombreMedio, string NombreMedioGenerico, bool Activo);

public record MedioClienteDto(int MedioId, int ClienteId, string NombreMedio, string NombreCliente, bool EsNacional);

public record MedioClienteCreateRequest(int MedioId, int ClienteId, bool EsNacional);

public record MedioClienteUpdateRequest(bool EsNacional);

public record MedioClientePorClienteDto(int MedioId, string NombreMedio, bool EsNacional);
