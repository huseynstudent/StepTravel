using MediatR;
using StoreApp.Application.CQRS.Countries.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Countries.Command.Request
{
    public class DeleteCountryCommandRequest : IRequest<ResponseModel<DeleteCountryCommandResponse>>
    {
        public int Id { get; set; }
    }
}