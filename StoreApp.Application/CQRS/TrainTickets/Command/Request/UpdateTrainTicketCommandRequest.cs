using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
namespace StoreApp.Application.CQRS.TrainTickets.Command.Request
{
    public class UpdateTrainTicketCommandRequest : IRequest<ResponseModel<UpdateTrainTicketCommandResponse>>
    {
        public int Id { get; set; }
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
        public State State { get; set; }
    }
}