using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.PlaneTickets.Command.Request;

public class DeletePlaneTicketCommandRequest: IRequest<ResponseModel<DeletePlaneTicketCommandResponse>>
{
    public int Id { get; set; }
}
