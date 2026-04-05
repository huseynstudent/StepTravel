using MediatR;
using StoreApp.Application.CQRS.Common;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.TrainTickets.Command.Request
{
    public class CreateTrainTicketCommandRequest : IRequest<ResponseModel<CreateTrainTicketCommandResponse>>
    {
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
        public DateTime DueDate { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public List<SeatGroupRequest> SeatGroups { get; set; } = new();
    }
}