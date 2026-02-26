namespace StoreApp.Domain.Entities;

public class BonusCard
{
    public int Id { get; set; }
    public int CardNumber { get; set; }
    public double Points { get; set; } = 0;
}
