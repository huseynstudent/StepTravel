using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Query.Request;
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
