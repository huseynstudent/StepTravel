namespace StoreApp.Application.CQRS.Seats.Query.Response;

public class GetSeatsByTicketQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOccupied { get; set; }
    public int VariantId { get; set; }
    public string VariantName { get; set; }
    public double VariantPrice { get; set; }
}