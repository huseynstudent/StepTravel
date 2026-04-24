namespace StoreApp.Application.CQRS.PlaneTickets.Command.Request;

public class ReturnPlaneTicketCommandResponse
{
    public int Id { get; set; }
    public string State { get; set; }
    public double Refund { get; set; }
    public string Message { get; set; }
}