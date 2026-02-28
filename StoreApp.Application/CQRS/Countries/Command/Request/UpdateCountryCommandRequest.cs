using MediatR;
using StoreApp.Application.CQRS.Countries.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Countries.Command.Request
{
    public class UpdateCountryCommandRequest : IRequest<ResponseModel<UpdateCountryCommandResponse>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}