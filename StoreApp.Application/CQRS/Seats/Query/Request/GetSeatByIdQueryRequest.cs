using MediatR;
using StoreApp.Application.CQRS.Seats.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Seats.Query.Request
{
    public class GetSeatByIdQueryRequest : IRequest<ResponseModel<GetSeatByIdQueryResponse>>
    {
        public int Id { get; set; }
    }
}
