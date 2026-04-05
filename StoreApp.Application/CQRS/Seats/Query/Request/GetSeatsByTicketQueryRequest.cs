using MediatR;
using StoreApp.Application.CQRS.Seats.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Seats.Query.Request;

public class GetSeatsByTicketQueryRequest : IRequest<ResponseModel<List<GetSeatsByTicketQueryResponse>>>
{
    public int TicketId { get; set; }
    public string TicketType { get; set; } // "plane" or "train"
}