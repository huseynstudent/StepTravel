using MediatR;
using StoreApp.Application.CQRS.Common;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.PlaneTickets.Command.Request;
public class CreatePlaneTicketCommandRequest: IRequest<ResponseModel<CreatePlaneTicketCommandResponse>>
{
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public double LuggageKg { get; set; }
    public DateTime DueDate { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public List<SeatGroupRequest> SeatGroups { get; set; } = new();
}