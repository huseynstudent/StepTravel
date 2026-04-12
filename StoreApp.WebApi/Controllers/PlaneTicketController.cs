using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
using StoreApp.DAL.Context;

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

    [HttpPut("fill")]
    public async Task<IActionResult> FillTicket(FillPlaneTicketCommandRequest request)
    {
        var result = await Sender.Send(request);
        if (result.Data == null)
            return BadRequest(new { message = "Bilet artıq alınıb və ya oturacaq doludur." });
        return Ok(result);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteTicket(DeletePlaneTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllTickets([FromQuery] GetAllPlaneTicketQueryRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetMyTickets([FromServices] StoreAppDbContext db)
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out int userId))
            return Unauthorized();

        var tickets = await db.PlaneTickets
            .Include(t => t.From).ThenInclude(l => l.Country)
            .Include(t => t.To).ThenInclude(l => l.Country)
            .Include(t => t.Variant)
            .Where(t => t.CustomerId == userId && !t.IsDeleted)
            .Select(t => new
            {
                t.Id,
                t.Airline,
                t.Gate,
                t.Plane,
                t.Meal,
                t.LuggageKg,
                t.HasPet,
                t.HasChild,
                t.LuggageCount,
                t.TotalLuggageKg,
                t.Note,
                t.Price,
                t.Discount,
                t.DueDate,
                t.BroughtDate,
                State = t.State.ToString(),
                From = t.From != null ? t.From.Name : null,
                To = t.To != null ? t.To.Name : null,
                Variant = t.Variant != null ? new
                {
                    t.Variant.Name,
                    t.Variant.AllowedLuggageKg,
                    t.Variant.AllowedLuggageCount,
                    t.Variant.IsPriority
                } : null,
                Seat = t.ChosenSeatId != null
                    ? db.Seats.Where(s => s.Id == t.ChosenSeatId).Select(s => new { s.Name }).FirstOrDefault()
                    : null,
            })
            .OrderByDescending(t => t.DueDate)
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTicketById(int id)
        => Ok(await Sender.Send(new GetPlaneTicketByIdQueryRequest { Id = id }));
}