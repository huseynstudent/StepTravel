using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.TrainTickets.Query.Request
{
    public class GetTrainTicketByIdQueryRequest : IRequest<ResponseModel<GetTrainTicketByIdQueryResponse>>
    {
        public int Id { get; set; }
    }
}