using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Seat : BaseEntity
{
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public Variant Variant { get; set; }
    public int VariantId { get; set; }
    public int? PlaneTicketId { get; set; }
    public int? TrainTicketId { get; set; }
}