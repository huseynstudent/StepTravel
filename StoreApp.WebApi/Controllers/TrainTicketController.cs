using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
using StoreApp.DAL.Context;

namespace StoreApp.WebApi.Controllers;

[Authorize]
public class TrainTicketController : BaseController
{
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CreateTicket(CreateTrainTicketCommandRequest request)
        => Ok(await Sender.Send(request));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTrainTicketCommandRequest request)
    {
        request.Id = id;
        var result = await Sender.Send(request);
        if (result?.Data == null)
            return BadRequest(new { message = "Ticket not found or cannot be updated." });
        return Ok(result);
    }

    [HttpPut("group")]
    [Authorize(Roles = "Admin,Company")]
    public async Task<IActionResult> UpdateTicketGroup([FromBody] UpdateTrainTicketGroupCommandRequest request)
    {
        var result = await Sender.Send(request);
        if (result?.Data == null)
            return BadRequest(new { message = "Ticket group not found or cannot be updated." });
        return Ok(result);
    }

    [HttpPut("fill")]
    public async Task<IActionResult> FillTicket(FillTrainTicketCommandRequest request)
    {
        var result = await Sender.Send(request);
        if (result.Data == null)
            return BadRequest(new { message = "Bilet artıq alınıb və ya oturacaq doludur." });
        return Ok(result);
    }

    [HttpPost("return/{id:int}")]
    public async Task<IActionResult> ReturnTicket(int id)
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out int userId))
            return Unauthorized();

        var ticket = await HttpContext.RequestServices
            .GetRequiredService<StoreAppDbContext>()
            .TrainTickets
            .FirstOrDefaultAsync(t => t.Id == id && t.CustomerId == userId && !t.IsDeleted);

        if (ticket == null)
            return NotFound(new { message = "Ticket not found or does not belong to you." });

        var result = await Sender.Send(new ReturnTrainTicketCommandRequest { Id = id });

        if (result?.Data == null)
            return BadRequest(new { message = "Ticket cannot be returned. It may have already departed or been cancelled." });

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTicket(int id)
        => Ok(await Sender.Send(new DeleteTrainTicketGroupCommandRequest { Id = id }));

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllTickets([FromQuery] GetAllTrainTicketQueryRequest request)
        => Ok(await Sender.Send(request));

    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetMyTickets([FromServices] StoreAppDbContext db)
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out int userId))
            return Unauthorized();

        var tickets = await db.TrainTickets
            .Include(t => t.From).ThenInclude(l => l.Country)
            .Include(t => t.To).ThenInclude(l => l.Country)
            .Include(t => t.Variant)
            .Where(t => t.CustomerId == userId && !t.IsDeleted)
            .Select(t => new
            {
                t.Id,
                t.TrainCompany,
                t.TrainNumber,
                t.VagonNumber,
                t.HasPet,
                t.HasChild,
                t.LuggageCount,
                t.TotalLuggageKg,
                t.Note,
                Price = t.Price != 0
                    ? t.Price
                    : db.Seats
                        .Where(s => s.TrainTicketId == t.Id && s.Variant != null)
                        .Min(s => (double?)s.Variant.Price) ?? 0,
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
                    ? db.Seats
                        .Where(s => s.Id == t.ChosenSeatId)
                        .Select(s => new { s.Name })
                        .FirstOrDefault()
                    : null,
            })
            .OrderByDescending(t => t.DueDate)
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTicketById(int id)
        => Ok(await Sender.Send(new GetTrainTicketByIdQueryRequest { Id = id }));
}