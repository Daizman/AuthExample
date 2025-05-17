using AuthExample.Contracts;

namespace AuthExample.Abstractions;

public interface IAuthService
{
    JwtTokenVm SignUp(SignUpDto dto);
    JwtTokenVm? LogIn(LogInDto dto);
    bool LogOut(Guid userId);
    bool VerifyToken(Guid userId, string token);
}
