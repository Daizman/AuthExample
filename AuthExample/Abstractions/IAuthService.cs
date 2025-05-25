using AuthExample.Contracts;

namespace AuthExample.Abstractions;

public interface IAuthService
{
    Task<LogInResponse> SignUpAsync(SignUpDto dto);
    Task<LogInResponse?> LogInAsync(LogInDto dto);
    Task<bool> LogOutAsync(Guid userId);
    Task<bool> VerifyTokenAsync(Guid userId, string token);
    Task<LogInResponse?> RefreshAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}
