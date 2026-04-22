using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;

namespace StoreApp.Application.CQRS.PlaneTickets.Command.Request;

public class UpdatePlaneTicketGroupCommandRequest : IRequest<ResponseModel<List<UpdatePlaneTicketCommandResponse>>>
{
    public string Airline { get; set; }
    public string Plane { get; set; }
    public string Gate { get; set; }
    public string Meal { get; set; }
    public double LuggageKg { get; set; }
    public DateTime DueDate { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public int? VariantId { get; set; }
    public string NewAirline { get; set; }
    public string NewGate { get; set; }
    public string NewMeal { get; set; }
    public double NewLuggageKg { get; set; }
    public State NewState { get; set; }
    public DateTime? NewDueDate { get; set; }
}

public class UpdatePlaneTicketCommandRequest : IRequest<ResponseModel<UpdatePlaneTicketCommandResponse>>
{
    public int Id { get; set; }
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public double LuggageKg { get; set; }
    public State State { get; set; }
    public int? VariantId { get; set; }
}