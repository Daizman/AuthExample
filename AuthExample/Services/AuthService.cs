using AuthExample.Abstractions;
using AuthExample.Contracts;
using AuthExample.Database;
using AuthExample.Models;
using AuthExample.Utils;
using Microsoft.EntityFrameworkCore;

namespace AuthExample.Services;

public class AuthService(
    AppDbContext dbContext,
    IJwtTokenGenerator jwtTokenGenerator
) : IAuthService
{
    public LogInResponse? LogIn(LogInDto dto)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.UserName == dto.UserName);
        if (user is null)
            return null;
        if (!PasswordHasher.VerifyPassword(user.Password, dto.Password))
            return null;

        var (jwt, refresh) = UpdateToken(user);
        dbContext.SaveChanges();

        return CreateResponse(jwt, refresh);
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

    public LogInResponse SignUp(SignUpDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Password = PasswordHasher.HashPassword(dto.Password),
        };

        dbContext.Users.Add(user);

        dbContext.SaveChanges();

        var (jwt, refresh) = UpdateToken(user);

        dbContext.SaveChanges();

        return CreateResponse(jwt, refresh);
    }

    public bool VerifyToken(Guid userId, string token)
    {
        var jwtToken = dbContext.JwtTokens.FirstOrDefault(x => x.UserId == userId);
        if (jwtToken is null)
            return false;

        return jwtToken.Token == token && jwtToken.ExpiresAt > DateTime.UtcNow;
    }

    public LogInResponse? Refresh(string refreshToken)
    {
        var existingRefreshToken = dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefault(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow);

        if (existingRefreshToken is null)
            return null;

        var (jwt, refresh) = UpdateToken(existingRefreshToken.User);

        dbContext.SaveChanges();

        return CreateResponse(jwt, refresh);
    }

    public void Revoke(string refreshToken)
    {
        var existingRefreshToken = dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefault(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow);

        if (existingRefreshToken is null)
            return;

        dbContext.Remove(existingRefreshToken);
        dbContext.SaveChanges();
    }

    private (JwtToken Jwt, RefreshToken Refresh) UpdateToken(User user)
    {
        var token = jwtTokenGenerator.Generate(user);
        var oldToken = dbContext.JwtTokens.FirstOrDefault(t => t.UserId == user.Id);
        if (oldToken is not null)
        {
            dbContext.Remove(oldToken);
        }
        dbContext.JwtTokens.Add(token);

        var refreshToken = jwtTokenGenerator.GetRefreshToken(user.Id);
        dbContext.Add(refreshToken);

        return (token, refreshToken);
    }

    private static LogInResponse CreateResponse(JwtToken jwt, RefreshToken refresh)
        => new(jwt.UserId, jwt.Token, refresh.Token);
}
