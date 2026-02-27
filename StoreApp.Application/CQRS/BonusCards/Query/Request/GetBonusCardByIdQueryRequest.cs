using MediatR;
using StoreApp.Application.CQRS.BonusCards.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.BonusCards.Query.Request
{
    public class GetBonusCardByIdQueryRequest : IRequest<ResponseModel<GetByIdBonusCardQueryResponse>>
    {
        public int Id { get; set; }
    }
}