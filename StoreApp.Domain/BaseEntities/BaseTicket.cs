using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;

namespace StoreApp.Domain.BaseEntities;

public class BaseTicket: BaseEntity
{
    public User? Customer { get; set; }
    public int? CustomerId { get; set; }
    public State State { get; set; } //State enum-"Canceled", "Delayed","Pending","Expired"
    public DateTime DueDate { get; set; }
    public DateTime? BroughtDate { get; set; }
    public Location From { get; set; }
    public Location To { get; set; }
    public Variant? Variant { get; set; }
    public int? ChosenSeatId { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public int? VariantId { get; set; }
    public bool HasPet { get; set; } = false;
    public bool HasChild { get; set; } = false;
    public int LuggageCount { get; set; } = 0;
    public double TotalLuggageKg { get; set; } = 0;
    public double Discount { get; set; } = 1;
    public bool IsRoundTrip { get; set; } = false;
    public double Price { get; set; }
    public string? Note { get; set; }
}
