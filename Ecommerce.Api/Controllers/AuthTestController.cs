using Ecommerce.Api.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class AuthTestController : ControllerBase
{
    [HttpGet("api/auth-test")]
    [Authorize]

    public async Task<ActionResult<string>> GetSomething()
    {
        return "You are authenticated";
    }
    
    
    [HttpGet("api/auth-test/{id:int}")]
    [Authorize(Roles = Sd.RoleAdmin)]
    public async Task<ActionResult<string>> GetSomething(int someIntValue)
    {
        return "You are Authorized with Role of Admin";
    }
    
    
}