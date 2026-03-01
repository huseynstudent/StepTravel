using MediatR;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Application.CQRS.Seats.Command.Response;

namespace StoreApp.Application.CQRS.Seats.Command.Request
{
    public class UpdateSeatCommandRequest : IRequest<ResponseModel<UpdateSeatCommandResponse>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOccupied { get; set; }
        public int VariantId { get; set; }
    }
}
