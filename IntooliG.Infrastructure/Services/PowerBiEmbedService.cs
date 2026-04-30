using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Abstractions.Services;
using IntooliG.Application.Features.PowerBi;
using IntooliG.Application.Options;
using Microsoft.Extensions.Options;

namespace IntooliG.Infrastructure.Services;

public sealed class PowerBiEmbedService : IPowerBiEmbedService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly PowerBiOptions _o;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICatRolPbiRepository _rolesPbi;

    public PowerBiEmbedService(
        HttpClient http,
        IOptions<PowerBiOptions> options,
        IUsuarioRepository usuarios,
        ICatRolPbiRepository rolesPbi)
    {
        _http = http;
        _o = options.Value;
        _usuarios = usuarios;
        _rolesPbi = rolesPbi;
    }

    public async Task<PowerBiEmbedResponseDto> GetReportEmbedForUserAsync(
        int usuarioId,
        string? reportId,
        bool isRdl,
        CancellationToken cancellationToken = default)
    {
        ValidateServicePrincipal();
        var rid = !string.IsNullOrWhiteSpace(reportId) ? reportId.Trim() : _o.ReportId;
        if (string.IsNullOrWhiteSpace(rid))
        {
            throw new InvalidOperationException("Indique PowerBi:ReportId o el parámetro reportId.");
        }

        ValidateBase();

        var pbiRol = await ResolvePbiRoleNameAsync(usuarioId, cancellationToken).ConfigureAwait(false);
        var rlsUser = await ResolveRlsUserNameAsync(usuarioId, cancellationToken).ConfigureAwait(false);

        var appToken = await RequestAzureAdTokenAsync(cancellationToken).ConfigureAwait(false);
        var baseApi = _o.ApiUrl.TrimEnd('/');

        var report = await GetReportAsync(baseApi, _o.WorkspaceId, rid, appToken, cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(report.EmbedUrl))
        {
            throw new InvalidOperationException("Power BI no devolvió embedUrl. Verifique WorkspaceId y el id del informe.");
        }

        var reportTypeText = report.ReportType?.ToString() ?? string.Empty;
        var isPaginated = isRdl
            || (reportTypeText.Contains("Paginated", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(reportTypeText));

        var embed = await GenerateReportTokenAsync(
                baseApi,
                _o.WorkspaceId,
                rid,
                appToken,
                pbiRol,
                rlsUser,
                _o.UseRlsIdentities ? report.DatasetId : null,
                cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(embed.Token))
        {
            throw new InvalidOperationException("No se pudo obtener el token de embebido de Power BI.");
        }

        return new PowerBiEmbedResponseDto
        {
            EmbedToken = embed.Token!,
            TokenExpiry = embed.Expiration ?? string.Empty,
            Id = rid,
            ReportId = rid,
            EmbedUrl = report.EmbedUrl!,
            Type = "report",
            IsPaginatedReport = isPaginated
        };
    }

    public async Task<PowerBiEmbedResponseDto> GetDashboardEmbedAsync(
        string? dashboardId,
        CancellationToken cancellationToken = default)
    {
        ValidateServicePrincipal();
        ValidateBase();
        var id = !string.IsNullOrWhiteSpace(dashboardId) ? dashboardId.Trim() : _o.DashboardId;
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidOperationException("Indique PowerBi:DashboardId o el parámetro dashboardId.");
        }

        var appToken = await RequestAzureAdTokenAsync(cancellationToken).ConfigureAwait(false);
        var baseApi = _o.ApiUrl.TrimEnd('/');

        var meta = await GetDashboardAsync(baseApi, _o.WorkspaceId, id, appToken, cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(meta.EmbedUrl))
        {
            throw new InvalidOperationException("El panel no expone embedUrl. Compruebe el id del panel.");
        }

        var embed = await GenerateDashboardTokenAsync(
                baseApi, _o.WorkspaceId, id, appToken, cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(embed.Token))
        {
            throw new InvalidOperationException("No se pudo generar el token de embebido del panel.");
        }

        return new PowerBiEmbedResponseDto
        {
            EmbedToken = embed.Token!,
            TokenExpiry = embed.Expiration ?? string.Empty,
            Id = id,
            ReportId = id,
            EmbedUrl = meta.EmbedUrl!,
            Type = "dashboard"
        };
    }

    public async Task<PowerBiEmbedResponseDto> GetTileEmbedAsync(
        string? dashboardId,
        string? tileId,
        CancellationToken cancellationToken = default)
    {
        ValidateServicePrincipal();
        ValidateBase();
        var dId = !string.IsNullOrWhiteSpace(dashboardId) ? dashboardId.Trim() : _o.TileDashboardId;
        if (string.IsNullOrWhiteSpace(dId) && !string.IsNullOrWhiteSpace(_o.DashboardId))
        {
            dId = _o.DashboardId;
        }
        var tId = !string.IsNullOrWhiteSpace(tileId) ? tileId.Trim() : _o.TileId;
        if (string.IsNullOrWhiteSpace(dId) || string.IsNullOrWhiteSpace(tId))
        {
            throw new InvalidOperationException("Indique PowerBi:TileDashboardId (o dashboardId) y PowerBi:TileId (o tileId).");
        }

        var appToken = await RequestAzureAdTokenAsync(cancellationToken).ConfigureAwait(false);
        var baseApi = _o.ApiUrl.TrimEnd('/');

        var meta = await GetTileAsync(baseApi, _o.WorkspaceId, dId, tId, appToken, cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(meta.EmbedUrl))
        {
            throw new InvalidOperationException("El mosaico no expone embedUrl. Compruebe ids de panel y mosaico.");
        }

        var embed = await GenerateTileTokenAsync(
                baseApi, _o.WorkspaceId, dId, tId, appToken, cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(embed.Token))
        {
            throw new InvalidOperationException("No se pudo generar el token de embebido del mosaico.");
        }

        return new PowerBiEmbedResponseDto
        {
            EmbedToken = embed.Token!,
            TokenExpiry = embed.Expiration ?? string.Empty,
            Id = tId,
            ReportId = tId,
            DashboardId = dId,
            EmbedUrl = meta.EmbedUrl!,
            Type = "tile"
        };
    }

    private void ValidateServicePrincipal()
    {
        if (!string.Equals(_o.AuthenticationType, "ServicePrincipal", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Solo está implementada la autenticación Service Principal. Ajuste PowerBi:AuthenticationType.");
        }
    }

    private void ValidateBase()
    {
        if (string.IsNullOrWhiteSpace(_o.TenantId)) throw new InvalidOperationException("Falta PowerBi:TenantId.");
        if (string.IsNullOrWhiteSpace(_o.ApplicationId)) throw new InvalidOperationException("Falta PowerBi:ApplicationId.");
        if (string.IsNullOrWhiteSpace(_o.ClientSecret)) throw new InvalidOperationException("Falta PowerBi:ClientSecret (user-secrets o entorno).");
        if (string.IsNullOrWhiteSpace(_o.WorkspaceId)) throw new InvalidOperationException("Falta PowerBi:WorkspaceId.");
    }

    private async Task<string> ResolvePbiRoleNameAsync(int usuarioId, CancellationToken cancellationToken)
    {
        if (usuarioId == 0)
        {
            return _o.DefaultPbiRol;
        }
        var u = await _usuarios.GetByIdAsync(usuarioId, cancellationToken).ConfigureAwait(false);
        if (u is null || u.RolPbiId is not > 0)
        {
            return _o.DefaultPbiRol;
        }
        var name = await _rolesPbi.GetNombreByIdAsync(u.RolPbiId.Value, cancellationToken)
            .ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(name) ? _o.DefaultPbiRol : name.Trim();
    }

    private async Task<string> ResolveRlsUserNameAsync(int usuarioId, CancellationToken cancellationToken)
    {
        if (usuarioId == 0)
        {
            return _o.EffectiveRlsUserName;
        }
        var u = await _usuarios.GetByIdAsync(usuarioId, cancellationToken).ConfigureAwait(false);
        if (u is null)
        {
            return _o.EffectiveRlsUserName;
        }
        return !string.IsNullOrWhiteSpace(u.Email) ? u.Email.Trim() : _o.EffectiveRlsUserName;
    }

    private async Task<string> RequestAzureAdTokenAsync(CancellationToken cancellationToken)
    {
        var host = _o.AuthorityHost.TrimEnd('/');
        var url = $"{host}/{_o.TenantId}/oauth2/v2.0/token";
        var body = new List<KeyValuePair<string, string>>
        {
            new("client_id", _o.ApplicationId),
            new("client_secret", _o.ClientSecret),
            new("scope", _o.Scope),
            new("grant_type", "client_credentials")
        };
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(body)
        };
        var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Error al obtener token de Azure AD ({(int)response.StatusCode}): {Truncate(json, 500)}");
        }
        var tokenResponse = JsonSerializer.Deserialize<AadTokenResponse>(json, JsonOptions);
        if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
        {
            throw new InvalidOperationException("Respuesta de Azure AD sin access_token.");
        }
        return tokenResponse.AccessToken;
    }

    private async Task<PbiReport> GetReportAsync(
        string baseApi, string groupId, string reportId, string appAccessToken, CancellationToken cancellationToken)
    {
        var url = $"{baseApi}/v1.0/myorg/groups/{groupId}/reports/{reportId}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", appAccessToken);
        var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Error al leer el informe en Power BI ({(int)response.StatusCode}): {Truncate(json, 500)}. Asegúrese de que el Service Principal esté en el workspace (admin o colaborador).");
        }
        return JsonSerializer.Deserialize<PbiReport>(json, JsonOptions) ?? new PbiReport();
    }

    private async Task<PbiDashboard> GetDashboardAsync(
        string baseApi, string groupId, string dashboardId, string appToken, CancellationToken cancellationToken)
    {
        var url = $"{baseApi}/v1.0/myorg/groups/{groupId}/dashboards/{dashboardId}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", appToken);
        var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Error al leer el panel en Power BI ({(int)response.StatusCode}): {Truncate(json, 500)}");
        }
        return JsonSerializer.Deserialize<PbiDashboard>(json, JsonOptions) ?? new PbiDashboard();
    }

    private async Task<PbiTile> GetTileAsync(
        string baseApi, string groupId, string dashboardId, string tileId, string appToken, CancellationToken cancellationToken)
    {
        var url = $"{baseApi}/v1.0/myorg/groups/{groupId}/dashboards/{dashboardId}/tiles/{tileId}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", appToken);
        var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Error al leer el mosaico en Power BI ({(int)response.StatusCode}): {Truncate(json, 500)}");
        }
        return JsonSerializer.Deserialize<PbiTile>(json, JsonOptions) ?? new PbiTile();
    }

    private async Task<PbiGenerateToken> GenerateReportTokenAsync(
        string baseApi,
        string groupId,
        string reportId,
        string appToken,
        string pbiRol,
        string rlsUser,
        string? datasetId,
        CancellationToken cancellationToken)
    {
        var url = $"{baseApi}/v1.0/myorg/groups/{groupId}/reports/{reportId}/GenerateToken";
        var body = BuildReportTokenRequest(pbiRol, rlsUser, datasetId);
        return await PostGenerateTokenAsync(url, appToken, body, cancellationToken).ConfigureAwait(false);
    }

    private static ReportGenerateTokenRequest BuildReportTokenRequest(string pbiRol, string rlsUser, string? datasetId)
    {
        if (!string.IsNullOrWhiteSpace(datasetId))
        {
            return new ReportGenerateTokenRequest
            {
                AccessLevel = "View",
                Identities = new List<TokenIdentity>
                {
                    new()
                    {
                        Username = rlsUser,
                        Roles = [pbiRol],
                        Datasets = [datasetId!]
                    }
                }
            };
        }
        return new ReportGenerateTokenRequest { AccessLevel = "View" };
    }

    private async Task<PbiGenerateToken> GenerateDashboardTokenAsync(
        string baseApi, string groupId, string dashboardId, string appToken, CancellationToken cancellationToken)
    {
        var url = $"{baseApi}/v1.0/myorg/groups/{groupId}/dashboards/{dashboardId}/GenerateToken";
        return await PostSimpleGenerateTokenAsync(url, appToken, cancellationToken).ConfigureAwait(false);
    }

    private async Task<PbiGenerateToken> GenerateTileTokenAsync(
        string baseApi, string groupId, string dashboardId, string tileId, string appToken, CancellationToken cancellationToken)
    {
        var url = $"{baseApi}/v1.0/myorg/groups/{groupId}/dashboards/{dashboardId}/tiles/{tileId}/GenerateToken";
        return await PostSimpleGenerateTokenAsync(url, appToken, cancellationToken).ConfigureAwait(false);
    }

    private async Task<PbiGenerateToken> PostSimpleGenerateTokenAsync(
        string url, string appToken, CancellationToken cancellationToken) =>
        await PostGenerateTokenAsync(
            url, appToken, new SimpleViewTokenBody { AccessLevel = "View" }, cancellationToken)
            .ConfigureAwait(false);

    private async Task<PbiGenerateToken> PostGenerateTokenAsync(
        string url,
        string appToken,
        object body,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(
            body,
            body.GetType(),
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = null
            });
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", appToken);
        var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var resp = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Error al generar token de embebido ({(int)response.StatusCode}): {Truncate(resp, 500)}");
        }
        return JsonSerializer.Deserialize<PbiGenerateToken>(resp, JsonOptions) ?? new PbiGenerateToken();
    }

    private static string Truncate(string? s, int max) =>
        s is null or { Length: 0 } ? string.Empty : (s.Length <= max ? s : s[..max] + "…");

    private sealed class AadTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    private sealed class PbiReport
    {
        [JsonPropertyName("embedUrl")]
        public string? EmbedUrl { get; set; }

        [JsonPropertyName("datasetId")]
        public string? DatasetId { get; set; }

        /// <summary>Enum numérico o string según la versión de la API.</summary>
        [JsonPropertyName("reportType")]
        public object? ReportType { get; set; }
    }

    private sealed class PbiDashboard
    {
        [JsonPropertyName("embedUrl")]
        public string? EmbedUrl { get; set; }
    }

    private sealed class PbiTile
    {
        [JsonPropertyName("embedUrl")]
        public string? EmbedUrl { get; set; }
    }

    private sealed class PbiGenerateToken
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        [JsonPropertyName("expiration")]
        public string? Expiration { get; set; }
    }

    private sealed class SimpleViewTokenBody
    {
        [JsonPropertyName("accessLevel")]
        public string AccessLevel { get; set; } = "View";
    }

    private sealed class ReportGenerateTokenRequest
    {
        [JsonPropertyName("accessLevel")]
        public string AccessLevel { get; set; } = "View";
        [JsonPropertyName("identities")]
        public List<TokenIdentity>? Identities { get; set; }
    }

    private sealed class TokenIdentity
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = "";
        [JsonPropertyName("roles")]
        public string[] Roles { get; set; } = Array.Empty<string>();
        [JsonPropertyName("datasets")]
        public string[] Datasets { get; set; } = Array.Empty<string>();
    }
}
