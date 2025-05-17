using AuthExample.Abstractions;
using AuthExample.Contracts;
using AuthExample.Database;
using AuthExample.Models;
using AuthExample.Utils;

namespace AuthExample.Services;

public class AuthService(
    AppDbContext dbContext,
    IJwtTokenGenerator jwtTokenGenerator
) : IAuthService
{
    public JwtTokenVm? LogIn(LogInDto dto)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.UserName == dto.UserName);
        if (user is null)
            return null;
        if (!PasswordHasher.VerifyPassword(user.Password, dto.Password))
            return null;

        var token = UpdateToken(user);
        dbContext.SaveChanges();

        return token?.ToJwtTokenVm();
    }

    public bool LogOut(Guid userId)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Id == userId);
        if (user is null)
            return false;

        var token = dbContext.JwtTokens.FirstOrDefault(x => x.UserId == userId);
        if (token is null)
        {
            return false;
        }
        dbContext.Remove(token);
        dbContext.SaveChanges();

        return true;
    }

    public JwtTokenVm SignUp(SignUpDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Password = PasswordHasher.HashPassword(dto.Password),
        };

        dbContext.Users.Add(user);

        dbContext.SaveChanges();

        var token = UpdateToken(user);

        dbContext.SaveChanges();

        return token.ToJwtTokenVm();
    }

    public bool VerifyToken(Guid userId, string token)
    {
        var jwtToken = dbContext.JwtTokens.FirstOrDefault(x => x.UserId == userId);
        if (jwtToken is null)
            return false;

        return jwtToken.Token == token && jwtToken.ExpiresAt > DateTime.UtcNow;
    }

    private JwtToken UpdateToken(User user)
    {
        var token = jwtTokenGenerator.Generate(user);
        var oldToken = dbContext.JwtTokens.FirstOrDefault(t => t.UserId == user.Id);
        if (oldToken is not null)
        {
            dbContext.Remove(oldToken);
        }
        dbContext.JwtTokens.Add(token);

        return token;
    }
}
