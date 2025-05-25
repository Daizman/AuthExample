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
    public async Task<LogInResponse?> LogInAsync(LogInDto dto)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
        if (user is null)
        {
            return null;
        }

        if (!PasswordHasher.VerifyPassword(user.Password, dto.Password))
        {
            return null;
        }

        var (jwt, refresh) = UpdateToken(user);
        await dbContext.SaveChangesAsync();

        return CreateResponse(jwt, refresh);
    }

    public async Task<bool> LogOutAsync(Guid userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return false;
        }

        var token = await dbContext.JwtTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        if (token is null)
        {
            return false;
        }
        dbContext.Remove(token);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<LogInResponse> SignUpAsync(SignUpDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Password = PasswordHasher.HashPassword(dto.Password),
        };

        await dbContext.AddAsync(user);

        await dbContext.SaveChangesAsync();

        var (jwt, refresh) = UpdateToken(user);

        await dbContext.SaveChangesAsync();

        return CreateResponse(jwt, refresh);
    }

    public async Task<bool> VerifyTokenAsync(Guid userId, string token)
    {
        var jwtToken = await dbContext.JwtTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        if (jwtToken is null)
        {
            return false;
        }

        return jwtToken.Token == token && jwtToken.ExpiresAt > DateTime.UtcNow;
    }

    public async Task<LogInResponse?> RefreshAsync(string refreshToken)
    {
        var existingRefreshToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow);

        if (existingRefreshToken is null)
        {
            return null;
        }

        var (jwt, refresh) = UpdateToken(existingRefreshToken.User);

        await dbContext.SaveChangesAsync();

        return CreateResponse(jwt, refresh);
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var existingRefreshToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow);

        if (existingRefreshToken is null)
        {
            return;
        }

        dbContext.Remove(existingRefreshToken);
        await dbContext.SaveChangesAsync();
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
