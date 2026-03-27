namespace StoreApp.Application.CQRS.TrainTickets.Query.Response;

public class GetAllTrainTicketQueryResponse
{
    public int Id { get; set; }
    public string TrainCompany { get; set; }
    public string TrainNumber { get; set; }
    public int VagonNumber { get; set; }
    public DateTime DueDate { get; set; }

    public string From { get; set; }
    public string To { get; set; }
}