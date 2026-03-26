using Microsoft.AspNetCore.Mvc;

namespace StoreApp.WebApi.Controllers;

using StoreApp.Application.CQRS.Auth.Command.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[AllowAnonymous]
public class AuthController : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommandRequest request)
    {
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommandRequest request)
    {
        var result = await Sender.Send(request);
        return Ok(result);
    }
}