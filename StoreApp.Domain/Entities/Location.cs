
using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Location: BaseEntity
{
    public string Name { get; set; }
    public Country Country { get; set; }
    public int CountryId { get; set; }
    public int DistanceToken { get; set; }
}



