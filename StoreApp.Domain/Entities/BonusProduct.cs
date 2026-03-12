using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class BonusProduct:BaseEntity
{
    public string Name { get; set; }
    public int PricePoint { get; set; }
    public int InStock { get; set; }
    public string ImageUrl { get; set; }
    public string ImageFileName { get; set; }
}
