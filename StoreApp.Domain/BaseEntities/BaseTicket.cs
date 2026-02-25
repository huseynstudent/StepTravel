using StoreApp.Domain.Entities;

namespace StoreApp.Domain.BaseEntities;

public class BaseTicket
{
    public int Id { get; set; }
    //public User Customer { get; set; } //User ve Role yazanda elave edecem

    //public State State { get; set; } //State enum-"Canceled", "Delayed","Pending","Expired"
    public Seat Seat { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime BroughtDate { get; set; }
    public Location From { get; set; }
    public Location To { get; set; }
    public Variant Variant { get; set; }
    public bool HasPet { get; set; } = false;
    public bool HasChild { get; set; } = false;
    public int LuggageCount { get; set; } = 0;
    public double TotalLuggageKg { get; set; } = 0;
    public double Discount { get; set; } = 1;
    public bool IsRoundTrip { get; set; } = false;
    public bool IsCashPayment { get; set; } = true;
    public double Price { get; set; }
    public string Note { get; set; }
}
