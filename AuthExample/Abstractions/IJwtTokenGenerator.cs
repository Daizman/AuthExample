using AuthExample.Models;

namespace AuthExample.Abstractions;

public interface IJwtTokenGenerator
{
    JwtToken Generate(User user);

    RefreshToken GetRefreshToken(Guid userId);
}
