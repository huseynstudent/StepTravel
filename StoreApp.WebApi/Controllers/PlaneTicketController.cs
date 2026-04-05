using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;

namespace StoreApp.WebApi.Controllers;

[Authorize]
public class PlaneTicketController : BaseController
{
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CreateTicket(CreatePlaneTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpPut]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> UpdateTicket(UpdatePlaneTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpDelete]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> DeleteTicket(DeletePlaneTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet]
    public async Task<IActionResult> GetAllTickets([FromQuery] GetAllPlaneTicketQueryRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(int id)
        => Ok(await Sender.Send(new GetPlaneTicketByIdQueryRequest { Id = id }));
}