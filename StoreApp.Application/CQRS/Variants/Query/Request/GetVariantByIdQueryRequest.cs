using MediatR;
using StoreApp.Application.CQRS.Variants.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Variants.Query.Request
{
    public class GetVariantByIdQueryRequest : IRequest<ResponseModel<GetVariantByIdQueryResponse>>
    {
        public int Id { get; set; }
    }
}