using MediatR;
using StoreApp.Application.CQRS.Seats.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Seats.Query.Request;

public class GetAllSeatQueryRequest : IRequest<Pagination<GetAllSeatQueryResponse>>
{
    public int Limit { get; set; } = 15;
    public int Page { get; set; } = 1;
}
