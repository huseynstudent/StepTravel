using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Application.Email.Commands;
using System.Threading.Tasks;

namespace StoreApp.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
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
    }
}