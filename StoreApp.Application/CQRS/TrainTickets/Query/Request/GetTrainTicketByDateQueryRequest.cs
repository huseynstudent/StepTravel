using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.TrainTickets.Query.Request;

public class GetTrainTicketByDateQueryRequest : IRequest<Pagination<GetAllTrainTicketQueryResponse>>
{
    public DateTime Date { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
