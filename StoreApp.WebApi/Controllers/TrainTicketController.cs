using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;

namespace StoreApp.WebApi.Controllers;

[Authorize]
public class TrainTicketController : BaseController
{
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CreateTicket(CreateTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpPut]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> UpdateTicket(UpdateTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpDelete]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> DeleteTicket(DeleteTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet]
    [AllowAnonymous]  // ← bilet siyahısı hər kəsə açıqdır
    public async Task<IActionResult> GetAllTickets([FromQuery] GetAllTrainTicketQueryRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet("{id}")]
    [AllowAnonymous]  // ← tək bilet də açıqdır
    public async Task<IActionResult> GetTicketById(int id)
        => Ok(await Sender.Send(new GetTrainTicketByIdQueryRequest { Id = id }));
}