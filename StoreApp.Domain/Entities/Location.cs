
using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Location: BaseEntity
{
    public string Name { get; set; }
    public int CountryId { get; set; } = 1;//default: Baku 
    public int DistanceToken { get; set; }
}
