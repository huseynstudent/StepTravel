using MediatR;
using StoreApp.Application.CQRS.Countries.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Countries.Query.Request
{
    public class GetCountryByIdQueryRequest : IRequest<ResponseModel<GetCountryByIdQueryResponse>>
    {
        public int Id { get; set; }
    }
}