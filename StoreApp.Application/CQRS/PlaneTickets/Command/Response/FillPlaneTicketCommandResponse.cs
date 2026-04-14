using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;
namespace StoreApp.Application.CQRS.PlaneTickets.Command.Response;
public class FillPlaneTicketCommandResponse
{
    public int Id { get; set; }
    public Domain.Entities.User Customer { get; set; }
    public State State { get; set; } 
    public DateTime DueDate { get; set; }
    public DateTime? BroughtDate { get; set; }
    public int? ChosenSeatId { get; set; }
    public int? VariantId { get; set; }
    public bool HasPet { get; set; } = false;
    public bool HasChild { get; set; } = false;
    public int LuggageCount { get; set; } = 0;
    public double TotalLuggageKg { get; set; } = 0;
    public double Discount { get; set; } = 1;
    public double Price { get; set; }
    public string? Note { get; set; }
}