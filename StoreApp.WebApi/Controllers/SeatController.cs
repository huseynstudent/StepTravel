using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Seats.Command.Request;
using StoreApp.Application.CQRS.Seats.Query.Request;
namespace StoreApp.WebApi.Controllers;
[AllowAnonymous]
public class SeatController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateSeat (CreateSeatCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateSeat(UpdateSeatCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSeat(DeleteSeatCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllSeats([FromQuery] GetAllSeatQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        return Ok(await Sender.Send(new GetSeatByIdQueryRequest { Id = id }));
    }
}