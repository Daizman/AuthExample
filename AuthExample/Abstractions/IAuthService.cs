using AuthExample.Contracts;

namespace AuthExample.Abstractions;

public interface IAuthService
{
    LogInResponse SignUp(SignUpDto dto);
    LogInResponse? LogIn(LogInDto dto);
    bool LogOut(Guid userId);
    bool VerifyToken(Guid userId, string token);
    LogInResponse? Refresh(string refreshToken);
    void Revoke(string refreshToken);
}
