using MediatR;
using StoreApp.Application.CQRS.Locations.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Locations.Query.Request
{
    public class GetLocationByIdQueryRequest : IRequest<ResponseModel<GetLocationByIdQueryResponse>>
    {
        public int Id { get; set; }
    }
}