using MediatR;
using StoreApp.Application.CQRS.Countries.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Countries.Query.Request
{
    public class GetAllCountryQueryRequest : IRequest<Pagination<GetAllCountryQueryResponse>>
    {
        public int Limit { get; set; } = 15;
        public int Page { get; set; } = 1;
    }
}