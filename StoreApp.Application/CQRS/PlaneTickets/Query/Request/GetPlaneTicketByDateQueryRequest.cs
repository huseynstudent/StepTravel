using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.PlaneTickets.Query.Request;

public class GetPlaneTicketByDateQueryRequest : IRequest<Pagination<GetAllPlaneTicketQueryResponse>>
{
    public DateTime Date { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
