using AuthExample.Abstractions;
using AuthExample.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

public class UserController(IAuthService authService) : BaseController
{
    [HttpPost("signup")]
    public ActionResult<Guid> SignUp([FromBody] SignUpDto dto)
    {
        var userId = authService.SignUp(dto);
        return Ok(userId);
    }
}
