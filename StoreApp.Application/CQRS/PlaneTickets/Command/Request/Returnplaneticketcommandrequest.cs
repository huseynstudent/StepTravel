using MediatR;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.PlaneTickets.Command.Request;

public class ReturnPlaneTicketCommandRequest : IRequest<ResponseModel<ReturnPlaneTicketCommandResponse>>
{
    public int Id { get; set; }
}