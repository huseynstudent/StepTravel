using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>()!;
    }
}
