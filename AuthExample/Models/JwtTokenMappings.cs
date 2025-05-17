using AuthExample.Contracts;

namespace AuthExample.Models;

public partial class JwtToken
{
    public JwtTokenVm ToJwtTokenVm()
    {
        return new JwtTokenVm(UserId, Token, ExpiresAt);
    }
}
