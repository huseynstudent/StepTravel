using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Seat : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public Variant Variant { get; set; }
}
