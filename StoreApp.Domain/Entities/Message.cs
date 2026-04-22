using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class Message : BaseEntity
{
    public int? SenderId { get; set; }
    public User? Sender { get; set; }

    public string Content { get; set; }
    public bool ForAdmin { get; set; }
    public bool HasBeenRead { get; set; } = false;
    public string? Response { get; set; }
}