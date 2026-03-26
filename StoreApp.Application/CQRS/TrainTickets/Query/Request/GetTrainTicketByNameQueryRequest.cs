using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.TrainTickets.Query.Request;

public class GetTrainTicketByNameQueryRequest : IRequest<Pagination<GetAllTrainTicketQueryResponse>>
{
    public string TrainCompany { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
