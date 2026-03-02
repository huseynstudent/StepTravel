using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.TrainTickets.Query.Request
{
    public class GetAllTrainTicketQueryRequest : IRequest<Pagination<GetAllTrainTicketQueryResponse>>
    {
        public int Limit { get; set; } = 15;
        public int Page { get; set; } = 1;
    }
}