namespace StoreApp.Application.CQRS.Common;

public class SeatGroupRequest
{
    public int VariantId { get; set; }
    public int RowCount { get; set; }       // how many rows
    public int SeatsPerRow { get; set; }    // how many seats per row (e.g. 6 for A-F)
}