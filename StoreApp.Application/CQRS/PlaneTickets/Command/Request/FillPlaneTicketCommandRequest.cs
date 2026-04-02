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
    public State State { get; set; }
    public DateTime DueDate { get; set; }
    public int SeatId { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public int VariantId { get; set; }
    public bool HasPet { get; set; } = false;
    public bool HasChild { get; set; } = false;
    public bool HasCheckedIn { get; set; } = false;
    public int LuggageCount { get; set; } = 0;
    public double TotalLuggageKg { get; set; } = 0;
    public bool IsRoundTrip { get; set; } = false;
    public bool IsCashPayment { get; set; } = true;
    public string Note { get; set; }
}
