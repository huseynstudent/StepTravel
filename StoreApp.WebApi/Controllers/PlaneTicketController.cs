using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
namespace StoreApp.WebApi.Controllers;
[AllowAnonymous]
public class PlaneTicketController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreatePlaneTicketCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateTicket(UpdatePlaneTicketCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(DeletePlaneTicketCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllTickets([FromQuery] GetAllPlaneTicketQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(int id)
    {
        return Ok(await Sender.Send(new GetPlaneTicketByIdQueryRequest { Id = id }));
    }
}