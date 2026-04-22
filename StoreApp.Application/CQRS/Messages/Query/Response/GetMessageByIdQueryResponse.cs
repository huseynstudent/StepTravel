namespace StoreApp.Application.CQRS.Messages.Query.Response;

public class GetMessageByIdQueryResponse
{
    public int Id { get; set; }
    public int? SenderId { get; set; }
    public string SenderFullName { get; set; }
    public string SenderEmail { get; set; }
    public string Content { get; set; }
    public bool ForAdmin { get; set; }
    public bool HasBeenRead { get; set; }
    public string? Response { get; set; }
    public DateTime CreatedDate { get; set; }
}