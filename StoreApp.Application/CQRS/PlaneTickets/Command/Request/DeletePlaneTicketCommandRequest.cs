using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

public class DeletePlaneTicketGroupCommandRequest : IRequest<ResponseModel<DeletePlaneTicketCommandResponse>>
{
    public int Id { get; set; }
}