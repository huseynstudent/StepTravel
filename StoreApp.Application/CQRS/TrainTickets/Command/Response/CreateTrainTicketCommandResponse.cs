using StoreApp.Application.CQRS.Common;

namespace StoreApp.Application.CQRS.TrainTickets.Command.Response
{
    public class CreateTrainTicketCommandResponse
    {
        public int Id { get; set; }
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
        public DateTime DueDate { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int TotalTicketsCreated { get; set; }
    }
}