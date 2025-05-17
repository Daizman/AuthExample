using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthExample.Abstractions;
using AuthExample.Configurations;
using AuthExample.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthExample.Services;

public class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public JwtToken Generate(User user)
    {
        SigningCredentials credentials = new(
            new SymmetricSecurityKey(Convert.FromBase64String(_options.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var now = DateTime.UtcNow;
        var expiration = now.AddMinutes(5);

        JwtSecurityToken securityToken = new(
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: expiration,
            claims: claims,
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return new JwtToken
        {
            UserId = user.Id,
            Token = token,
            CreatedAt = now,
            ExpiresAt = expiration,
        };
    }

    public RefreshToken GetRefreshToken(Guid userId) => new()
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        UserId = userId,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
    };
}
