using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.TrainTickets.Command.Request
{
    public class DeleteTrainTicketCommandRequest : IRequest<ResponseModel<DeleteTrainTicketCommandResponse>>
    {
        public int Id { get; set; }
    }

    public class DeleteTrainTicketGroupCommandRequest : IRequest<ResponseModel<DeleteTrainTicketCommandResponse>>
    {
        public int Id { get; set; }
    }
}