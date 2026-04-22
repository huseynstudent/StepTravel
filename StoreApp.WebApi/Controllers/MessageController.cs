using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Query.Request;
using StoreApp.Application.CQRS.Messages.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Auth;

namespace StoreApp.WebApi.Controllers;

public class MessageController : BaseController
{
    /// <summary>
    /// Any authenticated customer can send a message (to Admin or Executive).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Customer},{Roles.Admin},{Roles.Company}")]
    public async Task<IActionResult> CreateMessage(CreateMessageCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }

    /// <summary>
    /// The original sender can edit their own message content / target.
    /// </summary>
    [HttpPut]
    [Authorize(Roles = $"{Roles.Customer},{Roles.Admin},{Roles.Company}")]
    public async Task<IActionResult> UpdateMessage(UpdateMessageCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }

    /// <summary>
    /// Admin can delete any message.
    /// </summary>
    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteMessage(DeleteMessageCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }

    /// <summary>
    /// Admin responds to a message. Automatically marks it as read.
    /// </summary>
    [HttpPut("respond")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Company}")]
    public async Task<IActionResult> RespondToMessage(RespondToMessageCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }

    /// <summary>
    /// Mark a single message as read without adding a response.
    /// </summary>
    [HttpPatch("{id}/mark-read")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Company}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        return Ok(await Sender.Send(new MarkMessageAsReadCommandRequest { Id = id }));
    }

    /// <summary>
    /// Customer: öz göndərdiyi mesajları görür.
    /// </summary>
    [HttpGet("my")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetMyMessages(
        [FromQuery] GetAllMessageQueryRequest request,
        [FromServices] StoreAppDbContext db)
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out int userId))
            return Unauthorized();

        var query = db.Messages
            .Where(m => !m.IsDeleted && m.SenderId == userId)
            .Include(m => m.Sender)
            .AsQueryable();

        if (request.ForAdmin.HasValue)
            query = query.Where(m => m.ForAdmin == request.ForAdmin.Value);

        var total = query.Count();

        var paged = query
            .OrderByDescending(m => m.CreatedDate)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .ToList();

        var data = paged.Select(m => new GetAllMessageQueryResponse
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderFullName = m.Sender != null ? $"{m.Sender.Name} {m.Sender.Surname}" : "",
            Content = m.Content,
            ForAdmin = m.ForAdmin,
            HasBeenRead = m.HasBeenRead,
            Response = m.Response,
            CreatedDate = m.CreatedDate,
        }).ToList();

        return Ok(new Pagination<GetAllMessageQueryResponse>(data, total, request.Page, request.Limit));
    }

    /// <summary>
    /// Get all messages. Optionally filter by forAdmin (true = admin inbox, false = executive inbox).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Company}")]
    public async Task<IActionResult> GetAllMessages([FromQuery] GetAllMessageQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }

    /// <summary>
    /// Get a single message by ID.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Company}")]
    public async Task<IActionResult> GetMessageById(int id)
    {
        return Ok(await Sender.Send(new GetMessageByIdQueryRequest { Id = id }));
    }
}