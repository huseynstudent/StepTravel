using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; }
}