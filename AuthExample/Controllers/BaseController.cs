using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseController : ControllerBase;
