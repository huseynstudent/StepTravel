namespace StoreApp.Domain.Entities;

public class Variant
{
    public int Id { get; set; }
    public string Name { get; set; }="economy";
    public double Price { get; set; }
    public double AllowedLuggageKg { get; set; } = 15;
    public int AllowedLuggageCount { get; set; } = 1;
    public bool IsPriority { get; set; } = false;

}
