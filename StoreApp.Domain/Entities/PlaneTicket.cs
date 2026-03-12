using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class PlaneTicket : BaseTicket
{
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public bool HasCheckedIn { get; set; } = false;
    public double LuggageKg { get; set; }

    public int FromId { get; set; }
    public Location From { get; set; }

    public int ToId { get; set; }
    public Location To { get; set; }

    public int SeatId { get; set; }
    public Seat Seat { get; set; }

    public int VariantId { get; set; }
    public Variant Variant { get; set; }
}