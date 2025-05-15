using AuthExample.Abstractions;
using AuthExample.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

public class UserController(IAuthService userRepository) : BaseController
{
    [HttpPost("signup")]
    public ActionResult<Guid> SignUp([FromBody] SignUpDto dto)
    {
        var userId = userRepository.SignUp(dto);
        return Ok(userId);
    }

    [HttpPost("login")]
    public ActionResult<bool> LogIn([FromBody] LogInDto dto)
    {
        var result = userRepository.LogIn(dto);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("logout")]
    public ActionResult<bool> LogOut([FromBody] Guid userId)
    {
        var result = userRepository.LogOut(userId);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
