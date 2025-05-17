using System.Security.Claims;

namespace AuthExample.Extensions;

public static class HttpContextExtension
{
    public static Guid? ExtractUserIdFromClaims(this HttpContext context)
    {
        var claim = context.User.Claims.FirstOrDefault(
            claim => claim.Type == ClaimTypes.NameIdentifier);
        return claim is null ? null : Guid.Parse(claim.Value);
    }
}
