using MediatR;
using StoreApp.Application.CQRS.Seats.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Seats.Command.Request;

public class DeleteSeatCommandRequest: IRequest<ResponseModel<DeleteSeatCommandResponse>>
{
    public int Id { get; set; }
}
