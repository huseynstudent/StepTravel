using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Country : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}