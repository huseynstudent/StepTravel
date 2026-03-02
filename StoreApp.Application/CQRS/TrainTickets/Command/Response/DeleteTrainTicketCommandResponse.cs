namespace StoreApp.Application.CQRS.TrainTickets.Command.Response
{
    public class DeleteTrainTicketCommandResponse
    {
        public int Id { get; set; }
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
    }
}