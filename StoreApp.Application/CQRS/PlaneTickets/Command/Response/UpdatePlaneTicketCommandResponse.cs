namespace StoreApp.Application.CQRS.PlaneTickets.Command.Response;
public class UpdatePlaneTicketCommandResponse
{
    public int Id { get; set; }
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public double LuggageKg { get; set; }
}