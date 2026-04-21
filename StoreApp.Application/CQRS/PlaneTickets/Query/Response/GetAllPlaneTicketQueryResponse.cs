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
    public DateTime? ArrivalDate { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }  
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string State { get; set; }
    public int? VariantId { get; set; }
    public string VariantName { get; set; }
}