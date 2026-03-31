using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Variants.Command.Request;
using StoreApp.Application.CQRS.Variants.Query.Request;
namespace StoreApp.WebApi.Controllers;
[Authorize(Roles = "Admin")]
public class VariantController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateVariant(CreateVariantCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateVariant(UpdateVariantCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteVariant(DeleteVariantCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllVariants([FromQuery] GetAllVariantQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVariantById(int id)
    {
        return Ok(await Sender.Send(new GetVariantByIdQueryRequest { Id = id }));
    }
}