namespace StoreApp.Application.CQRS.PlaneTickets.Query.Response;

public class GetAllPlaneTicketQueryResponse
{
    public int Id { get; set; } // ID of the first available seat in the group
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public DateTime DueDate { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public double Price { get; set; }
    public int AvailableSeats { get; set; }
}