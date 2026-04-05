namespace StoreApp.Application.CQRS.TrainTickets.Command.Request;

using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using MediatR;


public class FillTrainTicketCommandRequest : IRequest<ResponseModel<FillTrainTicketCommandResponse>>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public State State { get; set; }
    public DateTime DueDate { get; set; }
    public int ChosenSeatId { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public bool HasPet { get; set; } = false;
    public bool HasChild { get; set; } = false;
    public int LuggageCount { get; set; } = 0;
    public double TotalLuggageKg { get; set; } = 0;
    public bool IsRoundTrip { get; set; } = false;
    public bool IsCashPayment { get; set; } = true;
    public string Note { get; set; }
}
