using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string CreateToken(Usuario usuario);
}
