using MediatR;
using StoreApp.Application.CQRS.Seats.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Seats.Command.Request;

public class CreateSeatCommandRequest: IRequest<ResponseModel<CreateSeatCommandResponse>>
{
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public int VariantId { get; set; }
}