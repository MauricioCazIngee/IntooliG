namespace IntooliG.Domain.Entities;

public static class UsuarioAppRoles
{
    public static string FromRolId(int rolId) => rolId == 1 ? "Admin" : "Usuario";
}
