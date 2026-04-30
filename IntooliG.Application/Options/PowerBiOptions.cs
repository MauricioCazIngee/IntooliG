namespace IntooliG.Application.Options;

public class PowerBiOptions
{
    public const string SectionName = "PowerBi";

    /// <summary>ServicePrincipal (recomendado) u otros métodos soportados por la API.</summary>
    public string AuthenticationType { get; set; } = "ServicePrincipal";

    public string TenantId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string WorkspaceId { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;

    /// <summary>Informe u opciones por defecto: dashboard y tile (legado MVC EmbedDashboard/EmbedTile).</summary>
    public string DashboardId { get; set; } = string.Empty;
    public string TileDashboardId { get; set; } = string.Empty;
    public string TileId { get; set; } = string.Empty;

    /// <summary>Valor por defecto como en el viejo mvc (string rolePbi = "ADMIN").</summary>
    public string DefaultPbiRol { get; set; } = "ADMIN";

    /// <summary>Si el usuario no tiene email, se usa en identidades RLS (o el de Azure si aplica).</summary>
    public string EffectiveRlsUserName { get; set; } = "pbi-embed@local";

    /// <summary>False: solo <c>{"accessLevel":"View"}</c> (útil si el dataset no usa RLS y el token con identidades falla).</summary>
    public bool UseRlsIdentities { get; set; } = true;

    public string AuthorityHost { get; set; } = "https://login.microsoftonline.com";
    public string Scope { get; set; } = "https://analysis.windows.net/powerbi/api/.default";
    public string ApiUrl { get; set; } = "https://api.powerbi.com";
}
