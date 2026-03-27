using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;

namespace StoreApp.WebApi.Controllers;

[AllowAnonymous]
public class TrainTicketController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreateTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpPut]
    public async Task<IActionResult> UpdateTicket(UpdateTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpDelete]
    public async Task<IActionResult> DeleteTicket(DeleteTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet]
    public async Task<IActionResult> GetAllTickets([FromQuery] GetAllTrainTicketQueryRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(int id)
        => Ok(await Sender.Send(new GetTrainTicketByIdQueryRequest { Id = id }));
}