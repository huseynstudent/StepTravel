namespace StoreApp.Application.CQRS.Seats.Query.Response;

public class GetSeatByIdQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public int VariantId { get; set; }
}
