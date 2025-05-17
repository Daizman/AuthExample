using AuthExample.Abstractions;
using AuthExample.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

public class UserController(IAuthService authService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("signup")]
    public ActionResult<JwtTokenVm> SignUp([FromBody] SignUpDto dto)
    {
        var token = authService.SignUp(dto);
        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<JwtTokenVm> LogIn([FromBody] LogInDto dto)
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
}
