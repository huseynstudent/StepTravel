using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;

public class UpdatePlaneTicketGroupCommandRequest : IRequest<ResponseModel<List<UpdatePlaneTicketCommandResponse>>>
{
    public string Airline { get; set; }
    public string Plane { get; set; }
    public string Gate { get; set; }
    public bool Meal { get; set; }
    public double LuggageKg { get; set; }
    public DateTime DueDate { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public int? VariantId { get; set; }

    // What to apply to all matched tickets
    public string NewAirline { get; set; }
    public string NewGate { get; set; }
    public bool NewMeal { get; set; }
    public double NewLuggageKg { get; set; }
    public State NewState { get; set; }
}
