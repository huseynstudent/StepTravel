using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

public class DeletePlaneTicketGroupCommandRequest : IRequest<ResponseModel<DeletePlaneTicketCommandResponse>>
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
}