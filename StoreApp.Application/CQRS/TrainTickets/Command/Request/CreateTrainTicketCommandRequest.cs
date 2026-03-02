using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.TrainTickets.Command.Request
{
    public class CreateTrainTicketCommandRequest : IRequest<ResponseModel<CreateTrainTicketCommandResponse>>
    {
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
    }
}