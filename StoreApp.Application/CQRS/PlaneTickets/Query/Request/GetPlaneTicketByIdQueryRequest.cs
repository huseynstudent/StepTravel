using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;

namespace StoreApp.Application.CQRS.PlaneTickets.Query.Request;

public class GetPlaneTicketByIdQueryRequest:IRequest<GetPlaneTicketByIdQueryResponse>
{
    public int Id { get; set; }
}
