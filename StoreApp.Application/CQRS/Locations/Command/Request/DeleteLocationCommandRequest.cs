using MediatR;
using StoreApp.Application.CQRS.Locations.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Locations.Command.Request
{
    public class DeleteLocationCommandRequest : IRequest<ResponseModel<DeleteLocationCommandResponse>>
    {
        public int Id { get; set; }
    }
}