using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.BonusProducts.Command.Request;
using StoreApp.Application.CQRS.BonusProducts.Query.Request;
using StoreApp.Domain.Auth;
namespace StoreApp.WebApi.Controllers;
[Authorize(Roles = "Admin,Company")]
public class BonusProductController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateBonusProduct(CreateBonusProductCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateBonusProduct(UpdateBonusProductCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteBonusProduct(DeleteBonusProductCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllBonusProducts([FromQuery] GetAllBonusProductQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBonusProductById(int id)
    {
        return Ok(await Sender.Send(new GetBonusProductByIdQueryRequest { Id = id }));
    }
}