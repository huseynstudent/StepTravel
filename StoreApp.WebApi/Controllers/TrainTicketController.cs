using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
namespace StoreApp.WebApi.Controllers;
[AllowAnonymous]
public class TrainTicketController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateTrainTicket(CreateTrainTicketCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateTrainTicket(UpdateTrainTicketCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteTrainTicket(DeleteTrainTicketCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllTrainTickets([FromQuery] GetAllTrainTicketQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrainTicketById(int id)
    {
        return Ok(await Sender.Send(new GetTrainTicketByIdQueryRequest { Id = id }));
    }
    [HttpGet("by-name")]
    public async Task<IActionResult> GetTrainTicketsByName([FromQuery] GetTrainTicketByNameQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }

    [HttpGet("by-date")]
    public async Task<IActionResult> GetTrainTicketsByDate([FromQuery] GetTrainTicketByDateQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
}