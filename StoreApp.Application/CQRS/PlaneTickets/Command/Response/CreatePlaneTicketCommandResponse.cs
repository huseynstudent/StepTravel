using StoreApp.Domain.Enums;

namespace StoreApp.Application.CQRS.PlaneTickets.Command.Response;

public class CreatePlaneTicketCommandResponse
{
    public int Id { get; set; }
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public double LuggageKg { get; set; }
    public DateTime DueDate { get; set; }
    public int FromId { get; set; }
    public string State { get; set; }
    public int ToId { get; set; }
    public int TotalTicketsCreated { get; set; }
}