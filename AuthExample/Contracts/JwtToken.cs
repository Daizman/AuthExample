namespace AuthExample.Contracts;

public record JwtTokenVm(Guid UserId, string Token, DateTime ExpiresAt);
