using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.TrainTickets.Query.Request;

public class GetAllTrainTicketQueryRequest : IRequest<Pagination<GetAllTrainTicketQueryResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public DateTime? Date { get; set; }
    public string? TrainCompany { get; set; }
    public int? FromLocationId { get; set; }
    public int? ToLocationId { get; set; }
}