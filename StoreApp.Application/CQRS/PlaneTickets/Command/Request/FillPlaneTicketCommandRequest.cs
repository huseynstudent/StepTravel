using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;

namespace StoreApp.Application.CQRS.PlaneTickets.Command.Request;

public class FillPlaneTicketCommandRequest: IRequest<ResponseModel<FillPlaneTicketCommandResponse>>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime DueDate { get; set; }
    public int ChosenSeatId { get; set; }
    public bool HasPet { get; set; } = false;
    public bool HasChild { get; set; } = false;
    public int LuggageCount { get; set; } = 0;
    public double TotalLuggageKg { get; set; } = 0;
    public string? Note { get; set; }
    public State State { get; set; } // Bu sətiri əlavə et
}
