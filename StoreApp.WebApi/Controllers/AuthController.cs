using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Application.CQRS.Auth.Query.Request;
using StoreApp.Application.CQRS.User.Command.Request;
using StoreApp.Application.CQRS.Users.Command.Request;
using StoreApp.Application.Email.Commands;
using System.Threading.Tasks;

namespace StoreApp.WebApi.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirst("uid")!.Value);
        return Ok(await Sender.Send(new GetMeQueryRequest { UserId = userId }));
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommandRequest request)
    {
        var result = await Sender.Send(request);

        if (result.Data == null)
        {
            return BadRequest(new { message = "Registration failed. Email might already exist." });
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest request)
    {
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand request)
    {
        var result = await Sender.Send(request);

        if (result)
        {
            return Ok(true);
        }

        return BadRequest(false);
    }
    [Authorize]
    [HttpPut("edit-profile")]
    public async Task<IActionResult> EditProfile([FromBody] EditProfileCommandRequest request)
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out int userId))
            return Unauthorized();

        request.UserId = userId;

        var result = await Sender.Send(request);
        if (result.Data is null)
            return BadRequest(new { message = "Profile update failed. Email may already be in use." });

        return Ok(result);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommandRequest request)
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out int userId))
            return Unauthorized();

        request.UserId = userId;

        var result = await Sender.Send(request);
        if (result.Data?.IsSuccess == false)
            return BadRequest(new { message = result.Data.Message });

        return Ok(result);
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommandRequest request)
    {
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommandRequest request)
    {
        var result = await Sender.Send(request);
        if (result.Data?.Success == false)
            return BadRequest(new { message = result.Data.Message });

        return Ok(result);
    }

}