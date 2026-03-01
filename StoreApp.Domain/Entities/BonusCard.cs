using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class BonusCard : BaseEntity
{
    public string CardNumber { get; set; }
    public double Points { get; set; }
}
