using MediatR;
using StoreApp.Application.CQRS.Locations.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
namespace StoreApp.Application.CQRS.Locations.Command.Request
{
    public class CreateLocationCommandRequest : IRequest<ResponseModel<CreateLocationCommandResponse>>
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public int DistanceToken { get; set; }
    }
}