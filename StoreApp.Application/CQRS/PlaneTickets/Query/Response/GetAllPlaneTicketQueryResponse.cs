namespace StoreApp.Application.CQRS.PlaneTickets.Query.Response;

public class GetAllPlaneTicketQueryResponse
{
    public int Id { get; set; }
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public double LuggageKg { get; set; }
    public DateTime DueDate { get; set; }
    public string From { get; set; }
    public string To { get; set; }
}
