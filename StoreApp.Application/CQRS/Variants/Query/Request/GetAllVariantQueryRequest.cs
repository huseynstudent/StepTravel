using MediatR;
using StoreApp.Application.CQRS.Variants.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Variants.Query.Request
{
    public class GetAllVariantQueryRequest : IRequest<Pagination<GetAllVariantQueryResponse>>
    {
        public int Limit { get; set; } = 15;
        public int Page { get; set; } = 1;
    }
}