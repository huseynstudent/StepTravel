using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.BonusProducts.Query.Request
{
    public class GetAllBonusProductQueryRequest
        : IRequest<Pagination<GetAllBonusProductQueryResponse>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}