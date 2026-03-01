using MediatR;
using StoreApp.Application.CQRS.Seats.Command.Response;

namespace StoreApp.Application.CQRS.Seats.Command.Request;

public class CreateSeatCommandRequest: IRequest<CreateSeatCommandResponse>
{
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public int VariantId { get; set; }
}
