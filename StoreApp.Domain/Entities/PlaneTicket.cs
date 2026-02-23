using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class PlaneTicket: BaseTicket
{
    public string Airline { get; set; }
    public string Gate { get; set; }
    public string Plane { get; set; }
    public string Meal { get; set; }
    public bool HasCheckedIn { get; set; } = false;
    public double LuggageKg { get; set; } 

}
