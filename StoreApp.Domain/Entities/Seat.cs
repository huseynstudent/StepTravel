using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Seat : BaseEntity
{
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    // public User OccupiedBy { get; set; }
    public Variant Variant { get; set; }
    public int VariantId { get; set; }
}
