using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Seats.Command.Request;
using StoreApp.Application.CQRS.Seats.Query.Request;
namespace StoreApp.WebApi.Controllers;
[Authorize(Roles = "Admin")]
public class SeatController : BaseController
{
    [HttpGet("by-ticket")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeatsByTicket([FromQuery] GetSeatsByTicketQueryRequest request)
    => Ok(await Sender.Send(request));
    [HttpPost]
    public async Task<IActionResult> CreateSeat (CreateSeatCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateSeat(UpdateSeatCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteSeat(DeleteSeatCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSeats([FromQuery] GetAllSeatQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        return Ok(await Sender.Send(new GetSeatByIdQueryRequest { Id = id }));
    }
}