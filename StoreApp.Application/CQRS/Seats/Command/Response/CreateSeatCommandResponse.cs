namespace StoreApp.Application.CQRS.Seats.Command.Response;

public class CreateSeatCommandResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public int VariantId { get; set; }
}
