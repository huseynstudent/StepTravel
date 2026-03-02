namespace StoreApp.Application.CQRS.TrainTickets.Query.Response
{
    class GetAllTrainTicketQueryResponse
    {
        public int Id { get; set; }
        public string TrainCompany { get; set; }
        public string TrainNumber { get; set; }
        public int VagonNumber { get; set; }
    }
}