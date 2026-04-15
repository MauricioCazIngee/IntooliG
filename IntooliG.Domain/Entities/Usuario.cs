using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntooliG.Domain.Entities;

[Table("tbUser")]
public class Usuario
{
    [Key]
    [Column("FiUsuarioId")]
    public int Id { get; set; }

    [Column("FiClienteId")]
    public int ClienteId { get; set; }

    [Column("FcNombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("FcApellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [Column("FcEmail")]
    public string Email { get; set; } = string.Empty;

    [Column("FcPassword")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("FdFechadeNacimiento")]
    public DateTime? FechaNacimiento { get; set; }

    [Column("FbCorreoVerificado")]
    public bool CorreoVerificado { get; set; }

    [Column("FcCodigoActivacion")]
    public Guid CodigoActivacion { get; set; }

    [Column("FcResetPasswordCode")]
    public string? ResetPasswordCode { get; set; }

    [Column("FiIntentos")]
    public int? Intentos { get; set; }

    [Column("FbPrimerAcceso")]
    public bool PrimerAcceso { get; set; }

    [Column("FdExpiracionPassword")]
    public DateTime? ExpiracionPassword { get; set; }

    [Column("FdFechaCreacion")]
    public DateTime FechaCreacion { get; set; }

    [Column("FbEstatus")]
    public bool Estatus { get; set; }

    [Column("FiRolID")]
    public int RolId { get; set; }

    [Column("FiRolPbiID")]
    public int? RolPbiId { get; set; }

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {Apellidos}";
}
