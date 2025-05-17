namespace AuthExample.Models;

public partial class JwtToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
