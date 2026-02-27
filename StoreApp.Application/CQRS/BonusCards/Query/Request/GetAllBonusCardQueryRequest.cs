using MediatR;
using StoreApp.Application.CQRS.BonusCards.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.BonusCards.Query.Request
{
    public class GetAllBonusCardQueryRequest : IRequest<Pagination<GetAllBonusCardQueryResponse>>
    {
        public int Limit { get; set; } = 15;
        public int Page { get; set; } = 1;
    }
}