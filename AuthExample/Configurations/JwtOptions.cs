using System.ComponentModel.DataAnnotations;

namespace AuthExample.Configurations;

public class JwtOptions
{
    [Required]
    public required string Issuer { get; init; }
    [Required]
    public required string Audience { get; init; }
    [Required]
    public required string Secret { get; init; }
}
