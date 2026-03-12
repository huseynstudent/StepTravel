using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
using StoreApp.Application.CQRS.BonusCards.Query.Request;
namespace StoreApp.WebApi.Controllers;
[AllowAnonymous]
public class BonusCardController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateBonusCard(CreateBonusCardCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateBonusCard(UpdateBonusCardCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteBonusCard(DeleteBonusCardCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllBonusCards([FromQuery] GetAllBonusCardQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBonusCardById(int id)
    {
        return Ok(await Sender.Send(new GetBonusCardByIdQueryRequest { Id = id }));
    }
}