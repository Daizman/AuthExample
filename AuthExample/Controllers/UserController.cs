using AuthExample.Abstractions;
using AuthExample.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

public class UserController(IAuthService authService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("signup")]
    public ActionResult<LogInResponse> SignUp([FromBody] SignUpDto dto)
    {
        var token = authService.SignUp(dto);
        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LogInResponse> LogIn([FromBody] LogInDto dto)
    {
        var result = authService.LogIn(dto);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("logout")]
    public ActionResult<bool> LogOut([FromBody] Guid userId)
    {
        var result = authService.LogOut(userId);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("refresh")]
    public ActionResult<LogInResponse> Refresh([FromBody] string refreshToken)
    {
        var result = authService.Refresh(refreshToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("revoke")]
    public ActionResult Revoke([FromBody] string refreshToken)
    {
        authService.Revoke(refreshToken);

        return NoContent();
    }
}
