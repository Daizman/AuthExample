using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthExample.Politics;

public class PostOwnerRequirementHandler(IHttpContextAccessor accessor)
    : AuthorizationHandler<PostOwnerRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PostOwnerRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var accessorResult = accessor
            .HttpContext!
            .Request
            .Query
            .TryGetValue("userId", out var userIdQuery);
        if (!accessorResult || userIdQuery.Count == 0)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (userIdClaim.Value != userIdQuery[0])
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
