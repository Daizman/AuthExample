using AuthExample.Abstractions;
using AuthExample.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

public class UserController(IAuthService authService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<ActionResult<LogInResponse>> SignUp([FromBody] SignUpDto dto)
    {
        var token = await authService.SignUpAsync(dto);
        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LogInResponse>> LogIn([FromBody] LogInDto dto)
    {
        var result = await authService.LogInAsync(dto);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<bool>> LogOut([FromBody] Guid userId)
    {
        var result = await authService.LogOutAsync(userId);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LogInResponse>> Refresh([FromBody] string refreshToken)
    {
        var result = await authService.RefreshAsync(refreshToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("revoke")]
    public async Task<ActionResult> Revoke([FromBody] string refreshToken)
    {
        await authService.RevokeAsync(refreshToken);

        return NoContent();
    }
}
