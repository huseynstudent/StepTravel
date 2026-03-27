using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.PlaneTickets.Query.Request;

public class GetAllPlaneTicketQueryRequest : IRequest<Pagination<GetAllPlaneTicketQueryResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public DateTime? Date { get; set; }
    public string? Airline { get; set; }
    public int? FromLocationId { get; set; }
    public int? ToLocationId { get; set; }
}
