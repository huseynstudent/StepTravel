using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.BonusProducts.Query.Request;

public class GetBonusProductByIdQueryRequest
    : IRequest<ResponseModel<GetBonusProductByIdQueryResponse>>
{
    public int Id { get; set; }
}