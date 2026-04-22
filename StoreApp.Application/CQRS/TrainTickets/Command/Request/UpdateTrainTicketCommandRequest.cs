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
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTrainTicketGroupCommandRequest : IRequest<ResponseModel<List<UpdateTrainTicketCommandResponse>>>
    {
        // Qrup axtarış sahələri (hansı biletlər dəyişdiriləcək)
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
        public DateTime DueDate { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int? VariantId { get; set; }

        // Yeni dəyərlər
        public string NewTrainCompany { get; set; }
        public string NewTrainNumber { get; set; }
        public int NewVagonNumber { get; set; }
        public State NewState { get; set; }
        public DateTime? NewDueDate { get; set; }
    }
}