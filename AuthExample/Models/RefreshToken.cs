namespace AuthExample.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
